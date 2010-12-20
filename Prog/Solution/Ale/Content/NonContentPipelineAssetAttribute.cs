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
using System.Text;

namespace Ale.Content
{
    /// <summary>
    /// Marks an asset that is not based on xna's content pipeline but is loaded with custom IAssetLoader
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class NonContentPipelineAssetAttribute : Attribute
    {
        private Type mAssetLoader;

        /// <summary>
        /// 
        /// </summary>
        public Type AssetLoader
        {
            get { return mAssetLoader; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="assetLoader">Type of IAssetLoader</param>
        public NonContentPipelineAssetAttribute(Type assetLoader)
        {
            if (null == assetLoader) { throw new ArgumentNullException("assetLoader"); }

            if (!typeof(IAssetLoader).IsAssignableFrom(assetLoader))
            {
                throw new ArgumentException(string.Format("Type '{0}' doesn't implements '{1}'", assetLoader.FullName, (typeof(IAssetLoader).FullName)));
            }

            mAssetLoader = assetLoader;
        }
    }
}
