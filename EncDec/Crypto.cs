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
        byte[] Iv = null;
        //byte[] Key = null;

        private Crypto()
        {
            RijAlg = new RijndaelManaged();

            // Fixme: Generate IV and write into non-encrypted file and read IV from encrypted file, RijAlg.GenerateIV();
            Iv = Encoding.ASCII.GetBytes("0123456789ABCDEF");

            //Key = Encoding.ASCII.GetBytes("0123456789ABCDEF0123456789ABCDEF");
            RijAlg.IV = Iv;
            //RijAlg.Key = Key;
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

        public byte[] GetEncryptData(byte[] data, int dataLength, byte[] key)
        {
            byte[] dataEncrypt = null;

            ICryptoTransform encryptorInterface = RijAlg.CreateEncryptor(key, Iv);
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

        public byte[] GetDecryptData(byte[] data, int dataLength, byte[] key)
        {
            byte[] dataDecrypt = new byte[dataLength];

            // see https://msdn.microsoft.com/en-us/library/system.security.cryptography.paddingmode(v=vs.110).aspx
            // see https://stackoverflow.com/questions/8583112/padding-is-invalid-and-cannot-be-removed
            ICryptoTransform decryptorInterface = RijAlg.CreateDecryptor(key, Iv);
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
