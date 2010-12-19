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
