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
using System.IO;
using Ionic.Zip;
using Microsoft.Xna.Framework.Content;
using SimpleOrmFramework;

namespace Ale.Content
{
    /// <summary>
    /// Content manager.
    /// Assets are associated with conten groups. Content group can be destroyed which means that assets associated with a group will be disposed if
    /// they are not associated also with another content groups.
    /// </summary>
    public class AleContentManager : ContentManager
    {
        #region Types

        /// <summary>
        /// 
        /// </summary>
        class PackedAssetDesc
        {
            /// <summary>
            /// 
            /// </summary>
            public string ApFile;

            /// <summary>
            /// 
            /// </summary>
            public string PathInApFile;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="apFile"></param>
            /// <param name="pathInApFile"></param>
            public PackedAssetDesc(string apFile, string pathInApFile)
            {
                ApFile = apFile;
                PathInApFile = pathInApFile;
            }
        }

        #endregion Types

        #region Fields

        Dictionary<string, AleAsset> mAssets = new Dictionary<string, AleAsset>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Only non-content pipeline objects
        /// </summary>
        private Dictionary<long, AleAsset> mDataObjectAssets = new Dictionary<long, AleAsset>();
        Dictionary<string, ContentGroup> mContentGroups = new Dictionary<string, ContentGroup>(StringComparer.OrdinalIgnoreCase);
        private OrmManager mOrmManager;
        ContentGroup mDefaultContentGroup;
        private string mContentRootDirectory;
        Dictionary<string, PackedAssetDesc> mPackedAssets = new Dictionary<string, PackedAssetDesc>(StringComparer.OrdinalIgnoreCase);
        Dictionary<Type, IAssetLoader> mAssetLoaders = new Dictionary<Type,IAssetLoader>();
        Stack<AleAsset> mAssetLoadingStack = new Stack<AleAsset>();

        /// <summary>
        /// (asset type x directory for assets of that type)
        /// </summary>
        private Dictionary<Type, string> mAssetTypeDirectories = new Dictionary<Type, string>();

        #endregion Fields

        #region Properties

        public string ModFile { get; private set; }
        
        /// <summary>
        /// Gets the default content group
        /// </summary>
        public ContentGroup DefaultContentGroup
        {
            get { return mDefaultContentGroup; }
        }

