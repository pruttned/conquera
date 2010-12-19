using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Ale.Tools;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    public sealed class AleRenderTarget : IDisposable
    {
        private static bool mInBeginEnd = false;
        private static DepthStencilBuffer mDefaultDepthStencilBuffer = null;

        private GraphicsDeviceManager mGraphicsDeviceManager;
        private RenderTarget2D mRenderTarget2D;
        private DepthStencilBuffer mDepthStencilBuffer;

        private NameId mName;
        private int mWidth;
        private int mHeight;
        private int mNumberLevels;
        private SurfaceFormat mFormat;
        private MultiSampleType mMultiSampleType;
        private int mMultiSampleQuality;
        private RenderTargetUsage mUsage;
        
        /// <summary>
        /// If == DepthFormat.Unknown then no DepthStencilBuffer is used
        /// </summary>
        private DepthFormat mDepthFormat;

        private bool mIsDisposed = false;

        public GraphicsDevice GraphicsDevice
        {
            get { return mGraphicsDeviceManager.GraphicsDevice; }
        }

        public NameId NameId
        {
            get { return NameId; }
        }

        public int Width
        {
            get { return mWidth; }
        }

        public int Height
        {
            get { return mHeight; }
        }

        public int NumberLevels
        {
            get { return mNumberLevels; }
        }

        public SurfaceFormat Format
        {
            get { return mFormat; }
        }

        public MultiSampleType MultiSampleType
        {
            get { return mMultiSampleType; }
        }

        public int MultiSampleQuality
        {
            get { return mMultiSampleQuality; }
        }

        public RenderTargetUsage Usage
        {
            get { return mUsage; }
        }

        public DepthFormat DepthFormat
        {
            get { return mDepthFormat; }
        }

        public Texture2D Texture
        {
            get 
            {
                if (null == mRenderTarget2D)
                {
                    return null;
                }
                if (mRenderTarget2D.IsContentLost)
                {
                    Clear(Color.White);
                }
                return mRenderTarget2D.GetTexture(); 
            }
        }

        public AleRenderTarget(GraphicsDeviceManager graphicsDeviceManager, NameId name, int width, int height, int numberLevels, SurfaceFormat format)
            : this(graphicsDeviceManager, name, width, height, numberLevels, format, DepthFormat.Unknown, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents)
        {
        }

        public AleRenderTarget(GraphicsDeviceManager graphicsDeviceManager, NameId name, int width, int height, int numberLevels, SurfaceFormat format, DepthFormat depthFormat,
            MultiSampleType multiSampleType, int multiSampleQuality, RenderTargetUsage usage)
        {
            Tracer.WriteInfo("Creating render target '{0}' width={1} height={2} levels={3} format={4} depthFormat={5} multiSampleType={6} multiSampleQuality={7} usage={8}",
                name, width, height, numberLevels, format, depthFormat, multiSampleType, multiSampleQuality, usage);

            mGraphicsDeviceManager = graphicsDeviceManager ;
            mName = name;
            mWidth = width;
            mHeight = height;
            mNumberLevels = numberLevels;
            mFormat = format;
            mMultiSampleType = multiSampleType;
            mMultiSampleQuality = multiSampleQuality;
            mUsage = usage;
            mDepthFormat = depthFormat;
        }

        /// <summary>
        /// Clears the render target with a given color. This method calls Begin and End.
        /// </summary>
        /// <param name="color"></param>
        public void Clear(Color color)
        {
            Begin();

            GraphicsDevice.Clear(color);

            End();
        }

        /// <summary>
        /// Clears the render target with a given color. This method calls Begin and End.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="clearOptions"></param>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        public void Clear(ClearOptions clearOptions, Color color, float depth, int stencil)
        {
            Begin();

            GraphicsDevice.Clear(clearOptions, color, depth, stencil);

            End();
        }

        public void Begin()
        {
            if (mInBeginEnd)
            {
                throw new InvalidOperationException("Multiple calls of Begin method detected withou calling End");
            }

            mInBeginEnd = true;

            if (null == mRenderTarget2D) //not loaded
            {
                Load();
            }

            if (null == mDefaultDepthStencilBuffer)
            {
                mDefaultDepthStencilBuffer = GraphicsDevice.DepthStencilBuffer;
                mDefaultDepthStencilBuffer.Disposing += new EventHandler(mDefaultDepthStencilBuffer_Disposing);
            }

            GraphicsDevice.SetRenderTarget(0, mRenderTarget2D);
            GraphicsDevice.DepthStencilBuffer = mDepthStencilBuffer;
        }

        public void End()
        {
            End(true);
        }

        public void End(bool setDefaultRenderTarget)
        {
            if (!mInBeginEnd)
            {
                throw new InvalidOperationException("End method can't be called without calling Begin method first");
            }

            mInBeginEnd = false;

            if (setDefaultRenderTarget)
            {
                GraphicsDevice.SetRenderTarget(0, null);
                GraphicsDevice.DepthStencilBuffer = mDefaultDepthStencilBuffer;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                Tracer.WriteInfo("Disposing AleRenderTarget '{0}'", mName);

                if (null != mRenderTarget2D)
                {
                    mRenderTarget2D.Dispose();
                    mRenderTarget2D = null;
                }

                if (null != mDepthStencilBuffer)
                {
                    mDepthStencilBuffer.Dispose();
                    mDepthStencilBuffer = null;
                }

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }
     
        #endregion IDisposable

        private void mDefaultDepthStencilBuffer_Disposing(object sender, EventArgs e)
        {
            mDefaultDepthStencilBuffer.Disposing -= mDefaultDepthStencilBuffer_Disposing;
            mDefaultDepthStencilBuffer = null;
        }

        private void Load()
        {
            Tracer.WriteInfo("Loading AleRenderTarget '{0}'", mName);

            mRenderTarget2D = new RenderTarget2D(GraphicsDevice, mWidth, mHeight, mNumberLevels, mFormat, mMultiSampleType, mMultiSampleQuality, mUsage);

            if (DepthFormat.Unknown != mDepthFormat)
            {
                mDepthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, mWidth, mHeight, mDepthFormat, mMultiSampleType, mMultiSampleQuality);
            }
        }
        
        public override string ToString()
        {
            return mName.ToString();
        }
    }
}
