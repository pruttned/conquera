using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ale.Settings;

namespace Ale.Tools
{
    public static class Tracer
    {
        private static StreamWriter mWriter = null;

        static Tracer()
        {
            if (MainSettings.Instance.Logging)
            {
                string path = Path.Combine(MainSettings.Instance.UserDir, "Trace.log");

                //ensure dir
                if (!Directory.Exists(MainSettings.Instance.UserDir))
                {
                    Directory.CreateDirectory(MainSettings.Instance.UserDir);
                }

                mWriter = new StreamWriter(path, false);
                mWriter.AutoFlush = true;
            }
            WriteInfo("Trace begin. ({0})", DateTime.Now.ToString());
        }

        public static void WriteInfo(object obj)
        {
            WriteInfo(obj.ToString());
        }

        public static void WriteWarning(object obj)
        {
            WriteWarning(obj.ToString());
        }

        public static void WriteError(object obj)
        {
            WriteError(obj.ToString());
        }

        public static void WriteInfo(string message, params object[] arg)
        {
            if (null != mWriter)
            {
                Write("INFO   ", string.Format(message, arg));
            }
        }

        public static void WriteWarning(string message, params object[] arg)
        {
            if (null != mWriter)
            {
                Write("WARNING", string.Format(message, arg));
            }
        }

        public static void WriteError(string message, params object[] arg)
        {
            if (null != mWriter)
            {
                Write("ERROR  ", string.Format(message, arg));
            }
        }

        private static void Write(string level, string message)
        {
            mWriter.WriteLine(string.Format("[{0}][{1}] {2}", DateTime.Now.ToString("HH:mm:ss"), level, message));
        }
    }
}
