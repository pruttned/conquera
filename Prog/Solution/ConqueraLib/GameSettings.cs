using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Settings;

namespace Conquera
{
    [AppSettings("Cfg.ini", SectionName = "Game")]
    public class GameSettings : IAppSettings
    {
        [AppSettingsParam]
        public float CameraCornerScrollSpeed { get; set; }
        [AppSettingsParam]
        public float CameraScrollSpeed { get; set; }

        public GameSettings()
        {
            CameraCornerScrollSpeed = 0.3f;
            CameraScrollSpeed = 0.05f;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
