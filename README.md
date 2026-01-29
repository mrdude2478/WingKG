# Wing IDE Keygen Algorithm – Technical Breakdown

> **Author:** MrDude  
> **Purpose:** Technical documentation of the key/activation algorithm  
> **Note:** This README is a formatted version of the original guide.

---

Keygen Steps guide by MrDude
============================

Step 1: Input Preparation
What happens: Combine the Request Code and License ID (keeping the hyphens)

JavaScript
-------------------------------------------------------------------
const inputString = requestCodeRaw + licenseIdRaw;
-------------------------------------------------------------------

Example: "RWB2F-HW2G7-794W5-P3AB5" + "CN123-45678-9ABCD-EFGHI"
Result: "RWB2F-HW2G7-794W5-P3AB5CN123-45678-9ABCD-EFGHI"

Why: Wing IDE's internal hash function includes the hyphen characters as part of the input. Removing them would generate a completely different SHA1 hash.
Key detail: The raw values with hyphens are used, not the cleaned 20-character versions.
________________________________________

Step 2: SHA1 Hashing
What happens: Generate a SHA1 cryptographic hash of the combined string

JavaScript
-------------------------------------------------------------------
const encoder = new TextEncoder();
const data = encoder.encode(inputString);  // Convert to UTF-8 bytes
const hashBuffer = await crypto.subtle.digest('SHA-1', data);
-------------------------------------------------------------------

Example:
Copy
Input:  "RWB2F-HW2G7-794W5-P3AB5CN123-45678-9ABCD-EFGHI"
52 57 42 32 46 2D 48 57 32 47 37 2D 37 39 34 57 35 2D 50 33 41 42 35 43 4E 31 32 33 2D 34 35 36 37 38 2D 39 41 42 43 44 2D 45 46 47 48 49

Output: Array of 20 bytes (160 bits)
Example SHA 1 of Array of bytes: A2DC63D6625A491842D8D849B314471ECDDA90F9

Why: SHA1 produces a deterministic "fingerprint" of the input. The same inputs always produce the same 20-byte output, but you can't reverse-engineer the inputs from the output.
________________________________________

Step 3: Convert to Hex String

What happens: Turn the 20 binary bytes into a 40-character hexadecimal string
JavaScript
-------------------------------------------------------------------
let hashHex = "";
for (let i = 0; i < hashArray.length; i++) {
    hashHex += hashArray[i].toString(16).padStart(2, '0').toUpperCase();
}
-------------------------------------------------------------------
// Example: "A2 DC 63 D6 62 5A 49 18 42 D8 D8 49 B3 14 47 1E CD DA 90 F9”
Breakdown:
•	Each byte (0-255) becomes 2 hex characters
•	20 bytes × 2 = 40 characters
•	padStart(2, '0') ensures single-digit values like 0x7 become "07" not just "7"

Example:
String:   "A2" + "DC" + "63" + "D6" + "62"...
________________________________________

Step 4: Extract Every Second Character

What happens: Take characters at even indices (0, 2, 4, 6...) from the 40-char hex string

JavaScript
-------------------------------------------------------------------
let shaPart = "";
for (let i = 0; i < hashHex.length; i += 2) {
    shaPart += hashHex[i];
}
-------------------------------------------------------------------
// 40 chars → 20 chars
Example:
Full String:   "A2 DC 63 D6 62 5A 49 18 42 D8 D8 49 B3 14 47 1E CD DA 90 F9” (remove spaces)
Indices:    0 1 2 3 4 5 6 7 8 9 10 11 12 13
Selection:  ^   ^   ^   ^   ^    ^    ^
Result:     A   D   6   D   6    5    4 ...
Final:      "AD6D65414DD4B141CD9F"

Why: This compresses the 40-char hex to 20 characters while maintaining entropy. It uses a subset of the hash data that will be converted to the license format.
________________________________________

Step 5: Base16 → Base30 Conversion

What happens: Convert the 20-character hex string to a shorter base-30 representation

JavaScript
-------------------------------------------------------------------
let base30Part = baseConvert(shaPart, BASE16, BASE30);
-------------------------------------------------------------------

Here's how the baseConvert function works in Step 5, breaking it down with a concrete example:

The Two-Phase Conversion Process
The function performs conversion in two phases:
1.	Phase 1: Convert input string → BigInt (treating input as base-16)
2.	Phase 2: Convert BigInt → output string (encoding as base-30)

