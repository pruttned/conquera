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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Content;
using SimpleOrmFramework;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;


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

        public PointLight(ContentGroup content, Material material)
            : base(new BoundingSphere(Vector3.Zero, 1), true)
        {
            mMeshPart = content.Load<Mesh>("Sphere").MeshParts[0];
            mMaterial = material;
            mColorParam = (Vector3MaterialEffectParam)material.MaterialEffect.ManualParameters["gColor"];
            Color = Vector3.One;
            mRadiusParam = (FloatMaterialEffectParam)material.MaterialEffect.ManualParameters["gLightRadius"];

            //ShowWorldBounds = true;
        }

        public PointLight(ContentGroup content, PointLightDesc desc)
            : this(content, desc.Material)
        {
            Scale = desc.Radius;
            Color = desc.Color;
        }

        protected override void OnEnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime)
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

}
