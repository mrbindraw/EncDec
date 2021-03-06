using System;
using System.Text;


namespace EncDec
{
    class App
    {
        private static App Instance = null;
        private App()
        {

        }

        public static App GetInstance()
        {
            if (Instance == null)
            {
                Instance = new App();
            }

            return Instance;
        }

        public void PrintHelp()
        {
            Console.WriteLine("[-h | --help]");
            Console.WriteLine("[-v | --version]");
            Console.WriteLine("[-enc | --encrypt], [-k=<secretkey> | --key=<secretkey>], [-p=<filename> | --path=<filename>]");
            Console.WriteLine("[-dec | --decrypt], [-k=<secretkey> | --key=<secretkey>], [-p=<filename> | --path=<filename>]");
        }

        public void PrintVersion()
        {
            Console.WriteLine("v1.0.0");
        }

        public void ProcessCommandLine(string[] args)
        {
            bool isSuccess = true;

            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }

            string argKeyValue = string.Empty;
            string argPathValue = string.Empty;

            // [-h | --help]
            if (args[0] == "-h" || args[0] == "--help")
            {
                PrintHelp();
                return;
            }


            // [-v | --version]
            if (args[0] == "-v" || args[0] == "--version")
            {
                PrintVersion();
                return;
            }


            // [-enc | --encrypt], [-k=<secretkey> | --key=<secretkey>], [-p=<filename> | --path=<filename>]
            if (args[0] == "-enc" || args[0] == "--encrypt")
            {
                if (Utils.GetInstance().ParseArgKey(args[1], new string[] { "-k", "--key" }, ref argKeyValue))
                {
                    if (Utils.GetInstance().ParseArgPath(args[2], new string[] { "-p", "--path" }, ref argPathValue))
                    {
                        isSuccess = ProcessCommandLineEncrypt(argKeyValue, argPathValue);
                        if (isSuccess)
                        {
                            Console.WriteLine("File encrypted successfully!");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Can't encrypt file: {0}", argPathValue);
                        }
                    }
                }
            }


            // [-dec | --decrypt], [-k=<secretkey> | --key=<secretkey>], [-p=<filename> | --path=<filename>]
            if (args[0] == "-dec" || args[0] == "--decrypt")
            {
                if (Utils.GetInstance().ParseArgKey(args[1], new string[] { "-k", "--key" }, ref argKeyValue))
                {
                    if (Utils.GetInstance().ParseArgPath(args[2], new string[] { "-p", "--path" }, ref argPathValue))
                    {
                        isSuccess = ProcessCommandLineDecrypt(argKeyValue, argPathValue);
                        if (isSuccess)
                        {
                            Console.WriteLine("File decrypted successfully!");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Can't decrypt file: {0}", argPathValue);
                        }
                    }
                }
            }

            if (!isSuccess)
            {
                PrintHelp();
            }
        }

        public bool ProcessCommandLineEncrypt(string argKeyValue, string argPathValue)
        {
            if (argKeyValue.Length == 0 || argPathValue.Length == 0)
            {
                return false;
            }

            byte[] key = Encoding.ASCII.GetBytes(argKeyValue);

            // read file
            byte[] dataFromFile = null;
            int dataLength = 0;

            if (!Utils.GetInstance().ReadFile(argPathValue, dataLength, ref dataFromFile))
            {
                return false;
            }

            // encrypt data
            Crypto.GetInstance().GenerateIV();
            byte[] iv = Crypto.GetInstance().GetIV();

            byte[] encData = Crypto.GetInstance().GetEncryptData(dataFromFile, dataFromFile.Length, key, iv);

            byte[] encDataIV = new byte[encData.Length + iv.Length];
            Buffer.BlockCopy(encData, 0, encDataIV, 0, encData.Length);
            Buffer.BlockCopy(iv, 0, encDataIV, encData.Length, iv.Length);

            // write encrypt data into file
            if (!Utils.GetInstance().WriteFile(argPathValue, encDataIV.Length, encDataIV))
            {
                return false;
            }

            return true;
        }

        public bool ProcessCommandLineDecrypt(string argKeyValue, string argPathValue)
        {
            if (argKeyValue.Length == 0 || argPathValue.Length == 0)
            {
                return false;
            }

            byte[] key = Encoding.ASCII.GetBytes(argKeyValue);

            // read encrypted file
            byte[] dataFromFile = null;
            int dataLength = 0;

            if (!Utils.GetInstance().ReadFile(argPathValue, dataLength, ref dataFromFile))
            {
                return false;
            }

            int ivLength = 16;
            byte[] iv = new byte[ivLength];
            Buffer.BlockCopy(dataFromFile, dataFromFile.Length - ivLength, iv, 0, ivLength);

            // decrypt data
            byte[] decData = Crypto.GetInstance().GetDecryptData(dataFromFile, dataFromFile.Length - ivLength, key, iv);

            // write decrypt data into file
            string decDataStr = Encoding.UTF8.GetString(decData);
            byte[] decDataClear = Encoding.UTF8.GetBytes(decDataStr.Trim('\0'));

            if (!Utils.GetInstance().WriteFile(argPathValue, decDataClear.Length, decDataClear))
            {
                return false;
            }

            return true;
        }

    }
}
