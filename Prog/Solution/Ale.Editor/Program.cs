using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ale.Settings;
using System.IO;

namespace Ale.Editor
{
    static class Program
    {        
        public readonly static string AssetSettingsSourceRootDirectory;

        static Program()
        {            
            AssetSettingsSourceRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\Ale.Editor.AssetSettings");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RendererWindow());
        }
    }
}
