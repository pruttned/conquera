using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Settings
{
    [AppSettings("Cfg.ini", SectionName = "Common")]
    public class CommonSettings : IAppSettings
    {
        [AppSettingsParam]
        public bool ConstraintCursor { get; set; }

        public CommonSettings()
        {
            ConstraintCursor = true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
