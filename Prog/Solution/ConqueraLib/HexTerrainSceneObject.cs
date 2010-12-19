//using System;
//using System.Collections.Generic;
//using System.Text;
//using SimpleOrmFramework;
//using Ale.Tools;
//using Microsoft.Xna.Framework;
//using Ale.Content;

//namespace Conquera
//{
//    [DataObject(MaxCachedCnt = 0)]
//    [CustomBasicTypeProvider(typeof(Point), typeof(FieldCustomBasicTypeProvider<Point>))]
//    public class HexTerrainSceneObject : OctreeSceneObject
//    {
//        public delegate void TileIndexChangedHandler(HexTerrainSceneObject obj, Point oldValue);
//        public event TileIndexChangedHandler TileIndexChanged;

//        private Point mTileIndex;

//        [DataProperty(NotNull = true)]
//        public Point TileIndex
//        {
//            get { return mTileIndex; }
//            set
//            {
//                if (mTileIndex != value)
//                {
//                    Point oldValue = mTileIndex;
//                    mTileIndex = value;

//                    if (IsInScene)
//                    {
//                        Position = HexTerrainScene.Terrain.GetPosFromIndex(mTileIndex);
//                    }

//                    OnTileIndexChanged(oldValue);

//                    if (null != TileIndexChanged)
//                    {
//                        TileIndexChanged.Invoke(this, oldValue);
//                    }
//                }
//            }
//        }

//        public HexTerrainScene HexTerrainScene
//        {
//            get{return (HexTerrainScene)Scene;}
//        }

//        public HexTerrainSceneObjectDesc HexTerrainSceneObjectDesc
//        {
//            get { return (HexTerrainSceneObjectDesc)Desc; }
//        }

//        [DataProperty(Column="Name", Unique = false, NotNull = true)] //!! Unique = false - unique in a scene but not in a whole table
//        private string NameInDb 
//        {
//            get { return Name.Name; }
//            set { Name = value; } 
//        }

//        public HexTerrainSceneObject(string name, long desc)
//            : base(name, desc)
//        {
//        }

//        private HexTerrainSceneObject()
//        {
//        }

//        protected override bool IsSceneValid(BaseScene scene)
//        {
//            return (scene is HexTerrainScene);
//        }

//        protected override void OnAddToSceneImpl(BaseScene scene)
//        {
//            Position = ((HexTerrainScene)scene).Terrain.GetPosFromIndex(mTileIndex);

//            base.OnAddToSceneImpl(scene);

//            if(null != HexTerrainSceneObjectDesc.IdleAnimation)
//            {
//                GraphicModel.AnimationPlayer.Animation = HexTerrainSceneObjectDesc.IdleAnimation;
//                GraphicModel.AnimationPlayer.Play(true);
//            }
//        }

//        protected virtual void OnTileIndexChanged(Point oldValue)
//        {
//        }
//    }




//    [DataObject(MaxCachedCnt = 5)]
//    public class HexTerrainSceneObjectSettings : OctreeSceneObjectSettings
//    {
//        [DataProperty]
//        public bool IsPassable { get; set; }

//        [DataProperty]
//        public string IdleAnimation { get; set; }
//    }

//    public class HexTerrainSceneObjectDesc : OctreeSceneObjectDesc
//    {
//        public bool IsPassable { get; private set; }
//        public NameId IdleAnimation { get; private set; }

//        public HexTerrainSceneObjectDesc(HexTerrainSceneObjectSettings settings, ContentGroup content)
//            :base(settings, content)
//        {
//            IsPassable = settings.IsPassable;

//            if (!string.IsNullOrEmpty(settings.IdleAnimation) && null == GraphicModel.Mesh.SkeletalAnimations[settings.IdleAnimation])
//            {
//                throw new ArgumentNullException(string.Format("Animation '{0}' doesn't exists in the mesh of the '{1}' scene object", settings.IdleAnimation, settings.Name));
//            }
//            IdleAnimation = settings.IdleAnimation;
//        }
//    }



//    class HexTerrainSceneObjectCollection : IList<HexTerrainSceneObject> //IList for Sof
//    {
//        //Dictionary<NameId, Point

//        #region IList<HexTerrainSceneObject> Members

//        int IList<HexTerrainSceneObject>.IndexOf(HexTerrainSceneObject item)
//        {
//            throw new NotImplementedException();
//        }

//        void IList<HexTerrainSceneObject>.Insert(int index, HexTerrainSceneObject item)
//        {
//            throw new NotImplementedException();
//        }

//        void IList<HexTerrainSceneObject>.RemoveAt(int index)
//        {
//            throw new NotImplementedException();
//        }

//        HexTerrainSceneObject IList<HexTerrainSceneObject>.this[int index]
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        #endregion

//        #region ICollection<HexTerrainSceneObject> Members

//        void ICollection<HexTerrainSceneObject>.Add(HexTerrainSceneObject item)
//        {
//            throw new NotImplementedException();
//        }

//        void ICollection<HexTerrainSceneObject>.Clear()
//        {
//            throw new NotImplementedException();
//        }

//        bool ICollection<HexTerrainSceneObject>.Contains(HexTerrainSceneObject item)
//        {
//            throw new NotImplementedException();
//        }

//        void ICollection<HexTerrainSceneObject>.CopyTo(HexTerrainSceneObject[] array, int arrayIndex)
//        {
//            throw new NotImplementedException();
//        }

//        int ICollection<HexTerrainSceneObject>.Count
//        {
//            get { throw new NotImplementedException(); }
//        }

//        bool ICollection<HexTerrainSceneObject>.IsReadOnly
//        {
//            get { throw new NotImplementedException(); }
//        }

//        bool ICollection<HexTerrainSceneObject>.Remove(HexTerrainSceneObject item)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        #region IEnumerable<HexTerrainSceneObject> Members

//        IEnumerator<HexTerrainSceneObject> IEnumerable<HexTerrainSceneObject>.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        #region IEnumerable Members

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }


//}
