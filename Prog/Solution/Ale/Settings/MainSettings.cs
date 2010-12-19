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
    [AppSettings("MainCfg.ini", SectionName = "Main", Readonly=true)]
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
        public string DefaultMod { get; set; }

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
            DefaultMod = "AL.mod";
            Logging = true;
        }
    }
}
