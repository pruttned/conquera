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
