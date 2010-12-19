using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;
using Ale.Content;
using Ale.Scene;

namespace Ale.Graphics
{
    public sealed class ParticleSystemManager : IDisposable, IFrameListener, ISceneDrawableComponent
    {
        #region Fields

        private DynamicQuadGeometryManager<ParticleVertex> mParticleDynamicGeometryManager;
        private DynamicallyLoadableObjectsDistanceUnloader mDynamicallyLoadableObjectsDistanceUnloader;
        private bool mIsDisposed = false;

        private List<ParticleSystem> mFireAndForgetParticleSystems = new List<ParticleSystem>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Main scene camera for checking psys distance and dynamic loading/unloading
        /// </summary>
        public ICamera MainCamera
        {
            get { return mDynamicallyLoadableObjectsDistanceUnloader.MainCamera; }
            set { mDynamicallyLoadableObjectsDistanceUnloader.MainCamera = value; }
        }
        
        internal DynamicQuadGeometryManager<ParticleVertex> ParticleDynamicGeometryManager
        {
            get { return mParticleDynamicGeometryManager; }
        }
                
        #endregion Properties

        public ParticleSystemManager(ParticleDynamicGeometryManager particleDynamicGeometryManager, ICamera mainCamera)
        {
            if (null == particleDynamicGeometryManager) throw new ArgumentNullException("particleDynamicGeometryManager");
            if (null == mainCamera) throw new ArgumentNullException("mainCamera");

            mParticleDynamicGeometryManager = particleDynamicGeometryManager;
            mDynamicallyLoadableObjectsDistanceUnloader = new DynamicallyLoadableObjectsDistanceUnloader(5);
            MainCamera = mainCamera;
        }

        public ParticleSystem CreateParticleSystem(ContentGroup contentGroup, string particleDescName)
        {
            if (null == contentGroup) throw new ArgumentNullException("contentGroup");
            if (string.IsNullOrEmpty(particleDescName)) throw new ArgumentNullException("particleDescName");

            return CreateParticleSystem(contentGroup.Load<ParticleSystemDesc>(particleDescName));
        }

        public ParticleSystem CreateParticleSystem(ParticleSystemDesc particleSystemDesc)
        {
            if (null == particleSystemDesc) throw new ArgumentNullException("particleSystemDesc");

            ParticleSystem particleSystem = new ParticleSystem(this, particleSystemDesc, true);
            mDynamicallyLoadableObjectsDistanceUnloader.RegisterObject(particleSystem);

            return particleSystem;
        }

        public void CreateFireAndforgetParticleSystem(ParticleSystemDesc particleSystemDesc, ref Vector3 worldPosition)
        {
            if (null == particleSystemDesc) throw new ArgumentNullException("particleSystemDesc");

            ParticleSystem particleSystem = new ParticleSystem(this, particleSystemDesc, false);
            particleSystem.Position = worldPosition;
            mFireAndForgetParticleSystems.Add(particleSystem);
        }

        public void CreateFireAndforgetParticleSystem(ParticleSystemDesc particleSystemDesc, Vector3 worldPosition)
        {
            CreateFireAndforgetParticleSystem(particleSystemDesc, ref worldPosition);
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (ParticleSystem ps in mFireAndForgetParticleSystems)
                {
                    ps.Dispose();
                }

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void EnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            if (0 != mFireAndForgetParticleSystems.Count)
            {
                //no visibility checking
                foreach (ParticleSystem pSys in mFireAndForgetParticleSystems)
                {
                    pSys.EnqueRenderableUnits(renderer, gameTime);
                }

                //remove inactive particle systems
                for (int i = mFireAndForgetParticleSystems.Count-1; i >= 0; --i)
                {
                    if (!mFireAndForgetParticleSystems[i].IsLoaded)
                    {
                        mFireAndForgetParticleSystems[i].Dispose();
                        mFireAndForgetParticleSystems.RemoveAt(i);
                    }
                }
            }
        }

        void ISceneDrawableComponent.EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ScenePass scenePass)
        {
            EnqueRenderableUnits(renderer, gameTime);
        }

        #region IFrameListener Members

        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).BeforeUpdate(gameTime);
        }

        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).AfterUpdate(gameTime);
        }

        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).BeforeRender(gameTime);
        }

        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
            ((IFrameListener)mDynamicallyLoadableObjectsDistanceUnloader).AfterRender(gameTime);
        }

        #endregion

        #region ISceneDrawableComponent Members



        #endregion
    }
}
