using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Settings
{
    [AppSettings("Cfg.ini", SectionName = "Video")]
    public class VideoSettings : IAppSettings
    {
        [AppSettingsParam]
        public int ScreenWidth { get; set; }

        [AppSettingsParam]
        public int ScreenHeight { get; set; }

        [AppSettingsParam]
        public bool Fullscreen { get; set; }

        public VideoSettings()
        {
            ScreenWidth = 800;
            ScreenHeight = 600;
            Fullscreen = false;
        }

        public VideoSettings(GraphicsDevice graphicsDevice)
        {
            if (null == graphicsDevice) throw new ArgumentNullException("graphicsDevice");

            ScreenWidth = graphicsDevice.DisplayMode.Width;
            ScreenHeight = graphicsDevice.DisplayMode.Height;
            Fullscreen = true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
