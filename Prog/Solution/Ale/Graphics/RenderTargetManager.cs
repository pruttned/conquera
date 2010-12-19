﻿using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public sealed class RenderTargetManager : IDisposable
    {
        private Dictionary<NameId, AleRenderTarget> mRenderTargets = new Dictionary<NameId, AleRenderTarget>();
        private GraphicsDeviceManager mGraphicsDeviceManager;

        private bool mIsDisposed = false;

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return mGraphicsDeviceManager; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Rt or Null</returns>
        public AleRenderTarget this[string name]
        {
            get { return this[(NameId)name]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Rt or Null</returns>
        public AleRenderTarget this[NameId name]
        {
            get 
            { 
                AleRenderTarget rt = null;
                mRenderTargets.TryGetValue(name, out rt);
                return rt;
            }
        }

        public RenderTargetManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            if (null == graphicsDeviceManager) throw new ArgumentNullException("graphicsDeviceManager");

            mGraphicsDeviceManager = graphicsDeviceManager;
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
            SurfaceFormat format, DepthFormat depthFormat, MultiSampleType multiSampleType, int multiSampleQuality, RenderTargetUsage usage)
        {
            if(null == name) throw new ArgumentNullException("name");

            AleRenderTarget rt = null;
            try
            {
                if (mRenderTargets.ContainsKey(name))
                {
                    throw new ArgumentException(string.Format("Render targtet with name '{0}' already exists", name));
                }
                rt = new AleRenderTarget(mGraphicsDeviceManager, name, width, height, numberLevels, format,depthFormat,  multiSampleType, multiSampleQuality, usage);
                mRenderTargets.Add(name, rt);

                return rt;
            }
            catch
            {
                if (null != rt)
                {
                    rt.Dispose();
                }
                throw;
            }
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
          SurfaceFormat format, MultiSampleType multiSampleType, int multiSampleQuality, RenderTargetUsage usage)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, DepthFormat.Unknown, multiSampleType,
                multiSampleQuality, usage);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
            SurfaceFormat format, DepthFormat depthFormat, MultiSampleType multiSampleType, int multiSampleQuality)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, depthFormat, multiSampleType,
                multiSampleQuality, RenderTargetUsage.DiscardContents);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
          SurfaceFormat format, MultiSampleType multiSampleType, int multiSampleQuality)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, DepthFormat.Unknown, multiSampleType,
                multiSampleQuality);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
            SurfaceFormat format, DepthFormat depthFormat, RenderTargetUsage usage)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, depthFormat, MultiSampleType.None, 0, usage);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels,
        SurfaceFormat format, RenderTargetUsage usage)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, DepthFormat.Unknown, usage);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, depthFormat, RenderTargetUsage.DiscardContents);
        }

        public AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format)
        {
            return CreateRenderTarget(name, width, height, numberLevels, format, DepthFormat.Unknown);
        }

        public AleRenderTarget GetRenderTarget(NameId name)
        {
            if (null == name) throw new ArgumentNullException("name");
            return mRenderTargets[name];
        }

        public bool DestroyRenderTarget(NameId name)
        {
            if(null == name) throw new ArgumentNullException("name");
            
           Tracer.WriteInfo("Destroying render targete '{0}'", name);

            AleRenderTarget rt;
            if (mRenderTargets.TryGetValue(name, out rt))
            {
                rt.Dispose();
                mRenderTargets.Remove(name);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (AleRenderTarget rt in mRenderTargets.Values)
                {
                    rt.Dispose();
                }

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }
    }
}
