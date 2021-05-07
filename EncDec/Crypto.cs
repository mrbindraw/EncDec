using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace EncDec
{
    class Crypto
    {
        private static Crypto Instance = null;

        RijndaelManaged RijAlg = null;

        private Crypto()
        {
            RijAlg = new RijndaelManaged();
            //RijAlg.Mode = CipherMode.CBC;
        }

        public static Crypto GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Crypto();
            }

            return Instance;
        }

        public void GenerateIV()
        {
            RijAlg.GenerateIV();
            //RijAlg.IV = Encoding.ASCII.GetBytes("0123456789ABCDEF");
        }

        public byte[] GetIV()
        {
            return RijAlg.IV;
        }

        public byte[] GetEncryptData(byte[] data, int dataLength, byte[] key, byte[] iv)
        {
            byte[] dataEncrypt = null;

            ICryptoTransform encryptorInterface = RijAlg.CreateEncryptor(key, iv);
            RijAlg.Padding = PaddingMode.Zeros;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptorInterface, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, dataLength);
                    csEncrypt.FlushFinalBlock();
                    dataEncrypt = msEncrypt.ToArray();
                }
            }

            return dataEncrypt;
        }

        public byte[] GetDecryptData(byte[] data, int dataLength, byte[] key, byte[] iv)
        {
            byte[] dataDecrypt = new byte[dataLength];

            // see https://msdn.microsoft.com/en-us/library/system.security.cryptography.paddingmode(v=vs.110).aspx
            // see https://stackoverflow.com/questions/8583112/padding-is-invalid-and-cannot-be-removed
            ICryptoTransform decryptorInterface = RijAlg.CreateDecryptor(key, iv);
            RijAlg.Padding = PaddingMode.Zeros;

            using (MemoryStream msDecrypt = new MemoryStream(data, 0, dataLength))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptorInterface, CryptoStreamMode.Read))
                {
                    csDecrypt.Read(dataDecrypt, 0, dataLength);
                }
            }

            return dataDecrypt;
        }
    }
}
