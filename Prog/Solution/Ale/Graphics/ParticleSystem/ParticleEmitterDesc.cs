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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;
using Ale.Content;

namespace Ale.Graphics
{
    public abstract class ParticleEmitterDesc : IDisposable
    {
        private Material mMaterial;
        private Texture2D mParticleColorFunctionTexture;

        private Vector4 mColorVariation;
        private float mRotationVariation;
        private float mSizeVariation;
        private Texture2DMaterialEffectParam mParticleColorFunctionMapFxParam;
        private Vector4MaterialEffectParam mColorVariationFxParam;
        private FloatMaterialEffectParam mRotationVariationFxParam;
        private FloatMaterialEffectParam mSizeVariationFxParam;
        
        private float mCycleTime;
        private int mMaxParticleCnt;
        
        private TimeFunction mEmissionRateFunction;

        private Vector3 mParticleDirection;
        private Vector3 mParticleDirectionPerp;
        private float mParticleDirectionVariation;
        private float mParticleLifeDuration;
        private float mParticleLifeDurationVariation;

        private float mFastForwardTimeOnLoad;
        private float mMaxFastForwardTime;
        private float mSampleRateOnFastForward;

        private TimeFunction mParticleSpeed;
        private TimeFunction mParticleRotation;
        private TimeFunction mParticleSize;

        private float mMaxParticleSize;

        /// <summary>
        /// Emitter position relative to particle system world position
        /// </summary>
        private Vector3 mRelativePosition;

        private IParticleAffector[] mParticleAffectors = null;

        private bool mSort;

        bool mIsDisposed = false;

        public Material Material
        {
            get { return mMaterial; }
        }

        public float CycleTime
        {
            get { return mCycleTime; }
        }

        public int MaxParticleCnt
        {
            get { return mMaxParticleCnt; }
        }

        public float FastForwardTimeOnLoad
        {
            get { return mFastForwardTimeOnLoad; }
        }

        public float MaxFastForwardTime
        {
            get { return mMaxFastForwardTime; }
        }
        public float SampleRateOnFastForward
        {
            get { return mSampleRateOnFastForward; }
        }

        public float MaxParticleSize
        {
            get { return mMaxParticleSize; }
        }

        public bool Sort
        {
            get { return mSort; }
        }

        /// <summary>
        /// Gets the emitter position relative to particle system world position
        /// </summary>
        public Vector3 RelativePosition
        {
            get { return mRelativePosition; }
        }

        public ParticleEmitterDesc(GraphicsDevice graphicsDevice, ParticleEmitterSettings settings, ContentGroup content)
        {
            mMaterial = content.Load<Material>(settings.Material);

            mParticleColorFunctionMapFxParam = (Texture2DMaterialEffectParam)mMaterial.MaterialEffect.ManualParameters.GetParamBySemantic("ColorFunctionMap");
            if (null != mParticleColorFunctionMapFxParam)
            {
                InitParticleColorFunctionTexture(graphicsDevice, settings);
            }
            mColorVariationFxParam = (Vector4MaterialEffectParam)mMaterial.MaterialEffect.ManualParameters.GetParamBySemantic("ColorVariation");
            mRotationVariationFxParam = (FloatMaterialEffectParam)mMaterial.MaterialEffect.ManualParameters.GetParamBySemantic("RotationVariation");
            mSizeVariationFxParam = (FloatMaterialEffectParam)mMaterial.MaterialEffect.ManualParameters.GetParamBySemantic("SizeVariation");

            mColorVariation = settings.ColorVariation;
            mRotationVariation = settings.RotationVariation;
            mSizeVariation = settings.SizeVariation;

            mCycleTime = settings.CycleTime;
            mMaxParticleCnt = settings.MaxParticleCnt;
            mEmissionRateFunction = settings.EmissionRateFunction;


            Vector2 dirAngles = settings.ParticleDirectionAngles;
            Vector2 xRotPos = new Vector2((float)System.Math.Cos(dirAngles.X - MathHelper.PiOver2),
                (float)System.Math.Sin(dirAngles.X - MathHelper.PiOver2));
            mParticleDirection = new Vector3((float)System.Math.Sin(dirAngles.Y) * xRotPos.X,
               -(float)System.Math.Cos(dirAngles.Y) * xRotPos.X,
               -xRotPos.Y);
            //mParticleDirection.Normalize();

            AleMathUtils.GetPerpVector(ref mParticleDirection, out mParticleDirectionPerp);

            mParticleDirectionVariation = settings.ParticleDirectionVariation;
            mParticleLifeDuration = settings.ParticleLifeDuration;
            mParticleLifeDurationVariation = settings.ParticleLifeDurationVariation;

            mFastForwardTimeOnLoad = settings.FastForwardTimeOnLoad;
            mMaxFastForwardTime = settings.MaxFastForwardTime;
            mSampleRateOnFastForward = settings.SampleRateOnFastForward;

            mParticleSpeed = settings.ParticleSettings.ParticleSpeed;
            mParticleRotation = settings.ParticleSettings.ParticleRotation;
            mParticleSize = settings.ParticleSettings.ParticleSize;

            mMaxParticleSize = mParticleSize.GetMax();

            mSort = settings.Sort;

            mRelativePosition = settings.RelativePosition;

            //particle affectors
            int particleAffectorsCnt = settings.ParticleAffectors.Count;
            if (0 < particleAffectorsCnt)
            {
                mParticleAffectors = new IParticleAffector[particleAffectorsCnt];
                for (int i = 0; i < particleAffectorsCnt; ++i)
                {
                    mParticleAffectors[i] = settings.ParticleAffectors[i].CreateParticleAffector();
                }
            }
        }

