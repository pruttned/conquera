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
using System.Linq;
using System.Text;
using Ale.Content;
using System.IO;

namespace Ale.Sound
{
    internal abstract class SoundLoader : IAssetLoader
    {
        private static string SoundDir;

        protected abstract AleSound CreateSound(SoundManager soundManager, string fileName);

        object IAssetLoader.LoadAsset(string name, ContentGroup contentGroup, out long Id)
        {
            Id = -1;

            SoundManager soundManager = contentGroup.ServiceProvider.GetService(typeof(SoundManager)) as SoundManager;
            if (null == soundManager)
            {
                throw new InvalidOperationException("SoundManager was not registered as a ServiceProvider");
            }

            if (null == SoundDir)
            {
                SoundDir = Path.Combine(contentGroup.ParentContentManager.RootDirectory, "Sounds");
            }

            string fileName = Path.Combine(SoundDir, name + ".xnb");

            return CreateSound(soundManager, fileName);
        }

        object IAssetLoader.LoadAsset(long id, ContentGroup contentGroup, out string name)
        {
            throw new NotImplementedException();
        }
    }

    internal class SoundLoader2d : SoundLoader
    {
        protected override AleSound CreateSound(SoundManager soundManager, string fileName)
        {
            return new Sound2d(soundManager, fileName);
        }
    }

    internal class SoundLoader3d : SoundLoader
    {
        protected override AleSound CreateSound(SoundManager soundManager, string fileName)
        {
            return new Sound3d(soundManager, fileName);
        }
    }
    

}
