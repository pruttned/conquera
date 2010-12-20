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
using Microsoft.Xna.Framework.Content;
using SimpleOrmFramework;

namespace Ale.Content
{
    public abstract class BaseAssetLoader<T> : IAssetLoader where T : class, IDataObject
    {
        public object LoadAsset(string assetName, ContentGroup contentGroup, out long id)
        {
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException("assetName"); }
            if (null == contentGroup) { throw new ArgumentNullException("contentGroup"); }

            T[] settings = contentGroup.ParentContentManager.OrmManager.LoadObjects<T>(string.Format("Name='{0}'", assetName));
            if (0 == settings.Length)
            {
                throw new ContentLoadException(string.Format("Settings (type='{0}') with name '{1}' doesn't exists", typeof(T).FullName, assetName));
            }
            id = settings[0].Id;
            return CreateDescWithCheck(settings[0], contentGroup);
        }

        public object LoadAsset(long id, ContentGroup contentGroup, out string name)
        {
            if (null == contentGroup) { throw new ArgumentNullException("contentGroup"); }

            T settings = contentGroup.ParentContentManager.OrmManager.LoadObject<T>(id);
            name = GetName(settings);
            return CreateDesc(settings, contentGroup);
        }

        private object CreateDescWithCheck(T settings, ContentGroup contentGroup)
        {
            try
            {
                return CreateDesc(settings, contentGroup);
            }
            catch (Exception ex)
            {
                throw new ContentLoadException(string.Format("Error occured during loading of '{0}' from settings '{1}'. See inner exception fro details", typeof(T).FullName, GetName(settings)), ex);
            }
        }

        protected abstract object CreateDesc(T settings, ContentGroup contentGroup);
        protected abstract string GetName(T settings);
    }
}
