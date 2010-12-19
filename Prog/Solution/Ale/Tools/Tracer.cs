//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

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
