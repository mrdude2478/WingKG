using System;

namespace WingIDEKeygen
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label_version = new System.Windows.Forms.Label();
            this.label_ID = new System.Windows.Forms.Label();
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.label_request = new System.Windows.Forms.Label();
            this.textBox_reqcode = new System.Windows.Forms.TextBox();
            this.label_actcode = new System.Windows.Forms.Label();
            this.textBox_actcode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonVerify = new System.Windows.Forms.Button();
            this.button_url = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(100, 9);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(172, 21);
            this.comboBox1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.comboBox1, "Press Enter on here to open magicnumbers");
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox1_KeyPress);
            // 
            // label_version
            // 
            this.label_version.AutoSize = true;
            this.label_version.Location = new System.Drawing.Point(6, 12);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(92, 13);
            this.label_version.TabIndex = 1;
            this.label_version.Text = "Wing Pro Version:";
            // 
            // label_ID
            // 
            this.label_ID.AutoSize = true;
            this.label_ID.Location = new System.Drawing.Point(6, 40);
            this.label_ID.Name = "label_ID";
            this.label_ID.Size = new System.Drawing.Size(61, 13);
            this.label_ID.TabIndex = 2;
            this.label_ID.Text = "License ID:";
            // 
            // textBox_id
            // 
            this.textBox_id.Location = new System.Drawing.Point(100, 37);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(172, 20);
            this.textBox_id.TabIndex = 3;
            this.textBox_id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.textBox_id, "Double click to generate a new value");
            this.textBox_id.TextChanged += new System.EventHandler(this.textBox_id_TextChanged);
            this.textBox_id.DoubleClick += new System.EventHandler(this.textBox_id_DoubleClick);
            // 
            // label_request
            // 
            this.label_request.AutoSize = true;
            this.label_request.Location = new System.Drawing.Point(6, 67);
            this.label_request.Name = "label_request";
            this.label_request.Size = new System.Drawing.Size(78, 13);
            this.label_request.TabIndex = 4;
            this.label_request.Text = "Request Code:";
            // 
            // textBox_reqcode
            // 
            this.textBox_reqcode.Location = new System.Drawing.Point(100, 64);
            this.textBox_reqcode.Name = "textBox_reqcode";
            this.textBox_reqcode.Size = new System.Drawing.Size(172, 20);
            this.textBox_reqcode.TabIndex = 5;
            this.textBox_reqcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.textBox_reqcode, "Paste the request key given by Wing IDE into here and an\r\nactivation key will be " +
        "automatically generated and pasted into\r\nyour clipboard");
            this.textBox_reqcode.TextChanged += new System.EventHandler(this.textBox_reqcode_TextChanged);
            this.textBox_reqcode.DoubleClick += new System.EventHandler(this.textBox_reqcode_DoubleClick);
            // 
            // label_actcode
            // 
            this.label_actcode.AutoSize = true;
            this.label_actcode.Location = new System.Drawing.Point(6, 94);
            this.label_actcode.Name = "label_actcode";
            this.label_actcode.Size = new System.Drawing.Size(85, 13);
            this.label_actcode.TabIndex = 6;
            this.label_actcode.Text = "Activation Code:";
            // 
            // textBox_actcode
            // 
            this.textBox_actcode.Location = new System.Drawing.Point(100, 91);
            this.textBox_actcode.Name = "textBox_actcode";
            this.textBox_actcode.Size = new System.Drawing.Size(172, 20);
            this.textBox_actcode.TabIndex = 7;
            this.textBox_actcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.textBox_actcode, "This will be populated automatically and the code will be\r\ncopied to your clipboa" +
        "rd when you enter a request code");
            this.textBox_actcode.TextChanged += new System.EventHandler(this.textBox_actcode_TextChanged);
            this.textBox_actcode.DoubleClick += new System.EventHandler(this.textBox_actcode_DoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(100, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Magic Values";
            this.toolTip1.SetToolTip(this.button1, "Show the magic numbers used for the activation code");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonVerify
            // 
            this.buttonVerify.Location = new System.Drawing.Point(9, 118);
            this.buttonVerify.Name = "buttonVerify";
            this.buttonVerify.Size = new System.Drawing.Size(82, 23);
            this.buttonVerify.TabIndex = 9;
            this.buttonVerify.Text = "Verify Code";
            this.toolTip1.SetToolTip(this.buttonVerify, "Validate the activation code against a Wing version");
            this.buttonVerify.UseVisualStyleBackColor = true;
            this.buttonVerify.Click += new System.EventHandler(this.buttonVerify_Click);
            // 
            // button_url
            // 
            this.button_url.Location = new System.Drawing.Point(190, 118);
            this.button_url.Name = "button_url";
            this.button_url.Size = new System.Drawing.Size(82, 23);
            this.button_url.TabIndex = 10;
            this.button_url.Text = "WingIDE";
            this.toolTip1.SetToolTip(this.button_url, "Click once to open url\r\nClick twice for a hacking hint");
            this.button_url.UseVisualStyleBackColor = true;
            this.button_url.Click += new System.EventHandler(this.button_url_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 150);
            this.Controls.Add(this.button_url);
            this.Controls.Add(this.buttonVerify);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_actcode);
            this.Controls.Add(this.label_actcode);
            this.Controls.Add(this.textBox_reqcode);
            this.Controls.Add(this.label_request);
            this.Controls.Add(this.textBox_id);
            this.Controls.Add(this.label_ID);
            this.Controls.Add(this.label_version);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wing IDE Keygen By MrDude";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.Label label_ID;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.Label label_request;
        private System.Windows.Forms.TextBox textBox_reqcode;
        private System.Windows.Forms.Label label_actcode;
        private System.Windows.Forms.TextBox textBox_actcode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonVerify;
        private System.Windows.Forms.Button button_url;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

