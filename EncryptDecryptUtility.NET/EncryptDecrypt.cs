using System;
using System.Globalization;
using Microsoft.VisualBasic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace EncryptDecryptUtility.NET
{
    #region PHP Encrypter & Decrypter
    public class EncryptDecryptPHPNET3DES
    {
        public string EncryptString(string Message)
        {
            byte[] Results;
            string Passphrase = "12345678abcdefgh";
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.CBC;
            TDESAlgorithm.Padding = PaddingMode.Zeros;
            TDESAlgorithm.IV = Convert.FromBase64String("5Fl6lc4xjwA=");
            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public string DecryptString(string Message)
        {
            string Passphrase = "12345678abcdefgh";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.CBC;
            TDESAlgorithm.Padding = PaddingMode.Zeros;
            TDESAlgorithm.IV = Convert.FromBase64String("5Fl6lc4xjwA=");
            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

    }
    #endregion

    #region Custom Encrypter & Decrypter
    public class EncryptDecrypt
    {
        // 3/13/98 Terrence : Add the NewDecryptStr
        // Port from C
        private String CRYPT_TITLE = new String("!CRYPT!".ToCharArray());
        private String PSEUDO_USER_NAME = new String("Virus3761267Trend".ToCharArray());
        private String PSEUDO_SERVER_NAME = new String("Windows7621673NT".ToCharArray());

        // for test only
        private String sOriginalStr = new String("!CRYPT!@7F957AD082FBADD72F2478791030D07C72C0B0289534767E168C19411AFD283DEEA745CF58D1EBD7924787910830D07C72C0B0289534767E168C19411AFD283DEEA745CF58D1EBD7924787910830D07C72C0B0289534767E168C19411AFD283DEEA745CF58D1EBD7924787910830D07C72C0B0289534767E18E01B8DAB95CEDCE78514501".ToCharArray());
        private String sDecryptStr = new String("".ToCharArray());

        // some constants
        private static byte[] pc_2 = {
    13, 16, 10, 23,  0,  4,
    2, 27, 14,  5, 20,  9,
    22, 18, 11,  3, 25,  7,
    15,  6, 26, 19, 12,  1,
    40, 51, 30, 36, 46, 54,
    29, 39, 50, 44, 32, 47,
    43, 48, 38, 55, 33, 52,
    45, 41, 49, 35, 28, 31,
    };

        private static byte[,] ip = {
    {7,0x02}, {6,0x02}, {5,0x02}, {4,0x02}, {3,0x02}, {2,0x02}, {1,0x02}, {0,0x02},
    {7,0x08}, {6,0x08}, {5,0x08}, {4,0x08}, {3,0x08}, {2,0x08}, {1,0x08}, {0,0x08},
    {7,0x20}, {6,0x20}, {5,0x20}, {4,0x20}, {3,0x20}, {2,0x20}, {1,0x20}, {0,0x20},
    {7, (byte) 0x80}, {6, (byte) 0x80}, {5, (byte) 0x80}, {4, (byte) 0x80}, {3, (byte) 0x80}, {2, (byte) 0x80}, {1, (byte) 0x80}, {0, (byte) 0x80},
    {7,0x01}, {6,0x01}, {5,0x01}, {4,0x01}, {3,0x01}, {2,0x01}, {1,0x01}, {0,0x01},
    {7,0x04}, {6,0x04}, {5,0x04}, {4,0x04}, {3,0x04}, {2,0x04}, {1,0x04}, {0,0x04},
    {7,0x10}, {6,0x10}, {5,0x10}, {4,0x10}, {3,0x10}, {2,0x10}, {1,0x10}, {0,0x10},
    {7,0x40}, {6,0x40}, {5,0x40}, {4,0x40}, {3,0x40}, {2,0x40}, {1,0x40}, {0,0x40},
    };

        private static byte[] ls = {
    1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1,
    };

        private static byte[] e = {
    31,  0,  1,  2,  3,  4,
    3,  4,  5,  6,  7,  8,
    7,  8,  9, 10, 11, 12,
    11, 12, 13, 14, 15, 16,
    15, 16, 17, 18, 19, 20,
    19, 20, 21, 22, 23, 24,
    23, 24, 25, 26, 27, 28,
    27, 28, 29, 30, 31,  0,
    };

        /*  The Selection functions (S-boxes) */
        private static byte[,] s = {
    { 14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
    0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
    4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
    15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13 },

    {15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
    3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
    0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
    13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9},

    {10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
    13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
    13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
    1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12},

    {7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
    13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
    10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
    3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14},

    {2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
    14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
    4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
    11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3},

    {12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
    10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
    9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
    4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13},

    {4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
    13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
    1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
    6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12},

    {13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
    1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
    7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
    2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11}
    };

        private static byte[] p = {
    15,  6, 19, 20,
    28, 11, 27, 16,
    0, 14, 22, 25,
    4, 17, 30,  9,
    1,  7, 23, 13,
    31, 26,  2,  8,
    18, 12, 29,  5,
    21, 10,  3, 24,
    };

        private static char[] fp = {
    (char)39,  (char)7, (char)47, (char)15, (char)55, (char)23, (char)63, (char)31,
    (char)38,  (char)6, (char)46, (char)14, (char)54, (char)22, (char)62, (char)30,
    (char)37,  (char)5, (char)45, (char)13, (char)53, (char)21, (char)61, (char)29,
    (char)36,  (char)4, (char)44, (char)12, (char)52, (char)20, (char)60, (char)28,
    (char)35,  (char)3, (char)43, (char)11, (char)51, (char)19, (char)59, (char)27,
    (char)34,  (char)2, (char)42, (char)10, (char)50, (char)18, (char)58, (char)26,
    (char)33,  (char)1, (char)41,  (char)9, (char)49, (char)17, (char)57, (char)25,
    (char)32,  (char)0, (char)40,  (char)8, (char)48, (char)16, (char)56, (char)24,
    };

        private static byte[,] pc_1 = {
    {7,0x01}, {6,0x01}, {5,0x01}, {4,0x01}, {3,0x01}, {2,0x01}, {1,0x01},
    {0,0x01}, {7,0x02}, {6,0x02}, {5,0x02}, {4,0x02}, {3,0x02}, {2,0x02},
    {1,0x02}, {0,0x02}, {7,0x04}, {6,0x04}, {5,0x04}, {4,0x04}, {3,0x04},
    {2,0x04}, {1,0x04}, {0,0x04}, {7,0x08}, {6,0x08}, {5,0x08}, {4,0x08},
    {7,0x40}, {6,0x40}, {5,0x40}, {4,0x40}, {3,0x40}, {2,0x40}, {1,0x40},
    {0,0x40}, {7,0x20}, {6,0x20}, {5,0x20}, {4,0x20}, {3,0x20}, {2,0x20},
    {1,0x20}, {0,0x20}, {7,0x10}, {6,0x10}, {5,0x10}, {4,0x10}, {3,0x10},
    {2,0x10}, {1,0x10}, {0,0x10}, {3,0x08}, {2,0x08}, {1,0x08}, {0,0x08},
    };


        private static byte[,] ks = new byte[16, 48];
        //{
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        //    };
        /* key schedule storage space */
        public EncryptDecrypt()
        {


        }
        private void des_key(byte[] key)
        {
            byte[] k = new byte[56];
            short i, j, shift, s, d;

            /*  Perform initial key permutation K0 = PC_1[key] */
            for (i = 0; i < 56; i++)
                k[i] = ((key[pc_1[i, 0]] & pc_1[i, 1]) != 0) ? (byte)1 : (byte)0;

            /*  Now calculate key schedule */
            for (i = shift = 0; i < 16; i++)
            {
                shift += ls[i];
                for (j = 0; j < 48; j++)
                {
                    s = pc_2[j];
                    if (s >= 28)
                    {              /* if s >= 28, bit is in D section */
                        d = 28;
                        s -= 28;
                    }
                    else
                        d = 0;                  /* else s is in the C section */
                    s += shift;                 /* Left shift same as addition mod 28 */
                    if (s >= 28)
                        s -= 28;
                    s += d;
                    ks[i, j] = k[s];
                }
            }
        }

        private void _cvtsh(byte[] data, char[] line, int len)
        {
            int a, b, c, num;
            int q;

            q = len * 2 - 1;
            // q = &line[len*2] - 1;

            for (a = len; a > 0; a--)
            {
                for (b = 0, num = 0; b < 2 && q >= 0; b++, q--)
                {
                    if (line[q] >= 'a' && line[q] <= 'f')
                        c = (line[q] - 'a' + 10);
                    else if (line[q] >= 'A' && line[q] <= 'F')
                        c = (line[q] - 'A' + 10);
                    else if (line[q] >= '0' && line[q] <= '9')
                        c = (line[q] - '0');
                    else
                        c = 0;
                    if (b == 0)
                        num = c;
                    else
                        num |= c << 4;
                }
                data[a - 1] = (byte)num;
            }
        }


        private void crypt(byte[] dest, byte[] src, int decipher)
        {
            // union {
            //		BYTE full[64];                  /* use buf.full[] to access all bits */
            //		struct {                        /* use buf.half.left|right to access */
            //			struct byte32 left;         /* an entire half.                   */
            //			struct byte32 right;        /* buf.half.left|right.c[] to get a  */
            //		} half;                         /* single bit in a half.             */
            // } buf;

            // struct byte32 temp_right;           /* Temporary storage for right half  */
            byte[] e_sel = new byte[48];                     /* Bits selected via e[] table       */
            byte[] f_sel = new byte[32];                     /* Bits from S-box function          */

            int i, j, k;
            int iter, key;                      /* Iteration count and current key   */
            byte[] buf_full = new byte[64];
            byte[] temp_right = new byte[32];

            /*  Perform the initial permutation. T0 = IP[src] */
            for (i = 0; i < 64; i++)
                buf_full[i] = ((src[ip[i, 0]] & ip[i, 1]) != 0) ? (byte)1 : (byte)0;

            /* Now perform { Li = Ri-1; Ri = Li-1 ^ f(Ri-1, Ki) } 16 times */
            for (iter = 0; iter < 16; iter++)
            {
                for (int x = 0; x < 32; x++)
                    temp_right[x] = buf_full[x + 32];    /* Save right half in a temp location */
                key = (decipher != 0) ? 15 - iter : iter;  /* Go backwards to decipher */

                /* Now get perform E box selection and xor in the current key */

                for (i = 0; i < 48; i++)
                    e_sel[i] = (byte)(buf_full[32 + (e[i])] ^ ks[key, i]);

                /* now peform s-box function */

                for (j = 0; j < 8; j++)
                {
                    i = 6 * j;
                    k = s[j, (e_sel[i + 0] << 5) |
                             (e_sel[i + 1] << 3) |
                             (e_sel[i + 2] << 2) |
                             (e_sel[i + 3] << 1) |
                             (e_sel[i + 4] << 0) |
                             (e_sel[i + 5] << 4)];
                    i = 4 * j;
                    f_sel[i + 0] = (byte)((k >> 3) & 1);
                    f_sel[i + 1] = (byte)((k >> 2) & 1);
                    f_sel[i + 2] = (byte)((k >> 1) & 1);
                    f_sel[i + 3] = (byte)((k >> 0) & 1);
                }

                /* Now perform permutation P */

                for (i = 0; i < 32; i++)
                    buf_full[32 + i] = (byte)(buf_full[i] ^ f_sel[p[i]]);

                //        buf.half.left = temp_right;     /* Li = Ri-1 */
                for (int x = 0; x < 32; x++)
                    buf_full[x] = temp_right[x];    /* Save right half in a temp location */
            }

            //    temp_right = buf.half.right;        /* Final permutations are reversed */
            //    buf.half.right = buf.half.left;
            //    buf.half.left = temp_right;
            for (int x = 0; x < 32; x++)
                temp_right[x] = buf_full[32 + x];
            for (int x = 0; x < 32; x++)
                buf_full[32 + x] = buf_full[x];
            for (int x = 0; x < 32; x++)
                buf_full[x] = temp_right[x];

            /*  Perform final permutation. dest = FP[T16] */

            for (i = 0; i < 8; i++)             /* Clear the bit values at dest[] */
                dest[i] = 0;

            for (i = j = 0; i < 8; i++)
            {
                for (k = 1; k <= 128; k <<= 1, j++)
                    if (buf_full[fp[j]] != 0)
                        dest[i] |= (byte)k;
            }
        }

        private String UnManglePassword(String iString, byte[] Key)
        {

            String oString = new String("".ToCharArray());
            byte[] Result = new byte[10];
            short BlockCount, PwdLength, i;
            int Seed = 0x4f627a3b;
            //	  BYTE Result [10];
            //	  WORD BlockCount, PwdLength, i;
            //	  DWORD Seed = 0x4f627a3b;

            String sSub0 = new String(iString.Substring(1, 2).ToCharArray());
            PwdLength = (short)int.Parse(sSub0, NumberStyles.HexNumber);
            BlockCount = (short)(iString[0] - '0');
            //	  sscanf(iString+1,"%02X",&PwdLength);
            //	  BlockCount = iString[0] - '0';

            if (BlockCount > 8 || PwdLength > 64 || BlockCount != (PwdLength + 7) / 8)
            {
                oString = iString;
                return oString;
            }
            //	  if (BlockCount > 8 || PwdLength > 64 ||  BlockCount != (PwdLength + 7) / 8) {
            //				 strcpy(oString,iString);
            //				 return;
            //				 }

            des_key(Key);
            for (i = 0; i < BlockCount; i++)
            {
                char[] a ={ 'a' };
                String sTemp0 = new String(a);
                byte[] oStringTemp = new byte[8];
                sTemp0 = iString.Substring(i * 16 + 3);
                char[] sLine = sTemp0.ToCharArray();
                _cvtsh(Result, sLine, 8);
                crypt(oStringTemp, Result, 1);
                int realLen;
                for (realLen = 0; realLen < 8; realLen++)
                    if (oStringTemp[realLen] == 0x00) break;

                // Use another non-deprecated API
                //String sTemp1 = new String(oStringTemp, 0, 0, realLen);
                String sTemp1;


                MemoryStream ms = new MemoryStream(oStringTemp);
                Encoding enc = null;
                StreamReader reader = null;
                try
                {
                    enc = Encoding.GetEncoding("iso-8859-1");
                    reader = new StreamReader(ms, enc);
                }
                catch (Exception)
                {
                    reader = new StreamReader(ms);
                }

                sTemp1 = reader.ReadToEnd();

                oString += sTemp1;
                //unsafe
                //{

                //    fixed (sbyte* poStringTemp = oStringTemp)
                //    {

                //        try
                //        {
                //            sTemp1 = new String(poStringTemp, 0, realLen, "8859_1");
                //        }
                //        catch (Exception e)
                //        {
                //            sTemp1 = new String(poStringTemp, 0, realLen);
                //        }
                //    }
                //}


            }

            //	  des_key ((unsigned char *) Key);
            //	  for (i = 0; i < BlockCount; i++) {
            //				 _cvtsh(Result,(iString+i*16)+3,8);
            //				 crypt((unsigned char *) (oString+i*8), Result, TRUE);
            //				 }
            char[] oStringTemp1 = oString.ToCharArray();
            int SeedTemp;
            for (i = 0; i < PwdLength / 4; i++)
            {
                SeedTemp = 0;
                for (int x = 0; x < 4; x++)
                    SeedTemp = (SeedTemp << 8) + (int)oStringTemp1[(i * 4) + (4 - x - 1)];
                Seed ^= SeedTemp;
            }
            //	  for (i=0;i<PwdLength/4;i++)
            //				 Seed ^= ((DWORD *)oString)[i];

            //String s0 = new String(Integer.toHexString(Seed));
            String s0 = new String(Conversion.Hex(Seed).ToCharArray());
            s0.ToUpper();
            String sZeros = new String("00000000".ToCharArray());
            String s1 = new String("".ToCharArray());
            if (s0.Length < 8)
                s1 = sZeros.Substring(s0.Length) + s0;
            else
                s1 = s0;
            //	  sprintf((char *) Result,"%08lX",Seed);

            //	String s2 = new String(iString.substring(BlockCount*16+3));
            //	if (s1.compareTo(s2) == 0)
            //		oString = iString;
            //	  if (strncmp((iString+BlockCount*16)+3, (char *) Result,8)) {
            //				 memcpy(oString,iString,PwdLength);
            //				 }
            //	  oString[PwdLength] = 0;
            return oString;
        }


        void MakeKey(short[] Key, String UserName, String ServerName)
        {

            //void MakeKey(LPSTR Key,LPSTR s1,LPSTR s2) {

            //    int i;
            //    WORD seed=0x7f3b;
            //    WORD *key = (WORD *)Key;

            //    for (i=0;(*s1||*s2)&&i<4;i++) {
            //            seed ^= (*s1|(*s2<<8))^0x6b2c;
            //            key[i%4==1?2:i%4==2?1:i%4] ^= seed;
            //				 if (*s1) s1++;
            //				 if (*s2) s2++;
            //				 }

            int i, k1 = 0, k2 = 0;
            short seed = 0x7f3b;

            int len1 = UserName.Length;
            int len2 = ServerName.Length;

            char[] s1 = UserName.ToCharArray();
            char[] s2 = ServerName.ToCharArray();
            for (i = 0; ((k1 < len1) || (k2 < len2)) && i < 4; i++)
            {
                int c0 = (short)s2[k2] << 8;
                seed ^= (short)(((short)s1[k1] | (short)c0) ^ 0x6b2c);
                Key[i % 4 == 1 ? 2 : i % 4 == 2 ? 1 : i % 4] ^= seed;
                if (k1 < len1) k1++;
                if (k2 < len2) k2++;
            }
        }

        ////////////////////////////////////////////////////////////
        String UnMakeEP(String iString, String UserName, String ServerName)
        {
            String Password = new String("".ToCharArray());
            short[] Key2 = { 0x0201, 0x0403, 0x0605, 0x0807, 0x0000 }; //9] = {1,2,3,4,5,6,7,8,0};
            byte[] Key = new byte[9];

            MakeKey(Key2, ServerName, UserName);
            // transfer short to byte
            Key[0] = (byte)(Key2[0] & 0x00ff);
            Key[1] = (byte)(Key2[0] >> 8);
            Key[2] = (byte)(Key2[1] & 0x00ff);
            Key[3] = (byte)(Key2[1] >> 8);
            Key[4] = (byte)(Key2[2] & 0x00ff);
            Key[5] = (byte)(Key2[2] >> 8);
            Key[6] = (byte)(Key2[3] & 0x00ff);
            Key[7] = (byte)(Key2[3] >> 8);
            Key[8] = (byte)0;

            // Password = UnManglePassword(iString, Key);

            // MakeKey((char *) Key,ServerName,UserName);
            //StringTokenizer  st  =  new  StringTokenizer(iString, "!", false);
            string[] split = iString.Split("!".ToCharArray());
            //String sTemp = new String("");
            String sTempOut = new String("".ToCharArray());
            foreach (string sTemp in split)
            {
                //sTemp = st.nextToken();
                sTempOut = UnManglePassword(sTemp, Key);
                Password += sTempOut;
            }
            //while  (st.hasMoreTokens())  {
            //    sTemp = st.nextToken();
            //    sTempOut = UnManglePassword(sTemp, Key);
            //    Password += sTempOut;
            //}

            //p = strtok(iString, sDel);
            //strcpy(Password, "");
            //while(p) {
            //	memset(sTemp, 0, sizeof(sTemp));
            //	strcpy(sTemp, p);
            //	UnManglePassword(sTempOut, sTemp, (char *) Key);
            //	strcat(Password, sTempOut);
            //	p = strtok(NULL, sDel);
            //}
            return Password;
        }
        /////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////
        public String NewDecryptStr(String sInput)
        {
            //char sTemp[ENCRYPT_LEN], s0[ENCRYPT_LEN];

            // translate to multibyte
            //strcpy(s0, sEncryptedPassword + lstrlen(CRYPT_TITLE));
            //UnMakeEP(s0, PSEUDO_USER_NAME, PSEUDO_SERVER_NAME, sTemp);

            // the firs 3 digit is useless
            //strcpy(sEncryptedPassword, sTemp + 3);
            //return 1;

            String s0 = new String(sInput.Substring(CRYPT_TITLE.Length).ToCharArray());
            String sTemp = UnMakeEP(s0, PSEUDO_USER_NAME, PSEUDO_SERVER_NAME);
            String sResult = new String(sTemp.Substring(3).ToCharArray());

            if (sResult.IndexOf('\0') >= 0)
                sResult = sResult.Substring(0, sResult.IndexOf('\0'));
            return sResult;
        }
        /////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////
        //private void _cvths(void *data,char *line,int len)
        private void _cvths(byte[] data, char[] line, int len)
        {
            int a;
            String sTemp;

            if (len > 32) len = 32;
            for (a = 0; a < len; a++)
            {
                char b = (char)(data[a] & 0xFF);
                //sTemp = new String(Integer.toHexString((int) b).toUpperCase());
                sTemp = new String(Conversion.Hex((int)b).ToUpper().ToCharArray());
                if (sTemp.Length < 2)
                    sTemp = "0" + sTemp;
                line[a * 2] = sTemp[0];
                line[a * 2 + 1] = sTemp[1];
            }
            //int a;
            //if (len > 32)
            //	len = 32;
            //for (a=0;a<len;a++)
            //	sprintf(line + (a*2), "%02X",((unsigned char *)(data))[a]);
        }


        //////////////////////////////////////////////////////////
        private String ManglePassword(String isString, byte[] Key)
        {
            byte[] Result = new byte[8];
            int BlockCount, PwdLength, i;
            int Seed = 0x4f627a3b;
            //		char iString[] = new char[4096];
            String oString = new String("".ToCharArray());

            //BYTE Result [8];
            //DWORD Seed = 0x4f627a3b;
            //int BlockCount, PwdLength, i;
            //char iString[40960];

            //memset(iString,0,sizeof(iString));
            //strncpy(iString,isString, strlen(isString));

            PwdLength = isString.Length;
            BlockCount = (PwdLength + 7) / 8;
            //PwdLength = strlen (iString);
            //BlockCount = (PwdLength + 7) / 8;

            char[] sOne = new char[1];
            sOne[0] = (char)(BlockCount + (int)'0');
            String sTemp = new String(sOne);
            oString += sTemp;
            //sTemp = Integer. toHexString(PwdLength).toUpperCase();
            sTemp = Conversion.Hex(PwdLength).ToUpper();
            if (sTemp.Length < 2)
                oString += "0";
            oString += sTemp;
            //oString[0] = '0' + BlockCount;
            //sprintf(oString+1,"%02X",PwdLength);

            des_key(Key);
            //des_key ((unsigned char *) Key);

            for (i = 0; i < BlockCount; i++)
            {
                String sTemp0 = new String("".ToCharArray());
                byte[] sLine = new byte[8];
                char[] sTemp1 = new char[16];
                int k;
                for (k = 0; k < 8; k++) sLine[k] = (byte)0;

                for (k = 0; k < 8; k++)
                {
                    if (i * 8 + k < isString.Length)
                        sLine[k] = (byte)isString[i * 8 + k];
                    else
                        break;
                }
                crypt(Result, sLine, 0);
                _cvths(Result, sTemp1, 8);

                int realLen;
                for (realLen = 0; realLen < 16; realLen++)
                    if (sTemp1[realLen] == 0x00) break;

                sTemp0 = new String(sTemp1, 0, realLen);

                oString += sTemp0;
            }
            //for (i = 0; i < BlockCount; i++) {
            //	crypt(Result, (unsigned char *) (iString+i*8), FALSE);
            //  _cvths(Result, (oString+i*16)+3, 8);
            //}

            char[] iStringTemp = isString.ToCharArray();
            int SeedTemp;
            for (i = 0; i < PwdLength / 4; i++)
            {
                SeedTemp = 0;
                for (int x = 0; x < 4; x++)
                    SeedTemp = (SeedTemp << 8) + (int)iStringTemp[(i * 4) + (4 - x - 1)];
                Seed ^= SeedTemp;
            }
            //for (i=0;i<PwdLength/4;i++)
            //        Seed ^= ((DWORD *)iString)[i];

            //String s0 = new String(Integer.toHexString(Seed).toUpperCase());
            String s0 = new String(Conversion.Hex(Seed).ToUpper().ToCharArray());
            s0.ToUpper();
            String sZeros = new String("00000000".ToCharArray());
            String s1 = new String("".ToCharArray());
            if (s0.Length < 8)
                s1 = sZeros.Substring(s0.Length) + s0;
            else
                s1 = s0;
            oString += s1;
            //memset(iString,0,128);
            //sprintf((oString+BlockCount*16)+3,"%08lX",Seed);
            return oString;
        }


        private String MakeEP(String UserName, String ServerName, String Password)
        {
            short[] Key2 = { 0x0201, 0x0403, 0x0605, 0x0807, 0x0000 }; //9] = {1,2,3,4,5,6,7,8,0};
            byte[] Key = new byte[9];

            MakeKey(Key2, ServerName, UserName);
            // transfer short to byte
            Key[0] = (byte)(Key2[0] & 0x00ff);
            Key[1] = (byte)(Key2[0] >> 8);
            Key[2] = (byte)(Key2[1] & 0x00ff);
            Key[3] = (byte)(Key2[1] >> 8);
            Key[4] = (byte)(Key2[2] & 0x00ff);
            Key[5] = (byte)(Key2[2] >> 8);
            Key[6] = (byte)(Key2[3] & 0x00ff);
            Key[7] = (byte)(Key2[3] >> 8);
            Key[8] = (byte)0;

            // String oString = new String(ManglePassword(Password, Key));
            String oString = new String("".ToCharArray());
            String sTemp = new String("".ToCharArray());
            int iLen = Password.Length;
            for (int k = 0; k < iLen; k += 64)
            {
                if (k + 64 > iLen)
                    sTemp = Password.Substring(k, iLen - k);
                else
                    sTemp = Password.Substring(k, 64);
                oString += ManglePassword(sTemp, Key);
                if (k + 64 < iLen)
                    oString += "!";
            }
            //strcpy(oString, "");
            //for(k=0; k<iLen; k+=64)	  {
            //	memset(sTemp, 0, sizeof(sTemp));
            //	strncpy(sTemp, (Password + k), 64);
            //	ManglePassword(sTempOut, sTemp, (char *) Key);
            //	strcat(oString, sTempOut);
            //	if (k+64 < iLen)
            //		strcat(oString, "!");
            //}
            return oString;
        }

        // Encrypt the original Password to an encrypted password
        public String NewEncryptStr(String sOrgPassword)
        {
            // char sTemp[ENCRYPT_LEN], s0[ENCRYPT_LEN];
            // char sRandStr[20];

            Double d = new Random().NextDouble();
            String s = new String(d.ToString().ToCharArray());


            //		srand(GetCurrentTime());
            //		sprintf(sRandStr, "%03d", (rand() % 1000));

            // make sure sRandStr 3 digits
            String sTemp = new String(s.Substring(2, 3).ToCharArray());
            while (sTemp.Length < 3)
                sTemp = "0" + sTemp;

            // translate to multibyte
            String s0 = new String(sTemp.ToCharArray());

            s0 = s0 + sOrgPassword;

            //strcpy(s0, sRandStr);
            //strcat(s0, sOrgPassword);
            sTemp = MakeEP(PSEUDO_USER_NAME, PSEUDO_SERVER_NAME, s0);

            // translate to WideChar and put !CRYPT!
            String szEncryptStrBuf = new String(CRYPT_TITLE.ToCharArray());
            szEncryptStrBuf += sTemp;

            //lstrcpy(szEncryptStrBuf, CRYPT_TITLE);
            //lstrcat(szEncryptStrBuf, sTemp);

            return szEncryptStrBuf;
        }

        public String EncryptStr(String SourceStr)
        {
            String sEncodedStr = new String("".ToCharArray());
            int len = SourceStr.Length;
            char[] sa = new char[len + 1];
            sa = SourceStr.ToCharArray();

            // must encode end of string, even SourceStr is null, (length == 0)
            if (len <= 0) return "0000";

            int high, low;
            for (int k = 0; k < len; k++)
            {
                high = sa[k] & 0x0000FF00;
                high = high >> 8;
                if (high < 16)
                {
                    if (high >= 0)
                        sEncodedStr += "0";	// less than 16, must add 0
                }
                sEncodedStr += Conversion.Hex(high);//Integer.toHexString(high);
                low = sa[k] & 0x000000FF;
                if (low < 16)
                {
                    if (low >= 0)
                        sEncodedStr += "0";	// less than 16, must add 0
                }
                sEncodedStr += Conversion.Hex(low);//Integer.toHexString(low);
            }

            // last one add the end of string
            sEncodedStr += "0000";
            return (sEncodedStr);
        }

        // 2/17/98 Terrence : every 4 char to represent one char
        public String DecryptStr(String EncodedStr)
        {
            int len = EncodedStr.Length;
            if ((len & 0x00000003) != 0) return "0000";// length must be even
            if (len == 0) return "0000";

            int x, y;
            int high, low;
            char[] dst = new char[(len * 4) + 1];
            char[] rst = new char[(len - 4) / 4];
            //EncodedStr.getChars(0, len-1, dst, 0);
            dst = EncodedStr.ToCharArray(0, len);
            int k;
            string sTemp = "";

            for (k = 0; k < (len - 4); k += 4)
            {
                //high = Character.digit(dst[k], 16);
                //low = Character.digit(dst[k+1], 16);

                sTemp = String.Format("{0}", dst[k]);
                high = int.Parse(sTemp, NumberStyles.HexNumber);


                sTemp = String.Format("{0}", dst[k + 1]);
                low = int.Parse(sTemp, NumberStyles.HexNumber);

                x = (high * 16) + low;

                //high = Character.digit(dst[k+2], 16);
                //low = Character.digit(dst[k+3], 16);

                sTemp = String.Format("{0}", dst[k + 2]);
                high = int.Parse(sTemp, NumberStyles.HexNumber);


                sTemp = String.Format("{0}", dst[k + 3]);
                low = int.Parse(sTemp, NumberStyles.HexNumber);

                y = (high * 16) + low;

                rst[k / 4] = (char)((x << 8) + y);
            }

            //		rst[(len/4)-1] = 0;
            String DecodedStr = new String(rst);
            return DecodedStr;
        }


        public String addOneParam(String name, bool b)
        {
            String r = new String(name.ToCharArray());
            String s = new String("".ToCharArray());

            r += "=";
            if (b == true) s = "1";
            else s = "0";

            String s1 = new String(EncryptStr(s).ToCharArray());
            r += s1;

            return r;
        }

        public String addOneParam(String name, int i)
        {
            String r = new String(name.ToCharArray());
            String s = new String("".ToCharArray());
            int i0 = i;

            r += "=";
            s = i0.ToString();

            String s1 = new String(EncryptStr(s).ToCharArray());
            r += s1;

            return r;
        }


        public String addOneParam(String name, String s)
        {
            String r = new String(name.ToCharArray());
            String s2 = new String(EncryptStr(s).ToCharArray());

            r += "=";
            if (s2.Length == 0) r += "\"\"";
            else r += s2;
            return r;
        }

        public String addRandomKey()
        {
            String s = new String("".ToCharArray());

            Double d = new Random().NextDouble();
            s += "&KEY=" + d.ToString();

            return s;
        }

        public int StringToInt(String s)
        {
            int x = 0;
            int len = s.Length;
            int a, k;
            char[] sa = new char[len + 1];

            sa = s.ToCharArray();

            for (k = 0; k < len; k++)
            {
                a = 0;
                if (sa[k] == (char)0) break;
                if ((sa[k] >= (char)0x0030) &&
                    (sa[k] <= (char)0x0039))
                {
                    a = sa[k] - (char)0x0030;
                    x = x * 10 + a;
                }
            }
            return x;
        }
    }
    #endregion

    #region OpenSSL Encrypter & Decrypter
    public class OpenSSLEncryptDecrypt
    {
        public string RSAPrivateEncrypt(string keyFile, string rawData)
        {
            string strEncryptedData = string.Empty;

            using (OpenSSL.Core.BIO bioPrivateKey = OpenSSL.Core.BIO.File(keyFile, "r"))
            {
                using (OpenSSL.Crypto.RSA rsaServiceProvider = OpenSSL.Crypto.RSA.FromPrivateKey(bioPrivateKey))
                {
                    byte[] byteEncryptedData = rsaServiceProvider.PrivateEncrypt(new UTF8Encoding().GetBytes(rawData), OpenSSL.Crypto.RSA.Padding.PKCS1);
                    strEncryptedData = BitConverter.ToString(byteEncryptedData).Replace("-", "");
                }
            }

            return strEncryptedData;
        }


    }
    #endregion

    #region Using Pem key to RSA Encrypter & Decrypter
    public class RSAEncryptDecrypt
    {
        //The max size that RSACryptoServiceProvider can handle is base on the RSA key size.
        //(key size/8)-11 ex (1024/8)-11 = 117 (the max lenght which can handle is 117)
        public string Decrypt(string pemFilePath,string rawData)
        {
            RSACryptoServiceProvider m_rsaProvider = PemKeyUtility.GetRSAProviderFromPemFile(pemFilePath);
            var decryptString = Encoding.UTF8.GetString(m_rsaProvider.Decrypt(Convert.FromBase64String(rawData), false));
            return decryptString;
        }
        public string Encrypt(string pemFilePath, string rawData)
        {
            RSACryptoServiceProvider m_rsaProvider = PemKeyUtility.GetRSAProviderFromPemFile(pemFilePath);
            string encryptString = Convert.ToBase64String(m_rsaProvider.Encrypt(Encoding.UTF8.GetBytes(rawData), false));

            return encryptString;
        }
    }
    #endregion
}