        private void InitParticleColorFunctionTexture(GraphicsDevice graphicsDevice, ParticleEmitterSettings particleSystemSettings)
        {
            TimeFunction particleColorRFunction = particleSystemSettings.ParticleSettings.ParticleColorRFunction;
            TimeFunction particleColorGFunction = particleSystemSettings.ParticleSettings.ParticleColorGFunction;
            TimeFunction particleColorBFunction = particleSystemSettings.ParticleSettings.ParticleColorBFunction;
            TimeFunction particleColorAFunction = particleSystemSettings.ParticleSettings.ParticleColorAFunction;
            mParticleColorFunctionTexture = new Texture2D(graphicsDevice, 64, 64, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] textureData = new Color[64 * 64];
            int index = 0;
            for (int i = 0; i < 64; ++i)
            {
                for (int j = 0; j < 64; ++j)
                {
                    float lerp = (float)index / 4096.0f;

                    textureData[index++] = new Color
                        (
                            particleColorRFunction[lerp],
                            particleColorGFunction[lerp],
                            particleColorBFunction[lerp],
                            particleColorAFunction[lerp]
                        );
                }
            }
            mParticleColorFunctionTexture.SetData<Color>(textureData);
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
                    if (null != mParticleColorFunctionTexture)
                    {
                        mParticleColorFunctionTexture.Dispose();
                        mParticleColorFunctionTexture = null;
                    }
                }
                mMaterial = null;
                mIsDisposed = true;
            }
        }

        public void UpdateMaterialEffectParameters()
        {
            if (null != mParticleColorFunctionMapFxParam)
            {
                mParticleColorFunctionMapFxParam.Value = mParticleColorFunctionTexture;
            }
            if (null != mColorVariationFxParam)
            {
                mColorVariationFxParam.Value = mColorVariation;
            }
            if (null != mRotationVariationFxParam)
            {
                mRotationVariationFxParam.Value = mRotationVariation;
            }
            if (null != mSizeVariationFxParam)
            {
                mSizeVariationFxParam.Value = mSizeVariation;
            }
        }

        public void InitParticle(float totalTime, ref Vector3 particleSystemPosition, Particle particle)
        {
            particle.BirthTime = totalTime;
            particle.LifeDuration = AleMathUtils.GetRandomFloat(mParticleLifeDuration, mParticleLifeDurationVariation);
            GenerateParticlePosition(ref particleSystemPosition, out particle.Position);
            particle.Seed = (float)AleMathUtils.Random.NextDouble();

            //Direction
            //random perpendicular vector to direction
            Quaternion quat;
            Quaternion.CreateFromAxisAngle(ref mParticleDirection, (float)AleMathUtils.Random.NextDouble() * MathHelper.TwoPi, out quat);
            Vector3 rotPerp;
            Vector3.Transform(ref mParticleDirectionPerp, ref quat, out rotPerp);
            //rotate arround  random  perpendicular  vector
            Quaternion.CreateFromAxisAngle(ref rotPerp, (float)AleMathUtils.Random.NextDouble() * mParticleDirectionVariation, out quat);
            Vector3.Transform(ref mParticleDirection, ref quat, out particle.Direction);

            //  particle.Size = mParticleSize[0.0f];
            //  particle.Rotation = mParticleRotation[0.0f];

            particle.IsAlive = true;
        }

        public float GetEmissionRate(float particleSystemCycleLerp)
        {
            return mEmissionRateFunction[particleSystemCycleLerp];
        }

        public float GetParticleSpeed(float particleLifeLerp)
        {
            return mParticleSpeed[particleLifeLerp];
        }

        public float GetParticleRotation(float particleLifeLerp)
        {
            return mParticleRotation[particleLifeLerp];
        }

        public float GetParticleSize(float particleLifeLerp)
        {
            return mParticleSize[particleLifeLerp];
        }

        public void ApplyParticleAffectors(ParticleEmitter emitter, float totalTime, float elapsedTime)
        {
            Vector3 emitterWorldPosition = emitter.Position;
            if (null != mParticleAffectors)
            {
                for (int i = 0; i < mParticleAffectors.Length; ++i)
                {
                    mParticleAffectors[i].AffectParticles(emitter, ref emitterWorldPosition, totalTime, elapsedTime);
                }
            }
        }

        protected abstract void GenerateParticlePosition(ref Vector3 particleSystemPosition, out Vector3 particlePosition);

    }
}
