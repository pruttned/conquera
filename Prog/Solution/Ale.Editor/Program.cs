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
        //[STAThread]
        static void Main()
        {
            using (AleApplication app = new AleApplication())
            {
                app.Run();
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new RendererWindow());
        }
    }
}