Character Sets Used
const BASE16 = "0123456789ABCDEF";              // 16 chars (hexadecimal)
const BASE30 = "123456789ABCDEFGHJKLMNPQRTVWXY"; // 30 chars (no 0,I,O,S,U,Z to avoid confusion)

Why Base30? It omits confusing characters (0 vs O, I vs 1, S vs 5, etc.) to prevent transcription errors in license keys.

Step-by-Step Example
Let's say shaPart = "AD6" (AD6D65414DD4B141CD9F)
Phase 1: Base16 → BigInt
The function interprets "AD6" as a hexadecimal number and converts it to a BigInt:
Copy
Initial: x = 0

Char 'A': index 10 in BASE16
  x = 0 × 16 + 10 = 10

Char 'D': index 13 in BASE16  
  x = 10 × 16 + 13 = 173

Char '6': index 6 in BASE16
  x = 173 × 16 + 6 = 2774

Result: x = 2774 (as BigInt)
    
Full:
x = 0
x = 0×16 + 10 = 10
x = 10×16 + 13 = 173
x = 173×16 + 6 = 2,774
x = 2,774×16 + 13 = 44,397
x = 44,397×16 + 6 = 710,358
x = 710,358×16 + 5 = 11,365,733
x = 11,365,733×16 + 4 = 181,851,732
x = 181,851,732×16 + 1 = 2,909,627,713
x = 2,909,627,713×16 + 4 = 46,554,043,412
x = 46,554,043,412×16 + 13 = 744,864,694,605
x = 744,864,694,605×16 + 13 = 11,917,835,113,693
x = 11,917,835,113,693×16 + 4 = 190,685,361,819,092
x = ... (continuing this pattern) ...
x = 818,987,392,838,930,182,294,943  ← Final BigInt value

Phase 2: BigInt → Base30
Now we divide this massive number by 30 repeatedly, using remainders to pick characters.
Base30 alphabet: 123456789ABCDEFGHJKLMNPQRTVWXY
Index reference:
0=1, 1=2, 2=3, ..., 8=9, 9=A, 10=B, 14=F, 15=G, 18=K, 23=Q, 27=W, 29=Y

Division sequence:
818987392838930182294943 ÷ 30 = 27299579761297672743164 remainder 23 → Q
27299579761297672743164 ÷ 30 = 909985992043255758105 remainder 14 → F
909985992043255758105 ÷ 30 = 30332866401441858603 remainder 15 → G
30332866401441858603 ÷ 30 = 1011095546714728620 remainder 3 → 4
1011095546714728620 ÷ 30 = 33703184890490954 remainder 0 → 1
33703184890490954 ÷ 30 = 1123439496349698 remainder 14 → F
1123439496349698 ÷ 30 = 37447983211656 remainder 18 → K
37447983211656 ÷ 30 = 1248266107055 remainder 6 → 7
1248266107055 ÷ 30 = 41608870235 remainder 5 → 6
41608870235 ÷ 30 = 1386962341 remainder 5 → 6
1386962341 ÷ 30 = 46232078 remainder 1 → 2
46232078 ÷ 30 = 1541069 remainder 8 → 9
1541069 ÷ 30 = 51368 remainder 29 → Y
51368 ÷ 30 = 1712 remainder 8 → 9
1712 ÷ 30 = 57 remainder 2 → 3
57 ÷ 30 = 1 remainder 27 → W
1 ÷ 30 = 0 remainder 1 → 2

Important: We build the result backwards (from last remainder to first):
Final Result = "2W39Y92667KF14GFQ"

Why is it exactly 17 characters?
Because (sha1 value) AD6D65414DD4B141CD9F represents a 80-bit value (20 hex chars × 4 bits).
Base30 encodes ~4.9 bits per character (log₂30 ≈ 4.907).
80 bits ÷ 4.907 bits/char ≈ 16.3 characters, so we need 17 characters to represent it without loss.
The specific value 818987392838930182294943 is large enough to require 17 base30 digits but not 18.

________________________________________

Step 6: Pad with '1's

What happens: Ensure the base30 string is exactly 17 characters long

JavaScript
-------------------------------------------------------------------
while (base30Part.length < 17) {
    base30Part = "1" + base30Part;
}
-------------------------------------------------------------------
Example:
Before: "ABC123"
After:  "1111111111111ABC123" (17 chars total)
Why: Fixed-width encoding makes parsing predictable. '1' is used as padding because it's the smallest value in BASE30 (index 0), ensuring the numeric value doesn't change significantly.
________________________________________

