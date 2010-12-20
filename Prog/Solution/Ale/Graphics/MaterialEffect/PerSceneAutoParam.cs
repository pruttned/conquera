//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Scene;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Auto parameter that is updated ones per scene
    /// </summary>
    internal class PerSceneAutoParam
    {
        #region Enums

        /// <summary>
        /// Defines parameter's semantic
        /// </summary>
        enum Semantic
        {
            /// <summary>
            /// View transformation metrix of a specific scene pass camera
            /// </summary>
            ScenePassCameraView,

            /// <summary>
            /// Projection transformation metrix of a specific scene pass camera
            /// </summary>
            ScenePassCameraProjection,

            /// <summary>
            /// View*Projection transformation metrix of a specific scene pass camera
            /// </summary>
            ScenePassCameraViewProjection,

            /// <summary>
            /// Untransformed position of the camera's eye of a specific scene pass camera
            /// </summary>
            ScenePassCameraEyePosition
        }

        #endregion Enums

        #region Fields

        /// <summary>
        /// Semantic names
        /// </summary>
        private static readonly string[] SemanticNames = Enum.GetNames(typeof(Semantic));

        /// <summary>
        /// Effect parameter to which is this auto parameter binded
        /// </summary>
        private EffectParameter mEffectParameter;

        /// <summary>
        /// Parameter's semantic
        /// </summary>
        private Semantic mSemantic;

        private NameId mScenePassName;

        private Scene.BaseScene mLastScene = null;
        private ScenePass mLastScenePass = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Creates a new auto parameter or returns null if its semantic is incompatible with per scene parameter semantics
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <returns>New auto parameter or null if its semantic is incompatible with per scene parameter semantics</returns>
        static public PerSceneAutoParam TryCreateAutoParam(EffectParameter effectParameter)
        {
            for (int i = 0; i < SemanticNames.Length; ++i)
            { //Scaning through semantic names is better then using Enum.parse because it is possible that 
                //the semantic will be not found in Semantic enum and Enum.Parse will throws exception in that case 
                //which is slow. If parameter's semantic is not found among Semantic enumeration members, then it just
                //meens that is is not a PerCameraAutoParam but another parameter type.
                if (string.Equals(SemanticNames[i], effectParameter.Semantic, StringComparison.OrdinalIgnoreCase))
                {
                    return new PerSceneAutoParam(effectParameter, (Semantic)i);
                }
            }
            return null;
        }

        internal bool IsUsedInTechnique(EffectTechnique technique)
        {
            return technique.IsParameterUsed(mEffectParameter);
        }

        /// <summary>
        /// Updates the binded parameter's value
        /// </summary>
        /// <param name="scene">- Actual scene</param>
        public void Update(Scene.BaseScene scene)
        {
            if (NeedScenePass()) //update scene pass
            {
                if (mLastScene != scene)
                {
                    mLastScene = scene;
                    mLastScenePass = scene.GetScenePass(mScenePassName);
                }
                if (null == mLastScene)
                {
                    mLastScenePass = null;
                }
            }

            switch (mSemantic)
            {
                case Semantic.ScenePassCameraView:
                    mEffectParameter.SetValue((null != mLastScenePass) ? mLastScenePass.Camera.ViewTransformation : Matrix.Identity);
                    break;
                case Semantic.ScenePassCameraProjection:
                    mEffectParameter.SetValue((null != mLastScenePass) ? mLastScenePass.Camera.ProjectionTransformation : Matrix.Identity);
                    break;
                case Semantic.ScenePassCameraViewProjection:
                    mEffectParameter.SetValue((null != mLastScenePass) ? mLastScenePass.Camera.ViewProjectionTransformation : Matrix.Identity);
                    break;
                case Semantic.ScenePassCameraEyePosition:
                    mEffectParameter.SetValue((null != mLastScenePass) ? mLastScenePass.Camera.WorldPosition : Vector3.Zero);
                    break;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this auto paramter binded</param>
        /// <param name="semantic">- Parameter's semantic</param>
        private PerSceneAutoParam(EffectParameter effectParameter, Semantic semantic)
        {
            mEffectParameter = effectParameter;
            mSemantic = semantic;

            if (NeedScenePass())
            {
                EffectAnnotation scenePassAnnotation = effectParameter.Annotations["ScenePass"];
                if (null == scenePassAnnotation)
                {
                    throw new ArgumentException(string.Format("Missing ScenePass annotation in {0} effectParameter '{1}'", mSemantic.ToString(), effectParameter.Name));
                }

                mScenePassName = scenePassAnnotation.GetValueString();
            }
        }

        private bool NeedScenePass()
        {
            return (Semantic.ScenePassCameraProjection == mSemantic || Semantic.ScenePassCameraView == mSemantic ||
                Semantic.ScenePassCameraViewProjection == mSemantic || Semantic.ScenePassCameraEyePosition == mSemantic);
        }

        #endregion Methods
    }
}