        /// <summary>
        /// Gets the orm manager for accesssing the db game storage
        /// </summary>
        public OrmManager OrmManager
        {
            get { return mOrmManager; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="modFile"></param>
        /// <param name="contentRootDirectory"></param>
        public AleContentManager(IServiceProvider serviceProvider, string modFile, string contentRootDirectory)
            : base(serviceProvider, contentRootDirectory)
        {
            if (string.IsNullOrEmpty(modFile)) { throw new ArgumentNullException("modFile"); }
            if (string.IsNullOrEmpty(contentRootDirectory)) { throw new ArgumentNullException("contentRootDirectory"); }

            ModFile = modFile;
            mContentRootDirectory = RootDirectory;
            mOrmManager = new OrmManager(OrmManager.CreateDefaultConnectionString(modFile));
            mDefaultContentGroup = new ContentGroup(serviceProvider, this, "default");
            mContentGroups.Add(mDefaultContentGroup.Name, mDefaultContentGroup);

            CahcePackagedFiles(RootDirectory, "");
        }

        /// <summary>
        /// Registers a directory under content root directory for the specified asset type.
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetTypeDirectory">Path relative to content root directory.</param>
        public void RegisterAssetTypeDirectory(Type assetType, string assetTypeDirectory)
        {
            if (string.IsNullOrEmpty(assetTypeDirectory))
            {
                throw new ArgumentException("Argument 'assetTypeDirectory' cannot be null or empty.");
            }

            assetTypeDirectory = GetCleanPath(assetTypeDirectory.Trim(' ', '\\', '/'));
            mAssetTypeDirectories.Add(assetType, assetTypeDirectory);
        }

        /// <summary>
        /// Gets directory under content root directory for the specified asset type. Returned path is relative to content root directory.
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public string GetAssetTypeDirectory(Type assetType)
        {
            string directory;
            if (mAssetTypeDirectories.TryGetValue(assetType, out directory))
            {
                return directory;
            }
            return string.Empty; //root directory; there is no special directory for the specified asset type
        }

        /// <summary>
        /// Unloads a given asset
        /// </summary>
        /// <param name="name"></param>
        internal void UnloadAsset(string assetName)
        {
            AleAsset asset;
            if (mAssets.TryGetValue(assetName, out asset))
            {
                if (0 == asset.DecRefCnt())
                {
                    mAssets.Remove(assetName);
                    if (asset.RealAsset is IDisposable)
                    {
                        ((IDisposable)asset.RealAsset).Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the content group by its name. It will be created if doesn't exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ContentGroup GetContentGroup(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            ContentGroup contentGroup;
            if (!mContentGroups.TryGetValue(name, out contentGroup))
            {
                contentGroup = new ContentGroup(ServiceProvider, this, name);
                mContentGroups.Add(name, contentGroup);
            }
            return contentGroup;
        }

        /// <summary>
        /// Destroy a given content group. All assets associated with this grop will be disposed if they are not associated with other content groups.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException">- Content group with a given name desn't existst</exception>
        public void DestroyContentGroup(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            ContentGroup contentGroup;
            if (!mContentGroups.TryGetValue(name, out contentGroup))
            {
                throw new ArgumentException(string.Format("Content group with name '{0}' desn't existst"), name);
            }

            contentGroup.Dispose();
            mContentGroups.Remove(name);
        }

        /// <summary>
        /// Loads an asset to the default content group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T Load<T>(string assetName)
        {
            return DefaultContentGroup.Load<T>(assetName);
        }

        /// <summary>
        /// Loads an asset to the default content group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T Load<T>(long id)
        {
            return DefaultContentGroup.Load<T>(id);
        }

        /// <summary>
        /// Gets asset infos in a hierarchy matching directory structure.
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public AleAssetDirectoryInfo GetAssetInfos(Type assetType)
        {
            string assetTypeRootPath = GetAssetTypeDirectory(assetType);
            if (string.IsNullOrEmpty(assetTypeRootPath))
            {
                return null;
            }
            
            //TODO: find assets in DB

            Dictionary<string, AleAssetDirectoryInfo> mAssetDirecotries = new Dictionary<string, AleAssetDirectoryInfo>();
            AleAssetDirectoryInfo assetTypeRootDirectory = new AleAssetDirectoryInfo(assetTypeRootPath, null);
            mAssetDirecotries.Add(string.Empty, assetTypeRootDirectory);

            FindNonPackedAssets(assetTypeRootPath, mAssetDirecotries, assetType);
            FindPackedAssets(assetTypeRootPath, mAssetDirecotries, assetType);

            return assetTypeRootDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="contentGroup"></param>
        /// <returns></returns>
        internal AleAsset LoadInter<T>(string assetName, ContentGroup contentGroup)
        {
            CheckRootDirectory();

            //assetName = GetCleanPath(assetName); already called in ContenGroup
            AleAsset asset;
            if (mAssets.TryGetValue(assetName, out asset))
            {
                if (!(asset.RealAsset is T))
                {
                    throw new ContentLoadException(string.Format("Error loading '{0}'. Asset is '{1}' but trying to load as '{2}'.", assetName, asset.RealAsset.GetType(), typeof(T)));
                }
                asset.IncRefCnt(); //inc ref cnt for asset (and also its childs)
            }
            else
            {
                asset = new AleAsset();
                
                if(0 != mAssetLoadingStack.Count)
                {
                    mAssetLoadingStack.Peek().AddChildAsset(asset);
                }
                
                mAssetLoadingStack.Push(asset);

                //check if it is not an non content pipeline asset
                IAssetLoader assetLoader = GetAssetLoader(typeof(T));

                T realAsset;
                long id = -1;
                if (null != assetLoader) //non content pipeline asset
                {
                    realAsset = (T)assetLoader.LoadAsset(assetName, contentGroup, out id);
                }
                else //content pipeline asset
                {
                    //it is neceseary to call ReadAsset on content group so all sub-assets will be also loaded in this group
                    realAsset = contentGroup.ReadAssetInter<T>(assetName);
                }

                mAssetLoadingStack.Pop();
                asset.Init(assetName, realAsset, id);
                mAssets.Add(assetName, asset);

                if (-1 != id)
                {
                    mDataObjectAssets.Add(id, asset);
                }
            }

            return asset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="contentGroup"></param>
        /// <returns></returns>
        internal AleAsset LoadInter<T>(long id, ContentGroup contentGroup)
        {
            CheckRootDirectory();

            //assetName = GetCleanPath(assetName); already called in ContenGroup
            AleAsset asset;
            if (mDataObjectAssets.TryGetValue(id, out asset))
            {
                if (!(asset.RealAsset is T))
                {
                    throw new ContentLoadException(string.Format("Error loading id='{0}'. Asset is '{1}' but trying to load as '{2}'.", id, asset.RealAsset.GetType(), typeof(T)));
                }

                asset.IncRefCnt(); //inc ref cnt for asset (and also its childs)
            }
            else
            {
                //check if it is not an non content pipeline asset
                IAssetLoader assetLoader = GetAssetLoader(typeof(T));
                if(null == assetLoader)
                {
                    throw new InvalidOperationException(string.Format("Asset Type='{0}' must have IAssetLoader in order to load it by its Id", typeof(T).FullName));
                }

                asset = new AleAsset();

                if (0 != mAssetLoadingStack.Count)
                {
                    mAssetLoadingStack.Peek().AddChildAsset(asset);
                }

                mAssetLoadingStack.Push(asset);

                T realAsset;
                string assetName;

                realAsset = (T)assetLoader.LoadAsset(id, contentGroup, out assetName);

                mAssetLoadingStack.Pop();
                asset.Init(assetName, realAsset, id);
                mAssets.Add(assetName, asset);
                mDataObjectAssets.Add(id, asset);
            }

            return asset;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        internal Stream OpenAssetStream(string assetName)
        {
            //asset name is already cleaned

            string assetNameWithXnb = assetName + ".xnb";
            string assetFile = Path.Combine(RootDirectory, assetNameWithXnb);
            try
            {
                if (File.Exists(assetFile))
                {
                    return new FileStream(assetFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                else //search in ap files
                {
                    PackedAssetDesc apFile;
                    if (mPackedAssets.TryGetValue(assetNameWithXnb, out apFile))
                    {
                        using (ZipFile zipFile = ZipFile.Read(apFile.ApFile))
                        {
                            ZipEntry zipEntry = zipFile[apFile.PathInApFile];
                            if(null == zipEntry)
                            {
                                throw new KeyNotFoundException(string.Format("File '{0}' doesn't exists in '{1}'", apFile.PathInApFile, apFile.ApFile));
                            }
                            MemoryStream stream = new MemoryStream((int)zipEntry.UncompressedSize);
                            zipEntry.Extract(stream);
                            stream.Position = 0;
                            return stream;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ContentLoadException(string.Format("Exception has occured during loading of the asset '{0}'. See inner exception for details", assetName), ex);
            }

            throw new ContentLoadException(string.Format("Asset '{0}' could not be found", assetName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected internal static string GetCleanPath(string path)
        {
            int num2;
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            for (int i = 1; i < path.Length; i = Math.Max(num2 - 1, 1))
            {
                i = path.IndexOf(@"\..\", i);
                if (i < 0)
                {
                    return path;
                }
                num2 = path.LastIndexOf(Path.DirectorySeparatorChar, i - 1) + 1;
                path = path.Remove(num2, (i - num2) + @"\..\".Length);
            }
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        protected override Stream OpenStream(string assetName)
        {
            throw new NotImplementedException("This shouldn't  be called");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //todo optimize
                foreach (ContentGroup contentGroup in mContentGroups.Values)
                {
                    contentGroup.Dispose();
                }
            }
            mDefaultContentGroup = null;
            mContentGroups.Clear();
            mOrmManager.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteDir"></param>
        /// <param name="relativeToRoot"></param>
        private void CahcePackagedFiles(string absoluteDir, string relativeToRoot)
        {
            //find ap (asset package) files
            foreach (string file in Directory.GetFiles(absoluteDir, "*.ap", SearchOption.TopDirectoryOnly))
            {
                //Directory.GetFiles will return all files whose extension starts with 'ap'
                if (string.Equals(Path.GetExtension(file), ".ap", StringComparison.OrdinalIgnoreCase))
                {
                    using (ZipFile apFile = ZipFile.Read(file))
                    {
                        foreach (ZipEntry zipEntry in apFile)
                        {
                            if (!zipEntry.IsDirectory)
                            {
                                string assetFile = GetCleanPath(Path.Combine(relativeToRoot, zipEntry.FileName));

                                try
                                {
                                    mPackedAssets.Add(assetFile, new PackedAssetDesc(file, zipEntry.FileName));
                                }
                                catch (ArgumentException)
                                {
                                    throw new ContentLoadException(string.Format("Asset '{0}' in package '{1}' already exists in package '{2}'", assetFile, file, mPackedAssets[assetFile]));
                                }
                            }
                        }
                    }
                }
            }

            foreach (string subDir in Directory.GetDirectories(absoluteDir))
            {
                CahcePackagedFiles(Path.Combine(absoluteDir, subDir), Path.Combine(relativeToRoot, Path.GetFileName(subDir)));
            }
        }

        private void CheckRootDirectory()
        {
            if (!object.ReferenceEquals(mContentRootDirectory, RootDirectory))
            {
                throw new InvalidOperationException("Don't change RootDirectory of the content manager. It is not supported. RootDirectory set property is public only because it is public in the base xna class");
            }
        }

        private IAssetLoader GetAssetLoader(Type assetType)
        {
            IAssetLoader assetLoader;
            if (!mAssetLoaders.TryGetValue(assetType, out assetLoader))
            {
                object[] nonContentPipelineAssetAttributes = assetType.GetCustomAttributes(typeof(NonContentPipelineAssetAttribute), false);
                if (null != nonContentPipelineAssetAttributes && 0 < nonContentPipelineAssetAttributes.Length)
                {
                    NonContentPipelineAssetAttribute nonContentPipelineAssetAttribute = (NonContentPipelineAssetAttribute)nonContentPipelineAssetAttributes[0];
                    assetLoader = (IAssetLoader)Activator.CreateInstance(nonContentPipelineAssetAttribute.AssetLoader);
                    mAssetLoaders.Add(assetType, assetLoader);
                }
                else //add null as a asset loader to mark it as a content pipeline based asset
                {
                    mAssetLoaders.Add(assetType, null);
                }
            }
            return assetLoader;
        }

        private void FindNonPackedAssets(string assetTypeRootPath, Dictionary<string, AleAssetDirectoryInfo> assetDirecotries, Type assetType)
        {
            string assetTypeRootAbsolutePath = Path.Combine(mContentRootDirectory, assetTypeRootPath) + Path.DirectorySeparatorChar;
            int assetTypeRootAbsolutePathLength = assetTypeRootAbsolutePath.Length;

            foreach (string fullFileName in Directory.GetFiles(assetTypeRootAbsolutePath, "*.xnb", SearchOption.AllDirectories))
            {
                string assetName = fullFileName.Substring(assetTypeRootAbsolutePathLength); //asset name will be relative to asset type root directory
                assetName = assetName.Substring(0, assetName.Length - 4); //removing the '.xnb' extension
                AddAssetInfo(assetName, assetDirecotries, assetType, AleAssetInfo.AssetStorageTypes.NonCompressedFile);
            }
        }

        private void FindPackedAssets(string assetTypeRootPath, Dictionary<string, AleAssetDirectoryInfo> assetDirecotries, Type assetType)
        {
            foreach (string relativeFileName in mPackedAssets.Keys) //'relativeFileName' - relative to content root directory
            {
                if (relativeFileName.StartsWith(assetTypeRootPath))
                {
                    string assetName = relativeFileName.Substring((assetTypeRootPath + Path.DirectorySeparatorChar).Length);  //asset name will be relative to asset type root directory
                    assetName = assetName.Substring(0, assetName.Length - 4); //removing the '.xnb' extension
                    AddAssetInfo(assetName, assetDirecotries, assetType, AleAssetInfo.AssetStorageTypes.CompressedFile);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName">Asset file name relative to asset type root directory and without the '.xnb' extension.</param>
        /// <param name="assetDirecotries"></param>
        /// <param name="assetType"></param>
        /// <param name="assetLocationType"></param>
        private void AddAssetInfo(string assetName, Dictionary<string, AleAssetDirectoryInfo> assetDirecotries, Type assetType,
            AleAssetInfo.AssetStorageTypes assetStorageType)
        {
            AleAssetDirectoryInfo directory = GetCreateAssetDirectoryInfo(Path.GetDirectoryName(assetName), assetDirecotries);
            directory.Assets.Add(new AleAssetInfo(assetName, assetStorageType, assetType, directory));
        }

        private AleAssetDirectoryInfo GetCreateAssetDirectoryInfo(string relativeName, Dictionary<string, AleAssetDirectoryInfo> assetDirecotries)
        {
            AleAssetDirectoryInfo directory;
            if(assetDirecotries.TryGetValue(relativeName, out directory))
            {
                return directory;
            }

            AleAssetDirectoryInfo parentDirectory = GetCreateAssetDirectoryInfo(Path.GetDirectoryName(relativeName), assetDirecotries);
            directory = new AleAssetDirectoryInfo(Path.GetFileName(relativeName), parentDirectory);
            parentDirectory.SubDirectories.Add(directory);
            assetDirecotries.Add(relativeName, directory);

            return directory;
        }

        #endregion Methods
    }
}
