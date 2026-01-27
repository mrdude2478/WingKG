using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WingIDEKeygen
{
    public partial class MainForm : Form    // ← or public partial class Form1 : Form
    {
        private static List<string> VersionList;
        private static Dictionary<string, int[]> VersionMagics;
        private static readonly string BASE16 = "0123456789ABCDEF";
        private static readonly string BASE30 = "123456789ABCDEFGHJKLMNPQRTVWXY";
        string goodchars = "123456789ABCDEFGHJKLMNPQRTVY";
        Random rng = new Random();

        //function used for sorting version strings like "10.X.X", "9.X.X", etc.
        private static int CompareVersions(string a, string b)
        {
            int va = ExtractMajorVersion(a);
            int vb = ExtractMajorVersion(b);

            // ascending (5 -> xx)
            return va.CompareTo(vb);
        }

        //function used for sorting version strings like "10.X.X", "9.X.X", etc.
        private static int ExtractMajorVersion(string version)
        {
            int dot = version.IndexOf('.');
            if (dot < 0)
                return 0;

            int value;
            if (int.TryParse(version.Substring(0, dot), out value))
                return value;

            return 0;
        }


        //fixed default version list and magic numbers
        private static readonly string[] DefaultVersionList = {
            "5.X.X",
            "6.X.X",
            "7.X.X",
            "8.X.X",
            "9.X.X",
            "10.X.X",
            "11.X.X",
            //"12.X.X",
        };

        /*
         * # Key vectors for Wing IDE Professional 11.0.7.1
         * Decompile with IDA as windows pe file.
         * https://wingware.com/pub/wingpro/11.0.7.1/install-root/bin/ide/src/process/__os__/win32/ctlutil.cp312-win_amd64.pyd
         * C:\Program Files\Wing Pro 11\bin\ide\src\process\__os__\win32\ctlutil.cp312-win_amd64.pyd
         * Search for string - %.5X%.5X%.5X%.5X, click on sub. (sub_180001070)
         * View Pseudocode or dump decompiled file to a c file to find the magic numbers
         * 
         * WingIDE versions 1 > 4 use different logic to generate keys and the magic number varies
         * depending on the OS used, these are shown for windows versions so you can
         * see what to ignore when decomipiling with IDA
        */

        //fixed default version magics
        private static readonly Dictionary<string, int[]> DefaultVersionMagics = new Dictionary<string, int[]>
        {
            /*
            { "2.X.X", new int[] { 123, 202, 97, 211 } },
            { "3.X.X", new int[] { 127, 45, 209, 198 } },
            { "4.X.X", new int[] { 240, 4, 47, 98 } },
            */
            { "5.X.X", new int[] { 7, 123, 23, 87 } },
            { "6.X.X", new int[] { 23, 161, 47, 9 } },
            { "7.X.X", new int[] { 221, 13, 93, 27 } },
            { "8.X.X", new int[] { 179, 95, 45, 245 } },
            { "9.X.X", new int[] { 123, 17, 42, 7 } },
            { "10.X.X", new int[] { 102, 99, 107, 117 } },
            { "11.X.X", new int[] { 6, 24, 15, 22 } }, //LABEL_24: v14,v15,v13,v16
            //{ "12.X.X", new int[] { 198, 232, 214, 228 } }, //untested
        };

        //load magic numbers from external file
        private static void LoadMagicNumbers()
        {
            VersionList = new List<string>(DefaultVersionList);
            VersionMagics = new Dictionary<string, int[]>(DefaultVersionMagics);

            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "magicnumbers.txt"
            );

            if (!File.Exists(path))
                return; // use defaults

            // Clear existing combobox data since magicnumbers.txt exists
            VersionList.Clear();
            VersionMagics.Clear();

            foreach (string rawLine in File.ReadAllLines(path))
            {
                string line = rawLine.Trim();

                // Skip empty lines and comments
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split('=');
                if (parts.Length != 2)
                    continue;

                string version = parts[0].Trim();
                string[] nums = parts[1].Split(',');

                if (nums.Length != 4)
                    continue;

                int[] magic = new int[4];
                bool valid = true;

                for (int i = 0; i < 4; i++)
                {
                    if (!int.TryParse(nums[i].Trim(), out magic[i]))
                    {
                        valid = false;
                        break;
                    }
                }

                if (!valid)
                    continue;

                VersionMagics[version] = magic;

                if (!VersionList.Contains(version))
                    VersionList.Add(version);
            }

            // Optional: sort newest first
            VersionList.Sort(CompareVersions);
        }

        //function to recover magic numbers from given licenseID, requestCode and activationCode
        static int[] RecoverMagic(string licenseID, string requestCode, string activationCode)
        {
            // 1. Rebuild lichash
            string lichash;

            using (SHA1 sha1 = SHA1.Create())
            {
                //Add Requestcode + licenceID strings together and convert to a byte array
                //then get the sha1 value of byte array. 
                byte[] data = Encoding.UTF8.GetBytes(requestCode + licenseID);
                byte[] hash = sha1.ComputeHash(data);
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToUpper();

                //MessageBox.Show(hashHex, "Debug HashHex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Remove every second character from the above sha1 value.
                string shaPart = "";
                for (int i = 0; i < hashHex.Length; i += 2)
                    shaPart += hashHex[i];

                //MessageBox.Show(shaPart.ToString(), "Debug HashHex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string base30Part = BaseConvert(
                    shaPart,
                    "0123456789ABCDEF",
                    "123456789ABCDEFGHJKLMNPQRTVWXY"
                );

                //MessageBox.Show(base30Part, "Debug HashHex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                while (base30Part.Length < 17)
                    base30Part = "1" + base30Part;

                lichash = AddHyphens(
                    requestCode.Substring(0, 3) + base30Part
                );
            }

            // 2. Decode activation code → actHex
            string act = activationCode.Replace("-", "");

            if (!act.StartsWith("AXX"))
                throw new Exception("Invalid activation code");

            act = act.Substring(3);

            string actHex = BaseConvert(
                act,
                "123456789ABCDEFGHJKLMNPQRTVWXY",
                "0123456789ABCDEF"
            ).ToLower();

            actHex = actHex.PadLeft(20, '0');

            // 3. Split targets
            int[] targets = new int[4];

            for (int i = 0; i < 4; i++)
            {
                targets[i] = Convert.ToInt32(
                    actHex.Substring(i * 5, 5), 16
                );
            }

            // 4. Brute-force magic values
            int[] magic = new int[4];

            for (int i = 0; i < 4; i++)
            {
                bool found = false;

                for (int ecx = 0; ecx <= 255; ecx++)
                {
                    if (loop(ecx, lichash) == targets[i])
                    {
                        magic[i] = ecx;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    throw new Exception("Magic[" + i + "] not found");
            }

            return magic;
        }

        public MainForm()
        {
            InitializeComponent();
            LoadMagicNumbers(); //load from external file if exists
            InitializeUI();
        }

        private void InitializeUI()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(VersionList.ToArray());
            //comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1; //set last item in the combobox
            /*
             * tN - Trial (10 day)
             * NN - Educational
             * EN - Non Commercial
             * CN - Commercial
             * 2nd char N(pro), L(personal)
             * Excluded letters:IOSUWXZ
             * Excluded digits:0
            */

            //generate initial random license ID on startup
            string randid = "CN" + "123456789ABCDEF"[rng.Next(15)] + RandomString(17, goodchars);
            textBox_id.Text = AddHyphens(randid);
            Clipboard.SetText(textBox_id.Text);
        }

        //function to generate activation code from given licenseID and requestCode
        private static string GenerateActivationCode(string licenseID, string requestCode, string versionStr)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(requestCode + licenseID);
                byte[] hash = sha1.ComputeHash(data);
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToUpper();

                //MessageBox.Show(hashHex, "Debug HashHex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string shaPart = "";
                for (int i = 0; i < hashHex.Length; i += 2)
                    shaPart += hashHex[i];  // even indices (0-based)
                //MessageBox.Show(shaPart, "Debug shaPart", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string base30Part = BaseConvert(shaPart, BASE16, BASE30);
                while (base30Part.Length < 17)
                    base30Part = "1" + base30Part;
                //MessageBox.Show(base30Part, "Debug base30Part", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string lichash = AddHyphens(requestCode.Substring(0, Math.Min(3, requestCode.Length)) + base30Part);
                //MessageBox.Show(lichash, "Debug lichash", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var magic = VersionMagics[versionStr];

                string actHex = "";
                actHex += loop(magic[0], lichash).ToString("x5"); //this is where we can find the magic numbers...
                actHex += loop(magic[1], lichash).ToString("x5");
                actHex += loop(magic[2], lichash).ToString("x5");
                actHex += loop(magic[3], lichash).ToString("x5");
                //MessageBox.Show(actHex, "Debug actHex", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string activationCode = BaseConvert(actHex.ToUpper(), BASE16, BASE30);
                //MessageBox.Show(activationCode, "Debug activationCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                while (activationCode.Length < 17)
                    activationCode = "1" + activationCode;

                activationCode = AddHyphens("AXX" + activationCode);

                return activationCode;
            }
        }

        //function to verify activation code
        static bool VerifyActivationCode(string licenseID, string requestCode, string activationCode, string version)
        {
            string expected = GenerateActivationCode(licenseID, requestCode,version);
            return string.Equals(expected, activationCode, StringComparison.OrdinalIgnoreCase);
        }

        //core loop function used in key generation and recovery
        private static int loop(int ecx, string lichash)
        {
            long part = 0;
            foreach (char c in lichash)
            {
                part = (ecx * part + c) & 0xFFFFF;   // 1048575
            }
            return (int)part;
        }

        //function to generate random string
        private static string RandomString(int size, string chars)
        {
            var sb = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
                sb.Append(chars[random.Next(chars.Length)]);
            return sb.ToString();
        }

        //function to convert number from one base to another
        private static string BaseConvert(string number, string fromDigits, string toDigits)
        {
            if (string.IsNullOrEmpty(number))
                return "";

            BigInteger x = 0;
            foreach (char digit in number)
            {
                int idx = fromDigits.IndexOf(digit);
                if (idx < 0)
                    throw new ArgumentException($"Invalid character '{digit}' in input");
                x = x * fromDigits.Length + idx;
            }

            var result = new StringBuilder();
            if (x == 0)
                return "";  // Match Python: return empty for zero

            while (x > 0)
            {
                int digitValue = (int)(x % toDigits.Length);
                result.Insert(0, toDigits[digitValue]);
                x /= toDigits.Length;
            }

            return result.ToString();
        }

        private static string AddHyphens(string code)
        {
            if (code.Length != 20) return code;
            return $"{code.Substring(0, 5)}-{code.Substring(5, 5)}-{code.Substring(10, 5)}-{code.Substring(15, 5)}";
        }

        private void textBox_id_DoubleClick(object sender, EventArgs e)
        {
            textBox_id.Clear();
            textBox_reqcode.Clear();
            textBox_actcode.Clear();
            string randid = "CN" + "123456789ABCDEF"[rng.Next(15)] + RandomString(17, goodchars);
            textBox_id.Text = AddHyphens(randid);
            Clipboard.SetText(textBox_id.Text);
        }

        private void textBox_reqcode_TextChanged(object sender, EventArgs e)
        {
            String code = textBox_reqcode.Text;
            if (code.Length == 23)
            {
                string version = comboBox1.SelectedItem?.ToString() ?? "";
                string licID = textBox_id.Text.Trim();
                string reqCode = textBox_reqcode.Text.Trim();

                if (string.IsNullOrWhiteSpace(reqCode))
                {
                    MessageBox.Show("Please input the correct Code!", "Woops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!VersionMagics.ContainsKey(version))
                {
                    MessageBox.Show("Unsupported version selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string actCode = GenerateActivationCode(licID, reqCode, version);
                textBox_actcode.Text = actCode;
                //Clipboard.SetText(actCode);
            }

            if (textBox_reqcode.Text.Length > 23)
            {
                int caretPos = textBox_reqcode.SelectionStart;
                textBox_reqcode.Text = textBox_reqcode.Text.Substring(0, 23);
                textBox_reqcode.SelectionStart = Math.Min(caretPos, 23);
            }
        }

        private void textBox_reqcode_DoubleClick(object sender, EventArgs e)
        {
            textBox_reqcode.Clear();
        }

        private void textBox_actcode_DoubleClick(object sender, EventArgs e)
        {
            textBox_actcode.Clear();
        }

        private void textBox_id_TextChanged(object sender, EventArgs e)
        {
            if (textBox_id.Text.Length > 23)
            {
                int caretPos = textBox_id.SelectionStart;
                textBox_id.Text = textBox_id.Text.Substring(0, 23);
                textBox_id.SelectionStart = Math.Min(caretPos, 23);
            }
        }

        private void textBox_actcode_TextChanged(object sender, EventArgs e)
        {
            if (textBox_actcode.TextLength == 23)
            {
                Clipboard.SetText(textBox_actcode.Text);
                //MessageBox.Show("Activation code is copied to your clipboard", "Great", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_actcode.TextLength == 23 && textBox_id.TextLength == 23 && textBox_reqcode.TextLength == 23)
            {
                int[] magic = RecoverMagic(textBox_id.Text.Trim(), textBox_reqcode.Text.Trim(), textBox_actcode.Text.Trim());
                MessageBox.Show("Magic values:\n[" + magic[0] + ", " + magic[1] + ", " + magic[2] + ", " + magic[3] + "]", "Recovered Magic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            string licID = textBox_id.Text.Trim();
            string reqCode = textBox_reqcode.Text.Trim();
            string actCode = textBox_actcode.Text.Trim();
            string version = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(version))
            {
                MessageBox.Show("Select a version.");
                return;
            }

            if (actCode.Length == 23)
            {
                bool valid = VerifyActivationCode(
                licID,
                reqCode,
                actCode,
                version
                );

                MessageBox.Show(
                    valid ? "Activation code is VALID"
                          : "Activation code is INVALID",
                    "Verification Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private DateTime _lastClickTime;
        private readonly TimeSpan _doubleClickThreshold = TimeSpan.FromMilliseconds(500);
        private CancellationTokenSource _singleClickCts;

        private async void button_url_Click(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            // Cancel any pending single-click action
            _singleClickCts?.Cancel();
            _singleClickCts?.Dispose();

            if ((now - _lastClickTime) <= _doubleClickThreshold)
            {
                // Double click detected!
                HandleDoubleClick();
                _lastClickTime = DateTime.MinValue;
                _singleClickCts = null;
            }
            else
            {
                // First click - record time
                _lastClickTime = now;

                // Create new cancellation token
                _singleClickCts = new CancellationTokenSource();

                try
                {
                    // Wait for potential double-click
                    await Task.Delay(_doubleClickThreshold.Milliseconds + 50,
                                    _singleClickCts.Token);

                    // If we get here without cancellation, it's a single click
                    if (_lastClickTime != DateTime.MinValue)
                    {
                        HandleSingleClick();
                    }
                }
                catch (TaskCanceledException)
                {
                    // Double-click occurred, single-click was cancelled
                    // Do nothing - double-click already handled
                }
            }
        }

        private void HandleDoubleClick()
        {
            MessageBox.Show(
                "1: Decompile the file below with IDA as a Windows PE file:\r\n\n" +
                "WingPro11\\bin\\ide\\src\\process\\__os__\\win32\\ctlutil.cp312-win_amd64.pyd\r\n\n" +
                "2: Search the decompiled file for string - %.5X%.5X%.5X%.5X, then click on the function\r\n\n" +
                "3: View the function in Pseudocode or dump the decompiled file to a c file then check it to find the magic numbers\r\n\n" +
                "4: Piracy is a crime, this keygen code is only for educational purposes",
                "How to find magic numbers in Wing 11.0.7.1",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void HandleSingleClick()
        {
            string url = "https://wingware.com/pub/wingpro/";

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open URL:\n" + ex.Message);
            }
        }
    }
}