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
