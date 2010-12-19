using System;
using Microsoft.Xna.Framework.Graphics;
using Ale.Tools;

namespace Ale.Graphics
{
    class RenderTargetAutoParam
    {
        /// <summary>
        /// Effect parameter to which is this auto parameter binded
        /// </summary>
        private EffectParameter mEffectParameter;
        private NameId mRenderTargetId;
        private Texture2D mLastTexture = null;

        static public RenderTargetAutoParam TryCreateAutoParam(EffectParameter effectParameter)
        {
            if ("RenderTargetMap" == effectParameter.Semantic)
            {
                return new RenderTargetAutoParam(effectParameter);
            }
            return null;
        }


        /// <summary>
        /// Updates the binded parameter's value
        /// </summary>
        /// <param name="gameTime">- Actual game time (null = reset parameter)</param>
        public void Update(RenderTargetManager renderTargetManager)
        {
            if (null != renderTargetManager)
            {
                AleRenderTarget rt = renderTargetManager[mRenderTargetId];
                Texture2D texture = null;

                if (null != rt)
                {
                    texture = rt.Texture;
                }

                if (mLastTexture != texture)
                {
                    mEffectParameter.SetValue(texture);
                    mLastTexture = texture;
                }
            }
            else
            {
                mEffectParameter.SetValue((Texture2D)null);
                mLastTexture = null;
            }
        }

        internal bool IsUsedInTechnique(EffectTechnique technique)
        {
            return technique.IsParameterUsed(mEffectParameter);
        }
        
        private RenderTargetAutoParam(EffectParameter effectParameter)
        {
            mEffectParameter = effectParameter;
            EffectAnnotation targetNameAnnotation = effectParameter.Annotations["TargetName"];
            if (null == targetNameAnnotation)
            {
                throw new ArgumentException(string.Format("Missing TargetName annotation in RenderTargetMap effectParameter '{0}'", effectParameter.Name));
            }
            mRenderTargetId = targetNameAnnotation.GetValueString();
        }

        internal void OnLost()
        {
            mLastTexture = null;
        }
    }
}
