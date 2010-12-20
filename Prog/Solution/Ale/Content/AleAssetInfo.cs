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
