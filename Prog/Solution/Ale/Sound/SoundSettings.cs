using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Settings;

namespace Ale.Sound
{
    [AppSettings("Cfg.ini", SectionName = "Sound")]
    public class SoundSettings : IAppSettings
    {
        [AppSettingsParam]
        public float MusicVolume { get; set; }

        [AppSettingsParam]
        public int MaxChannelCnt { get; set; }

        [AppSettingsParam]
        public float SoundVolume { get; set; }

        public SoundSettings()
        {
            MusicVolume = 0.5f;
            MaxChannelCnt = 2048;
            SoundVolume = 0.7f;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
