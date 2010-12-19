



using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Ale.Content;

namespace Ale.Graphics
{
    /// <summary>
    /// Renderable for displaying bounding box of a renderable
    /// </summary>
    public class BoundingBoxRenderable : Renderable, IRenderableUnit
    {
        #region Fields

        /// <summary>
        /// Vertex buffer
        /// </summary>
        private static VertexBuffer mVertexBuffer = null;

        /// <summary>
        /// Index buffer
        /// </summary>
        private static IndexBuffer mIndexBuffer = null;

        /// <summary>
        /// Material
        /// </summary>
        private static Material mMaterial;

        /// <summary>
        /// Color effect param
        /// </summary>
        private static Vector3MaterialEffectParam mColorParam;
        private static Vector3MaterialEffectParam mPositionParam;
        private static Vector3MaterialEffectParam mScaleParam;

        /// <summary>
        /// Vertex declaration
        /// </summary>
        private static VertexDeclaration mVertexDeclaration;

        /// <summary>
        /// Line color
        /// </summary>
        private Vector3 mColor;

        private Vector3 mPosition;
        private Vector3 mScale;


        #endregion Fields

        #region Properties

        #region IRenderableUnit

        /// <summary>
        /// Parent renderable
        /// </summary>
        Renderable IRenderableUnit.ParentRenderable
        {
            get { return this; }
        }

        /// <summary>
        /// Material
        /// </summary>
        Material IRenderableUnit.Material
        {
            get { return mMaterial; }
        }

        #endregion IRenderableUnit

        /// <summary>
        /// Gets/sets the colot
        /// </summary>
        public Color Color
        {
            set { mColor = value.ToVector3(); }
            get { return new Color(mColor); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderable">- Renderable whose bounding box should be rendered</param>
        /// <param name="color">- Line color</param>
        public BoundingBoxRenderable(Color color)
            : base(new BoundingSphere(), true)
        {
            if (null == mVertexBuffer)
            {
                throw new InvalidOperationException("You must call static Init before calling ctor");
            }

            mColor = color.ToVector3();
        }

        /// <summary>
        /// Static init. Call this method once before contructing bounding box renderables
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        static public void Init(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            if (null != mVertexBuffer)
            {
                throw new InvalidOperationException("BoundingBoxRenderable is already initialized");
            }

            mMaterial = new Material(content.Load<MaterialEffect>("BoundingBoxPrimitiveFx"), DefaultRenderLayers.GroundStandingObjects);
            mColorParam = (Vector3MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters["gColor"];
            mPositionParam = (Vector3MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters["gPosition"];
            mScaleParam = (Vector3MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters["gScale"];


            mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]{ new VertexElement(0, 0, VertexElementFormat.Vector3,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.Position, 0)});

            //geometry
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),

                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1)
            };
            mVertexBuffer = new VertexBuffer(graphicsDevice, 12 * 8, BufferUsage.WriteOnly);
            mVertexBuffer.SetData<Vector3>(vertices);

            short[] indices = new short[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0,

                4, 5,
                5, 6,
                6, 7,
                7, 4,

                0, 4,
                1, 5,
                2, 6,
                3, 7
            };

            mIndexBuffer = new IndexBuffer(graphicsDevice, 16 * 24, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
            mIndexBuffer.SetData<short>(indices);
        }

        /// <summary>
        /// Updates transformations according to a bounding box
        /// </summary>
        /// <param name="boundingBox">- Bounding box in world space</param>
        public void SetBoundingBox(BoundingBox boundingBox)
        {
            //            SetTransformation(boundingBox.Min, Quaternion.Identity, boundingBox.Max - boundingBox.Min);
            //Position = boundingBox.Min;
            mPosition = boundingBox.Min;
            mScale = boundingBox.Max - boundingBox.Min;
        }

        #region IRenderableUnit

        /// <summary>
        /// Renders the renderable
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(AleGameTime gameTime)
        {
            GraphicsDevice graphicsDevice = mVertexDeclaration.GraphicsDevice;
            graphicsDevice.VertexDeclaration = mVertexDeclaration;
            graphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, 12);
            graphicsDevice.Indices = mIndexBuffer;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 8, 0, 12);
        }

        /// <summary>
        /// Updates the material effect params just before render
        /// </summary>
        public void UpdateMaterialEffectParameters()
        {
            mColorParam.Value = mColor;
            mScaleParam.Value = mScale;
            mPositionParam.Value = mPosition;
        }

        #endregion IRenderableUnit

        /// <summary>
        /// Enques the itself to the render queue
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="gameTime"></param>
        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            renderer.EnqueueRenderable(this);
        }

        #endregion Methods
    }
}