Step 7: Create the LicHash

What happens: Combine the first 3 characters of the Request Code with the 17-char base30 string, then add hyphens

JavaScript
-------------------------------------------------------------------
const lichash = addHyphens(requestCodeRaw.substring(0, 3) + base30Part);
-------------------------------------------------------------------

// Example: "RWB" + "2W39Y92667KF14GFQ" → "RWB2W39Y92667KF14GFQ"
// Then hyphens: "RWB2W-39Y92-667KF-14GFQ"

•	Request code prefix: First 3 chars
•	Base30 part: 17 chars
•	Total: 20 chars → XXXXX-XXXXX-XXXXX-XXXXX format

Example:
Prefix:    "RWB"
Combined:  "RWB2W39Y92667KF14GFQ" (20 chars)
Hyphenated: "RWB2W-39Y92-667KF-14GFQ"
This 20-character lichash is the intermediate license hash used for the final activation calculation.
________________________________________

Step 8: Load Magic Numbers

What happens: Retrieve the version-specific constants

JavaScript
-------------------------------------------------------------------
const magic = magics[version];
-------------------------------------------------------------------
// Example for 11.X.X: [6, 24, 15, 22]

What are these? These are 4 hardcoded integers found in Wing IDE's compiled binary (ctlutil.cpython-*.so). Each major version has different values to prevent keys from working across versions.
________________________________________

Step 9: The Loop Function (Core Security)

What happens: Calculate 4 separate 5-digit hex values using a custom hashing algorithm

JavaScript
-------------------------------------------------------------------
function loop(ecx, lichash) {
    let part = 0;
    for (let i = 0; i < lichash.length; i++) {
        part = (ecx * part + lichash.charCodeAt(i)) & 0xFFFFF;
    }
    return part;
}

// Calculate 4 parts
let actHex = "";
actHex += loop(magic[0], lichash).toString(16).padStart(5, '0');
actHex += loop(magic[1], lichash).toString(16).padStart(5, '0');
actHex += loop(magic[2], lichash).toString(16).padStart(5, '0');
actHex += loop(magic[3], lichash).toString(16).padStart(5, '0');
-------------------------------------------------------------------
// Result: 20 hex characters (5 chars × 4 parts)

How loop works (detailed):

This is a rolling hash function similar to Rabin-Karp string hashing.

Initialize: part = 0
For each char in lichash:
    part = (magic_number × part + ASCII_value) mod 2^20
    
& 0xFFFFF keeps only the lower 20 bits (ensures result is 0-1048575)

Concrete Example:

lichash = "RWB2W-39Y92-667KF-14GFQ" (without hyphens: "RWB2W39Y92667KF14GFQ")
magic[0] = 6

Char 'R' (ASCII 82):
  part = (6 × 0 + 82) & 0xFFFFF = 82

Char 'W' (ASCII 87):
  part = (6 × 82 + 87) & 0xFFFFF = 579

Char 'B' (ASCII 66):
  part = (6 × 579 + 66) & 0xFFFFF = 3540
  
Char '2' (ASCII 50):
  part = (6 × 3540 + 50) & 0xFFFFF= 21290

Char 'W' (ASCII 87):
  part = (6 × 21290 + 87) & 0xFFFFF = 127827
  
Char '-' (ASCII 45):
  part = (6 × 127827 + 45) & 0xFFFFF = 767007

Char '3' (ASCII 51):
  part = (6 × 767007 + 51) = 4602093 → & 0xFFFFF = 407789

Char '9' (ASCII 57):
  part = (6 × 407789 + 57) = 2446791 → & 0xFFFFF = 349639

Char 'Y' (ASCII 89):
  part = (6 × 349639 + 89) = 2097923 → & 0xFFFFF = 771
  
Char '9' (ASCII 57):
  part = (6× 771 + 57) = 4683

Char '2' (ASCII 50):
  part = (6 × 4683 + 50) = 28148
  
Char '-' (ASCII 45):
  part = (6 × 28148 + 45) = 168933
  
Char '6' (ASCII 54):
  part = (6 × 168933 + 54) = 1013652

Char '6' (ASCII 54):
  part = (6 × 1013652 + 54) = 6081966 → & 0xFFFFF = 839086

Char '7' (ASCII 55):
  part = (6 × 839086 + 55) = 5034571 → & 0xFFFFF = 840267
  
Char 'K' (ASCII 75):
  part = (6 × 840267 + 75) = 5041677 → & 0xFFFFF = 847373

