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
using Microsoft.Xna.Framework.Content;
using System.IO;
using SimpleOrmFramework;

namespace Ale.Content
{
    /// <summary>
    /// Content group. 
    /// Groups assets so they can be unloaded if they are not neceseary any more. 
    /// For example it is possible to create conten group for a game scene and if it is unloaded, all assets that are not associated also with other groups are disposed.
    /// RootDirectory is ignored  - RootDirectory in AleContentManager is used
    /// </summary>
    public class ContentGroup : ContentManager
    {
        #region Fields

        private string mName;
        private AleContentManager mParentContentManager;
        private Dictionary<string, object> mAssets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Only for non-content pipeline objects
        /// </summary>
        private Dictionary<long, object> mDataObjectAssets = new Dictionary<long, object>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the name of the group
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the parent content manager
        /// </summary>
        public AleContentManager ParentContentManager
        {
            get { return mParentContentManager; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="contentManager"></param>
        /// <param name="contentRootDirectory"></param>
        /// <param name="name"></param>
        public ContentGroup(IServiceProvider serviceProvider, AleContentManager contentManager, string name)
            : base(serviceProvider)
        {
            if (null == contentManager) { throw new ArgumentNullException("contentManager"); }
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            mParentContentManager = contentManager;
            mName = name;
        }

        /// <summary>
        /// Loads an asset to this group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException("assetName"); }

            assetName = Path.Combine(ParentContentManager.GetAssetTypeDirectory(typeof(T)), AleContentManager.GetCleanPath(assetName));

            object asset;
            if (mAssets.TryGetValue(assetName, out asset))
            {
                if (!(asset is T))
                {
                    throw new ContentLoadException(string.Format("Error loading '{0}'. Asset is '{1}' but trying to load as '{2}'.", assetName, asset.GetType(), typeof(T)));
                }
            }
            else
            {
                AleAsset aleAsset = mParentContentManager.LoadInter<T>(assetName, this);
                asset = aleAsset.RealAsset;
                AddChilds(aleAsset);

                mAssets.Add(assetName, asset);

                if (-1 != aleAsset.Id)
                {
                    mDataObjectAssets.Add(aleAsset.Id, asset);
                }

            }

            return (T)asset;
        }

        /// <summary>
        /// Loads an asset to this group (only for non-contentpipeline assets)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public T Load<T>(long id)
        {
            object asset;
            if (mDataObjectAssets.TryGetValue(id, out asset))
            {
                if (!(asset is T))
                {
                    throw new ContentLoadException(string.Format("Error loading id='{0}'. Asset is '{1}' but trying to load as '{2}'.", id, asset.GetType(), typeof(T)));
                }
            }
            else
            {
                AleAsset aleAsset = mParentContentManager.LoadInter<T>(id, this);
                asset = aleAsset.RealAsset;
                AddChilds(aleAsset);

                mAssets.Add(aleAsset.Name, asset);
                mDataObjectAssets.Add(aleAsset.Id, asset);
            }
            return (T)asset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        internal T ReadAssetInter<T>(string assetName)
        {
            return ReadAsset<T>(assetName, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (string asssetName in mAssets.Keys)
                {
                    mParentContentManager.UnloadAsset(asssetName);
                }
                mAssets.Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        protected override Stream OpenStream(string assetName)
        {
            return mParentContentManager.OpenAssetStream(assetName);
        }

        private void AddChilds(AleAsset aleAsset)
        {
            if (null != aleAsset.Childs)
            {
                foreach (AleAsset childAsset in aleAsset.Childs)
                {
                    object existingChildAsset;
                    if (mAssets.TryGetValue(childAsset.Name, out existingChildAsset))
                    {
                        if (existingChildAsset != childAsset.RealAsset)
                        {
                            throw new InvalidOperationException("Fatal error : existingChildAsset != childAsset.RealAsset");
                        }
                    }
                    else
                    {
                        childAsset.IncRefCnt();
                        mAssets.Add(childAsset.Name, childAsset.RealAsset);
                    }
                }
            }
        }

        #endregion Methods
    }
}
