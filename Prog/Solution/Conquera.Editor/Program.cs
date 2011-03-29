//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
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
using System.IO;
using Ale.Settings;
using Ale.Tools;
using System.Windows.Forms;

namespace Conquera.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            InitDefaultsettings();

            NewMapInfo newMapInfo = null;
            using (NewMapDlg newMapDlg = new NewMapDlg())
            {
                if (DialogResult.OK == newMapDlg.ShowDialog())
                {
                    newMapInfo = newMapDlg.NewMapInfo;
                }
                else
                {
                    return;
                }
            }

            using (EditorApplication app = new EditorApplication(newMapInfo))
            {
                //try
                //{
                    app.Run();
                //}
                //catch (Exception ex)
                //{
                //    Tracer.WriteError(ex.ToString());
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