Char 'F' (ASCII 70):
  part = (6 × 847373 + 70) = 5084308 → & 0xFFFFF = 890004
  
Char '-' (ASCII 45):
  part = (6 × 890004 + 45) = 5340069 → & 0xFFFFF = 97189
  
Char '1' (ASCII 49):
  part = (6 × 97189 + 49) = 583183

Char '4' (ASCII 52):
  part = (6 × 583183 + 52) = 3499150 → & 0xFFFFF = 353422

Char 'G' (ASCII 71):
  part = (6 × 353422 + 71) = 2120603 → & 0xFFFFF = 23451
  
Char 'F' (ASCII 70):
  part = (6 × 23451 + 70) = 140776

Char 'Q' (ASCII 81):
  part = (6 × 140776 + 81) = 844737


Final value on first run with magic number (6): 0xCE3C1 (844737)
Magic numbers or 11.X.X: [6, 24, 15, 22]

6  = 0xCE3C1
24 = 0x72EA1
15 = 0xA91F8
22 = 0x7C661

0 - ce3c1
1 - ce3c172ea1
2 - ce3c172ea1a91f8
3 - ce3c172ea1a91f87c661 (final string result on 4rth loop)

Why 4 different magic numbers? Each magic number produces a different segment of the activation code. All 4 must be correct for the license to validate.

The & 0xFFFFF bitmask: Ensures the result never exceeds 1,048,575 (0xFFFFF), guaranteeing a 5-digit hex number (max FFFFF).

The 0xFFFFF bitmask just takes the last 20 bits of number cnverted to binary:
example:
4,602,135 = 10001100011100100010111
Last 20 bits = 01100011100100010111 (decimal 407,831)
________________________________________

Step 10: Convert Hex to Base30 (Again)

What happens: Convert the 20-char hex string from Step 9 into base30
JavaScript
-------------------------------------------------------------------
let activationCode = baseConvert(actHex, BASE16, BASE30);
-------------------------------------------------------------------

Step 10: Base16 → Base30 Conversion

Phase 1: Hex String → BigInt
The function interprets "CE3C172EA1A91F87C661" as a base-16 number and converts it to a decimal BigInt:

CE3C172EA1A91F87C661 (hex) 
= 12×16^19 + 14×16^18 + 3×16^17 + 12×16^16 + 1×16^15 + 7×16^14 + 2×16^13 + 14×16^12 + 10×16^11 + 1×16^10 + 10×16^9 + 9×16^8 + 1×16^7 + 15×16^6 + 8×16^5 + 7×16^4 + 12×16^3 + 6×16^2 + 6×16^1 + 1×16^0

= 973,915,970,565,829,038,687,841 (decimal)

This is a 24-digit decimal number.


Phase 2: BigInt → Base30 (Repeated Division)

Now we divide 973915970565829038687841 by 30 repeatedly. Each remainder (0-29) maps to an index in the Base30 alphabet.

Base30 Alphabet: 123456789ABCDEFGHJKLMNPQRTVWXY
Table
Division Step	Quotient	Remainder	Index Maps To	Char
973915970565829038687841 ÷ 30	32,463,865,685,527,634,622,928	1	BASE30[1]	2
32,463,865,685,527,634,622,928 ÷ 30	1,082,128,856,184,254,487,430	28	BASE30[28]	X
1,082,128,856,184,254,487,430 ÷ 30	36,070,961,872,808,482,914	10	BASE30[10]	B
36,070,961,872,808,482,914 ÷ 30	1,202,365,395,760,282,763	24	BASE30[24]	R
1,202,365,395,760,282,763 ÷ 30	40,078,846,525,342,758	23	BASE30[23]	Q
40,078,846,525,342,758 ÷ 30	1,335,961,550,844,758	18	BASE30[18]	K
1,335,961,550,844,758 ÷ 30	44,532,051,694,825	8	BASE30[8]	9
44,532,051,694,825 ÷ 30	1,484,401,723,160	25	BASE30[25]	T
1,484,401,723,160 ÷ 30	49,480,057,438	20	BASE30[20]	M
49,480,057,438 ÷ 30	1,649,335,247	28	BASE30[28]	X
1,649,335,247 ÷ 30	54,977,841	17	BASE30[17]	J
54,977,841 ÷ 30	1,832,594	21	BASE30[21]	N
1,832,594 ÷ 30	61,086	14	BASE30[14]	F
61,086 ÷ 30	2,036	6	BASE30[6]	7
2,036 ÷ 30	67	26	BASE30[26]	V
67 ÷ 30	2	7	BASE30[7]	8
2 ÷ 30	0	2	BASE30[2]	3

