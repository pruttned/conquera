using System;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using SimpleOrmFramework;
using Ale.Tools;
using Ale.Content;

namespace Ale.Scene
{
    public abstract class SceneObject : IDisposable
    {
        private BaseScene mScene;
        private bool mIsDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException">Not yet in a scene</exception>
        public BaseScene Scene
        {
            get 
            {
                IsInSceneCheck();
                return mScene; 
            }
        }

        public bool IsInScene
        {
            get { return null != mScene; }
        }

        /// <summary>
        /// Must be called from a scene class after the object has been added to this scene
        /// </summary>
        /// <param name="scene"></param>
        public void OnAddToScene(BaseScene scene)
        {
            if (IsInScene)
            {
                throw new InvalidOperationException("Object was already added to a scene");
            }
            if (!IsSceneValid(scene))
            {
                throw new ArgumentException(string.Format("Scene (type='{0}') is not valid for this object (type='{1}')", scene.GetType().FullName, this.GetType().FullName));
            }
            mScene = scene;

            OnAddToSceneImpl(scene);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    OnDispose();
                }
                mIsDisposed = true;
            }
        }

        protected void IsInSceneCheck()
        {
            if (!IsInScene) throw new InvalidOperationException("Object has not yet been inserted to a scene");
        }

        protected abstract void OnAddToSceneImpl(BaseScene scene);
        protected abstract void OnDispose();
        protected abstract bool IsSceneValid(BaseScene scene);
    }
}
