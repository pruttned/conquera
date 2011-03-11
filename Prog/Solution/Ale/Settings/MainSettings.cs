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
using System.Text;
using System.IO;

namespace Ale.Settings
{
    /// <summary>
    /// Use MainSettings.Instance to get settings.
    /// </summary>
    [AppSettings(null, SectionName = "Main", Readonly=true)]
    public class MainSettings : IAppSettings
    {
        private static MainSettings mInstance;

        private string mUserDir;
        private string mDataDirectory;

        public static MainSettings Instance
        {
            get
            {
                if (null == mInstance)
                {
                    AppSettingsManager manager = new AppSettingsManager(AppDomain.CurrentDomain.BaseDirectory);
                    mInstance = manager.GetSettings<MainSettings>();
                }
                return mInstance;
            }
        }

        [AppSettingsParam]
        public string UserDir 
        {
            get { return mUserDir; }
            set 
            {
                mUserDir = value;
                if (!Path.IsPathRooted(mUserDir))
                {
                    mUserDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), mUserDir);
                }
            } 
        }

        [AppSettingsParam]
        public string DataDirectory
        {
            get { return mDataDirectory; }
            set
            {
                mDataDirectory = value;
                if (!Path.IsPathRooted(mDataDirectory))
                {
                    mDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mDataDirectory);
                }
            }
        }
        [AppSettingsParam]
        public string Mod { get; set; }

        [AppSettingsParam]
        public bool Logging { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
        
        private MainSettings()
        {
            UserDir = @"My Games\AL\";
            DataDirectory = "Data";
            Mod = "AL.mod";
            Logging = true;
        }
    }
}
