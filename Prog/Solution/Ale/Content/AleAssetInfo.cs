using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ale.Content
{
    [DebuggerDisplay("DisplayName = {DisplayName}")]
    public class AleAssetDirectoryInfo
    {
        private string mDisplayName;
        private AleAssetDirectoryInfo mParentDirectory;
        private List<AleAssetDirectoryInfo> mSubDirectories = new List<AleAssetDirectoryInfo>();
        private List<AleAssetInfo> mAssets = new List<AleAssetInfo>();

        public string DisplayName
        {
            get { return mDisplayName; }
        }

        public AleAssetDirectoryInfo ParentDirectory
        {
            get { return mParentDirectory; }
        }

        public List<AleAssetDirectoryInfo> SubDirectories
        {
            get { return mSubDirectories; }
        }

        public List<AleAssetInfo> Assets
        {
            get { return mAssets; }
        }

        public AleAssetDirectoryInfo(string displayName, AleAssetDirectoryInfo parentDirectory)
        {
            mDisplayName = displayName;
            mParentDirectory = parentDirectory;
        }
    }

    [DebuggerDisplay("Name = {Name}")]
    public class AleAssetInfo
    {
        public enum AssetStorageTypes { NonCompressedFile, CompressedFile, DataBase };

        private string mName;
        private string mShortName;
        private AssetStorageTypes mStorageType;
        private Type mType;
        private AleAssetDirectoryInfo mDirectory;

        public string Name
        {
            get { return mName; }
        }

        public string ShortName
        {
            get { return mShortName; }
        }

        public AssetStorageTypes StorageType
        {
            get { return mStorageType; }
        }

        public Type Type
        {
            get { return mType; }
        }

        public AleAssetDirectoryInfo Directory
        {
            get { return mDirectory; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Asset name - file name relative to the asset type root directory and without the extension.</param>
        /// <param name="storageType"></param>
        /// <param name="type"></param>
        /// <param name="directory"></param>
        public AleAssetInfo(string name, AssetStorageTypes storageType, Type type, AleAssetDirectoryInfo directory)
        {
            mName = name;
            mShortName = Path.GetFileName(name);
            mStorageType = storageType;
            mType = type;
            mDirectory = directory;
        }
    }
}
