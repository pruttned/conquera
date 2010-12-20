//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
