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
using Ale.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Loader for GraphicModelDesc asset
    /// </summary>
    public class GraphicModelDescLoader : BaseAssetLoader<GraphicModelSettings>
    {
        protected override object CreateDesc(GraphicModelSettings settings, ContentGroup contentGroup)
        {
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)contentGroup.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            return new GraphicModelDesc(graphicsDevice, settings, contentGroup);
        }

        protected override string GetName(GraphicModelSettings settings)
        {
            return settings.Name;
        }
    }
}
