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
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;
using Ale.Tools;
using System.Collections.ObjectModel;

namespace Ale.Scene
{
    public abstract class OctreeScene : BaseScene
    {
        private OctreeSceneDrawableComponent mOctree;
        private StaticGeometry mStaticGeomtery;
        private List<OctreeSceneObject> mSceneObjects = new List<OctreeSceneObject>();
        private ReadOnlyCollection<OctreeSceneObject> mReadonlySceneObjects;
        private bool mIsUpdatingObjects = false;
        private int mObjectsToDeleteCnt = 0;

        public Octree Octree
        {
            get { return mOctree; }
        }

        public StaticGeometry StaticGeomtery
        {
            get { return mStaticGeomtery; }
        }

        public ReadOnlyCollection<OctreeSceneObject> SceneObjects
        {
            get { return mReadonlySceneObjects; }
        }

        /// <summary>
        /// Gets the statistic of a last executed octree query in EnqueRenderableUnits method
        /// </summary>
        public OctreeFilterStatistics SceneQueuingStatistics
        {
            get { return mOctree.SceneQueuingStatistics; }
        }

        public OctreeScene(SceneManager sceneManager, ContentGroup content, BoundingBox sceneBounds)
            : base(sceneManager, content)
        {
            mOctree = new OctreeSceneDrawableComponent(sceneBounds);
            SceneDrawableComponents.Add(mOctree);

            mReadonlySceneObjects = new ReadOnlyCollection<OctreeSceneObject>(mSceneObjects);

            mStaticGeomtery = new StaticGeometry(SceneManager.GraphicsDeviceManager, Octree, MainCamera, 20);
            RegisterFrameListener(mStaticGeomtery);
        }

        public override void Update(AleGameTime gameTime)
        {
            mIsUpdatingObjects = true;
            for (int i = mSceneObjects.Count - 1; i >= 0; --i)
            {
                var obj = mSceneObjects[i];
                if (null != obj)
                {
                    obj.Update(gameTime);
                }
            }
            mIsUpdatingObjects = false;

            if (0 < mObjectsToDeleteCnt)
            {
                for (int i = mSceneObjects.Count - 1; mObjectsToDeleteCnt > 0; --i)
                {
                    if (null == mSceneObjects[i])
                    {
                        mSceneObjects.RemoveAt(i);
                        mObjectsToDeleteCnt--;
                    }
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                mStaticGeomtery.Dispose();

                foreach (OctreeSceneObject sceneObject in mSceneObjects)
                {
                    if (null != sceneObject) //shouldn't happen
                    {
                        sceneObject.Dispose();
                    }
                }
            }

            base.Dispose(isDisposing);
        }

        protected void AddSceneObject(OctreeSceneObject sceneObject)
        {
            if (null == sceneObject) throw new ArgumentNullException("sceneObject");

            mSceneObjects.Add(sceneObject);
            sceneObject.OnAddToScene(this);
        }

        protected bool DestroySceneObject(OctreeSceneObject sceneObject)
        {
            if (null == sceneObject) throw new ArgumentNullException("sceneObject");

            if (mIsUpdatingObjects)
            {
                int index = mSceneObjects.IndexOf(sceneObject);
                if (-1 < index)
                {
                    sceneObject.Dispose();
                    mObjectsToDeleteCnt++;
                    mSceneObjects[index] = null;
                    return true;
                }
            }
            else
            {
                if (mSceneObjects.Remove(sceneObject))
                {
                    sceneObject.Dispose();
                    return true;
                }
            }
            return false;
        }
    }
}
