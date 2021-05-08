using System.IO;


namespace EncDec
{
    class Utils
    {
        private static Utils Instance = null;

        private Utils()
        {
            
        }

        public static Utils GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Utils();
            }

            return Instance;
        }

        public bool ParseArgKey(string argCmd, string[] keys, ref string outKey)
        {
            if (argCmd.Length == 0 || keys.Length == 0)
            {
                outKey = string.Empty;
                return false;
            }

            int postfixIndex = argCmd.IndexOf('=');
            if (postfixIndex < 0)
            {
                outKey = "";
                return false;
            }

            string argKey = argCmd.Substring(0, postfixIndex);
            if (argKey == keys[0] || argKey == keys[1])
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

        public bool ParseArgPath(string argCmd, string[] keys, ref string outPath)
        {
            if (argCmd.Length == 0 || keys.Length == 0)
            {
                outPath = string.Empty;
                return false;
            }

            int postfixIndex = argCmd.IndexOf('=');
            if (postfixIndex < 0)
            {
                outPath = string.Empty;
                return false;
            }

            string argPath = argCmd.Substring(0, postfixIndex);
            if (argPath == keys[0] || argPath == keys[1])
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
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (fs.Length <= 0 || !fs.CanRead)
            {
                return false;
            }

            byte[] dataRead = new byte[fs.Length];
            fs.Read(dataRead, 0, dataRead.Length);

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

            if (!File.Exists(path))
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
    }
}
