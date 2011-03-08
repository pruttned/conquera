using System;
using System.IO;
using Ale.Settings;

namespace Conquera.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            InitDefaultsettings();



            using (EditorApplication app = new EditorApplication())
            {
                //try
                //{
                app.Run();
                //}
                //catch (Exception ex)
                //{
                //    Tracer.WriteError(ex.ToString());
                //    //MessageBox.Show(ex.ToString());
                //    throw;
                //}
            }
        }


        /// <summary>
        /// Copies default editor settings to the editor's user dir
        /// </summary>
        private static void InitDefaultsettings()
        {
            string srcDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EditorDefaultCfg");
            string destDir = MainSettings.Instance.UserDir;

            foreach (string srcFile in Directory.GetFiles(srcDir, "*.ini"))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(srcFile));
                if (!File.Exists(destFile))
                {
                    File.Copy(srcFile, destFile);
                }
            }
        }
    }
}

