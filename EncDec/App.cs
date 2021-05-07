﻿using System;
using System.Text;
using System.IO;

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

        public bool ParseArgKey(string argCmd, ref string outKey)
        {
            int postfixIndex = argCmd.IndexOf('=');
            if (postfixIndex < 0)
            {
                outKey = "";
                return false;
            }

            string argKey = argCmd.Substring(0, postfixIndex);
            if (argKey == "-k" || argKey == "--key")
            {
                string argKeyValue = argCmd.Substring(postfixIndex + 1);

                // TODO ...
                if (argKeyValue.Length != 32)
                {
                    
                }

                outKey = argKeyValue;
                return true;
            }

            return false;
        }

        public bool ParseArgPath(string argCmd, ref string outPath)
        {
            int postfixIndex = argCmd.IndexOf('=');
            if (postfixIndex < 0)
            {
                outPath = string.Empty;
                return false;
            }

            string argPath = argCmd.Substring(0, postfixIndex);
            if (argPath == "-p" || argPath == "--path")
            {
                string argPathValue = argCmd.Substring(postfixIndex + 1);
                outPath = argPathValue;
                return true;
            }

            return false;
        }

        public bool ReadFile(string path, int dataLength, ref byte[] outData)
        {
            // Fixme: Unused dataLength variable!
            if (path.Length == 0)
            {
                outData = new byte[0];
                return false;
            }

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            //if (fs.Length <= 0 || !fs.CanRead)
            if (fs.Length < 16 || !fs.CanRead) // Minimal size 16 bytes for file, valid for Rijndael algorithm
            {
                outData = new byte[0];
                return false;
            }

            byte[] dataRead = new byte[fs.Length];
            int readBytes = fs.Read(dataRead, 0, dataRead.Length);
            Console.WriteLine("readBytes: {0}, fs.Length: {1}", readBytes, fs.Length);
            outData = new byte[readBytes]; // check!
            outData = dataRead;
            fs.Close();

            return true;
        }

        public bool WriteFile(string path, int dataLength, byte[] data)
        {
            if (path.Length == 0)
            {
                return false;
            }

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            if (!fs.CanWrite)
            {
                return false;
            }

            long dataLengthL = dataLength;
            if (dataLengthL < fs.Length)
            {
                fs.SetLength(dataLengthL);
            }

            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(data, 0, dataLength);
            fs.Close();

            return true;
        }

        public void ProcessCommandLine(string[] args)
        {
            //Console.WriteLine("args.Length: {0}", args.Length);
            //foreach (string arg in args)
            //{
            //    Console.WriteLine("arg: {0}", arg);
            //}

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
                if (ParseArgKey(args[1], ref argKeyValue))
                {
                    byte[] key = Encoding.ASCII.GetBytes(argKeyValue);

                    if (ParseArgPath(args[2], ref argPathValue))
                    {
                        // read file
                        byte[] dataFromFile = null;
                        int dataLength = 0;
                        if (ReadFile(argPathValue, dataLength, ref dataFromFile))
                        {
                            // encrypt data
                            byte[] encData = Crypto.GetInstance().GetEncryptData(dataFromFile, dataFromFile.Length, key);

                            // write encrypt data into file
                            if (WriteFile(argPathValue, encData.Length, encData))
                            {
                                Console.WriteLine("File encrypted successfully!");
                            }
                        }

                        return;
                    }
                }
            }


            // [-dec | --decrypt], [-k=<secretkey> | --key=<secretkey>], [-p=<filename> | --path=<filename>]
            if (args[0] == "-dec" || args[0] == "--decrypt")
            {
                if (ParseArgKey(args[1], ref argKeyValue))
                {
                    byte[] key = Encoding.ASCII.GetBytes(argKeyValue);

                    if (ParseArgPath(args[2], ref argPathValue))
                    {
                        // read encrypted file
                        byte[] dataFromFile = null;
                        int dataLength = 0;
                        if (ReadFile(argPathValue, dataLength, ref dataFromFile))
                        {
                            // decrypt data
                            byte[] decData = Crypto.GetInstance().GetDecryptData(dataFromFile, dataFromFile.Length, key);

                            // write decrypt data into file
                            string decDataStr = Encoding.UTF8.GetString(decData);
                            byte[] decDataClear = Encoding.UTF8.GetBytes(decDataStr.Trim('\0'));

                            if (WriteFile(argPathValue, decDataClear.Length, decDataClear))
                            {
                                Console.WriteLine("File decrypted successfully!");
                            }
                        }

                        return;
                    }
                }
            }


            PrintHelp();
            return;
        }
    }
}