using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Content;

namespace Ale.Graphics
{
    public class PointLight : Renderable, ILightRenderableUnit
    {
        MeshPart mMeshPart;
        Material mMaterial;
        Vector3MaterialEffectParam mColorParam;
        FloatMaterialEffectParam mRadiusParam;

        Renderable IRenderableUnit.ParentRenderable
        {
            get { return this; }
        }

        Material IRenderableUnit.Material
        {
            get { return mMaterial; }
        }

        public Vector3 Color { get; set; }

        public PointLight(ContentGroup content, GraphicsDeviceManager graphicsDeviceManager, Material material)
            : base(new BoundingSphere(Vector3.Zero, 1), true)
        {
            mMeshPart = content.Load<Mesh>("Sphere").MeshParts[0];
            mMaterial = material;
            mColorParam = (Vector3MaterialEffectParam)material.MaterialEffect.ManualParameters["gColor"];
            Color = Vector3.One;
            mRadiusParam = (FloatMaterialEffectParam)material.MaterialEffect.ManualParameters["gLightRadius"];

            //ShowWorldBounds = true;
        }

        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
        {
            renderer.EnqueLight(this);
        }

        void IRenderableUnit.Render(AleGameTime gameTime)
        {
            mMeshPart.Render();
        }

        void IRenderableUnit.UpdateMaterialEffectParameters()
        {
            if (null != mColorParam)
            {
                mColorParam.Value = Color;
            }
            if (null != mRadiusParam)
            {
                mRadiusParam.Value = Scale;
            }
        }
        protected override void OnWorldBoundsChanged()
        {
            base.OnWorldBoundsChanged();
        }
    }

    public interface ILightRenderableUnit : IRenderableUnit
    {

    }
}
