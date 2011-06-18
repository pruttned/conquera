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
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public interface IRenderTargetManager : IDisposable
    {
        GraphicsDeviceManager GraphicsDeviceManager { get; }
        AleRenderTarget this[NameId item] { get; }

        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat, MultiSampleType multiSampleType, int multiSampleQuality, RenderTargetUsage usage);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, MultiSampleType multiSampleType, int multiSampleQuality, RenderTargetUsage usage);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat, MultiSampleType multiSampleType, int multiSampleQuality);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, MultiSampleType multiSampleType, int multiSampleQuality);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat, RenderTargetUsage usage);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, RenderTargetUsage usage);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat);
        AleRenderTarget CreateRenderTarget(NameId name, int width, int height, int numberLevels, SurfaceFormat format);
        AleRenderTarget GetRenderTarget(NameId name);
        bool DestroyRenderTarget(NameId name);

    }

    public sealed class RenderTargetManager : IRenderTargetManager
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
