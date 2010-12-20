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

using Ale.Content;
using Ale.Scene;
using Ale.Graphics;

namespace Ale.Editor
{
    [RenderInfo("Material")]
    public class MaterialRenderInfo : IRenderInfo
    {
        private BaseScene mScene = null;

        public BaseScene Scene
        {
            get { return mScene; }
        }

        public void Update(object assetSettings, SceneManager sceneManager, ContentGroup content)
        {
            if (Scene == null)
            {
                mScene = new MaterialScene(sceneManager, content);
            }
            ((MaterialScene)Scene).SetMaterialSettings((MaterialSettings)assetSettings);
        }
    }
}