Building the Result

The algorithm prepends each character (adds to the front), so we read the remainders from bottom to top: 3 8 V 7 F N J X M T 9 K Q R B X 2

Wait—let's check that. Reading from the last division (step 17) to the first (step 1):
Table
Order	Remainder	Char
17 → 1	2	3
16 → 2	7	8
15 → 3	26	V
14 → 4	6	7
13 → 5	14	F
12 → 6	21	N
11 → 7	17	J
10 → 8	28	X
9 → 9	20	M
8 → 10	25	T
7 → 11	8	9
6 → 12	18	K
5 → 13	23	Q
4 → 14	24	R
3 → 15	10	B
2 → 16	28	X
1 → 17	1	2
Final Result: "38V7FNJXMT9KQRBX2" (17 characters)

Why 17 Characters?
The input CE3C172EA1A91F87C661 represents approximately 9.74 × 10²³.
30^16 = 4.3 × 10²³ (too small)
30^17 = 1.3 × 10²⁵ (large enough)
Since 9.74 × 10²³ is greater than 30^16 but less than 30^17, it requires exactly 17 digits in base30.
________________________________________

Step 11: Pad to 17 Characters

(Since length in the previous step is already 17, the loop does not execute—no padding is added).

What happens: Ensure the activation code is exactly 17 characters, padding with '1' at the front

JavaScript
-------------------------------------------------------------------
while (activationCode.length < 17) {
    activationCode = "1" + activationCode;
}
-------------------------------------------------------------------
Result: Always 17 characters of base30 alphabet.
________________________________________

Step 12: Final Assembly

What happens: Add the "AXX" prefix and hyphenate into the final format

JavaScript
-------------------------------------------------------------------
activationCode = addHyphens("AXX" + activationCode);
-------------------------------------------------------------------

Breakdown:
1.	Prefix: "AXX" (indicates this is an activation code, not a license ID)
2.	Body: 17-char base30 from Step 11
3.	Combined: 3 + 17 = 20 characters
4.	Hyphenated: XXXXX-XXXXX-XXXXX-XXXXX

Final Example:
Before: "AXX38V7FNJXMT9KQRBX2"
After:  "AXX38-V7FNJ-XMT9K-QRBX2"
________________________________________

Complete Flow Summary

1. INPUTS
   License ID: CN123-45678-9ABCD-EFGHI
   Request:    RWB2F-HW2G7-794W5-P3AB5
   Version:    11.X.X → magic = [6, 24, 15, 22]

2. COMBINE → "RWB2F-HW2G7-794W5-P3AB5CN123-45678-9ABCD-EFGHI"

3. SHA1 HASH → 20 bytes → hex "A2DC63D6625A491842D8D849B314471ECDDA90F9"

4. EXTRACT EVERY 2nd CHAR → "AD6D65414DD4B141CD9F"

5. BASE16→BASE30 → "2W39Y92667KF14GFQ"

6. PAD TO 17 → "2W39Y92667KF14GFQ"

7. LICHASH → "RWB" + "2W39Y92667KF14GFQ" = "RWB2W39Y92667KF14GFQ"

8. LOOP FUNCTION (×4)
   loop(6, lichash)   → 0xce3c1 → "ce3c1"
   loop(24, lichash)  → 0x72ea1 → "72ea1"
   loop(15, lichash)  → 0xa91f8 → "a91f8"
   loop(22, lichash)  → 0x7c661 → "7c661"
   
   actHex = "ce3c172ea1a91f87c661"

9. BASE16→BASE30 → "38V7FNJXMT9KQRBX2"

10. PAD TO 17 → "38V7FNJXMT9KQRBX2"

11. FINAL ASSEMBLY
    "AXX" + "38V7FNJXMT9KQRBX2" = "AXX38V7FNJXMT9KQRBX2"
    
12. ADD HYPHENS → "AXX38-V7FNJ-XMT9K-QRBX2"

Why this works: Wing IDE contains the same algorithm and magic numbers. When you enter the activation code, it performs the same calculations on the License ID and Request Code. If your activation code matches its internal calculation, the license validates.

The security relies on:
•	Secrecy of magic numbers (hardcoded in the binary)
•	Irreversibility of SHA1 (you can't derive the magic numbers from valid keys)
•	Obfuscation (the loop function is simple but the magic numbers make it hard to forge)
