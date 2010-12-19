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
using Ale.Tools;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{

    public class MeshConnectionPoint
    {
        private NameId mName;

        private Matrix mLocTransf;

        /// <summary>
        /// If null, then the mesh is parent of this connection point
        /// </summary>
        private SkeletonBone mParentBone;

        public NameId Name
        {
            get { return mName; }
        }

        public Matrix LocTransf
        {
            get { return mLocTransf; }
        }

        public Vector3 LocPosition { get; private set; }
        public Quaternion LocOrientation { get; private set; }

        /// <summary>
        /// If null, then the mesh is parent of this connection point
        /// </summary>
        public SkeletonBone ParentBone
        {
            get { return mParentBone; }
        }

        public bool HasParentBone
        {
            get { return null != mParentBone; }
        }

        internal MeshConnectionPoint(ContentReader input, Mesh mesh)
        {
            mName = input.ReadString();
            bool hasParentBone = input.ReadBoolean();
            if (hasParentBone)
            {
                NameId parentBoneName = input.ReadString();
                mParentBone = GetBoneByName(parentBoneName, mesh);
            }
            else
            {
                mParentBone = null;
            }
            Vector3 position = input.ReadVector3();
            Quaternion orientation = input.ReadQuaternion();

            LocPosition = position;
            LocOrientation = orientation;

            AleMathUtils.BuildTransformation(ref position, ref orientation, out mLocTransf);
        }

        internal MeshConnectionPoint(NameId name, NameId parentBoneName, Vector3 position, Quaternion orientation, Mesh mesh)
        {
            mName = name;
            if (null != parentBoneName)
            {
                mParentBone = GetBoneByName(parentBoneName, mesh);
            }
            else
            {
                mParentBone = null;
            }

            LocPosition = position;
            LocOrientation = orientation;

            AleMathUtils.BuildTransformation(ref position, ref orientation, out mLocTransf);
        }

        public override string ToString()
        {
            return mName.Name;
        }

        private SkeletonBone GetBoneByName(NameId parentBoneName, Mesh mesh)
        {
            if (null == mesh.Skeleton)
            {
                throw new ArgumentException("Mesh has no skeleton");
            }

            int boneIndex = mesh.Skeleton.FindBoneByName(parentBoneName);
            if (0 > boneIndex)
            {
                throw new ArgumentException(string.Format("Bone '{0}' couldn't be found", parentBoneName));
            }
            return mesh.Skeleton[boneIndex];
        }
    }

    public class MeshConnectionPointCollection : ReadOnlyDictionary<NameId, MeshConnectionPoint>
    {
        Dictionary<NameId, MeshConnectionPoint> mConnectionPoints;

        internal MeshConnectionPointCollection(Dictionary<NameId, MeshConnectionPoint> connectionPoints)
            :base(connectionPoints)
        {
            mConnectionPoints = connectionPoints;
        }

        internal MeshConnectionPointCollection()
            : this(new Dictionary<NameId, MeshConnectionPoint>())
        {
        }

        internal static MeshConnectionPointCollection Read(ContentReader input, Mesh mesh)
        {
            int cnt = input.ReadInt32();
            if (0 == cnt)
            {
                return null;
            }

            MeshConnectionPointCollection connectionPoints = new MeshConnectionPointCollection();

            for (int i = 0; i < cnt; ++i)
            {
                connectionPoints.Add(new MeshConnectionPoint(input, mesh));
            }

            return connectionPoints;
        }

        internal void Add(MeshConnectionPoint connectionPoint)
        {
            mConnectionPoints.Add(connectionPoint.Name, connectionPoint);
        }
    }
}
