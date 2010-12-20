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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ale.Graphics
{
    /// <summary>
    /// Material parameter
    /// </summary>
    public abstract class MaterialParam
    {
        #region Fields

        /// <summary>
        /// Effect parameter whose value is configured by this material parameter
        /// </summary>
        private MaterialEffectParam mMaterialEffectParam;

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Gets the parameter's name
        /// </summary>
        public string Name
        {
            get { return mMaterialEffectParam.Name; }
        }

        /// <summary>
        /// Effect parameter whose value is configured by this material parameter
        /// </summary>
        public MaterialEffectParam MaterialEffectParam
        {
            get { return mMaterialEffectParam; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="materialEeffectParam">- Effect parameter to which will be this parameter binded</param>
        internal MaterialParam(MaterialEffectParam materialEeffectParam)
        {
            if (null == materialEeffectParam) { throw new ArgumentNullException("materialEeffectParam"); }
            mMaterialEffectParam = materialEeffectParam;
        }

        /// <summary>
        /// Applies the material parameter on the binded effect parameter
        /// </summary>
        public abstract void Apply();
        
        #endregion Methods
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericMaterialParam<T, P> : MaterialParam where P : GenericMaterialEffectParam<T>
    {
        private T mValue;
        private P mMaterialEeffectParam;

        public T Value
        {
            get { return mValue; }
        }

        internal GenericMaterialParam(P materialEeffectParam, T value)
            : base(materialEeffectParam)
        {
            if (null == value) { throw new ArgumentNullException("value"); }

            mMaterialEeffectParam = materialEeffectParam;
            mValue = value;
        }

        public override void Apply()
        {
            mMaterialEeffectParam.Value = Value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IntMaterialParam : GenericMaterialParam<int, IntMaterialEffectParam>
    {
        public IntMaterialParam(IntMaterialEffectParam materialEeffectParam, int value)
            : base(materialEeffectParam, value)
        {}
    }

    /// <summary>
    /// 
    /// </summary>
    public class FloatMaterialParam : GenericMaterialParam<float, FloatMaterialEffectParam>
    {
        public FloatMaterialParam(FloatMaterialEffectParam materialEeffectParam, float value)
            : base(materialEeffectParam, value)
        {}
    }

    /// <summary>
    /// 
    /// </summary>
    public class BoolMaterialParam : GenericMaterialParam<bool, BoolMaterialEffectParam>
    {
        public BoolMaterialParam(BoolMaterialEffectParam materialEeffectParam, bool value)
            : base(materialEeffectParam, value)
        {}
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector2MaterialParam : GenericMaterialParam<Vector2, Vector2MaterialEffectParam>
    {
        public Vector2MaterialParam(Vector2MaterialEffectParam materialEeffectParam, Vector2 value)
            : base(materialEeffectParam, value)
        {}
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector3MaterialParam : GenericMaterialParam<Vector3, Vector3MaterialEffectParam>
    {
        public Vector3MaterialParam(Vector3MaterialEffectParam materialEeffectParam, Vector3 value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector4MaterialParam : GenericMaterialParam<Vector4, Vector4MaterialEffectParam>
    {
        public Vector4MaterialParam(Vector4MaterialEffectParam materialEeffectParam, Vector4 value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatrixMaterialParam : GenericMaterialParam<Matrix, MatrixMaterialEffectParam>
    {
        public MatrixMaterialParam(MatrixMaterialEffectParam materialEeffectParam, Matrix value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Texture2DMaterialParam : GenericMaterialParam<Texture2D, Texture2DMaterialEffectParam>
    {
        public Texture2DMaterialParam(Texture2DMaterialEffectParam materialEeffectParam, Texture2D value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Texture3DMaterialParam : GenericMaterialParam<Texture3D, Texture3DMaterialEffectParam>
    {
        public Texture3DMaterialParam(Texture3DMaterialEffectParam materialEeffectParam, Texture3D value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TextureCubeMaterialParam : GenericMaterialParam<TextureCube, TextureCubeMaterialEffectParam>
    {
        public TextureCubeMaterialParam(TextureCubeMaterialEffectParam materialEeffectParam, TextureCube value)
            : base(materialEeffectParam, value)
        {
        }
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                         Arrays
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// 
    /// </summary>
    public class IntArrayMaterialParam : GenericMaterialParam<int[], IntArrayMaterialEffectParam>
    {
        public IntArrayMaterialParam(IntArrayMaterialEffectParam materialEeffectParam, int[] value)
            : base(materialEeffectParam, value)
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FloatArrayMaterialParam : GenericMaterialParam<float[], FloatArrayMaterialEffectParam>
    {
        public FloatArrayMaterialParam(FloatArrayMaterialEffectParam materialEeffectParam, float[] value)
            : base(materialEeffectParam, value)
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BoolArrayMaterialParam : GenericMaterialParam<bool[], BoolArrayMaterialEffectParam>
    {
        public BoolArrayMaterialParam(BoolArrayMaterialEffectParam materialEeffectParam, bool[] value)
            : base(materialEeffectParam, value)
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector2ArrayMaterialParam : GenericMaterialParam<Vector2[], Vector2ArrayMaterialEffectParam>
    {
        public Vector2ArrayMaterialParam(Vector2ArrayMaterialEffectParam materialEeffectParam, Vector2[] value)
            : base(materialEeffectParam, value)
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector3ArrayMaterialParam : GenericMaterialParam<Vector3[], Vector3ArrayMaterialEffectParam>
    {
        public Vector3ArrayMaterialParam(Vector3ArrayMaterialEffectParam materialEeffectParam, Vector3[] value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Vector4ArrayMaterialParam : GenericMaterialParam<Vector4[], Vector4ArrayMaterialEffectParam>
    {
        public Vector4ArrayMaterialParam(Vector4ArrayMaterialEffectParam materialEeffectParam, Vector4[] value)
            : base(materialEeffectParam, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatrixArrayMaterialParam : GenericMaterialParam<Matrix[], MatrixArrayMaterialEffectParam>
    {
        public MatrixArrayMaterialParam(MatrixArrayMaterialEffectParam materialEeffectParam, Matrix[] value)
            : base(materialEeffectParam, value)
        {
        }
    }
}


















//namespace Ale.Graphics
//{
//    /// <summary>
//    /// Material parameter
//    /// </summary>
//    public class MaterialParam
//    {
//        #region Delegates

//        /// <summary>
//        /// Handler for a ValueChanged event
//        /// </summary>
//        /// <param name="param">- Param whose value has been changed</param>
//        public delegate void ValueChangedHandler(MaterialParam param);

//        #endregion Delegates

//        #region Events

//        /// <summary>
//        /// Raised whenever has been param's value changed
//        /// </summary>
//        public event ValueChangedHandler ValueChanged;

//        #endregion Events

//        #region Fields

//        /// <summary>
//        /// Effect parameter whose value is configured by this material parameter
//        /// </summary>
//        private MaterialEffectParam mMaterialEffectParam;

//        /// <summary>
//        /// Parameter's value( null -> Value is not updated when calling Apply)
//        /// </summary>
//        private object mValue;

//        #endregion Fields

//        #region Properties

//        /// <summary>
//        /// Gets the parameter's name
//        /// </summary>
//        public string Name
//        {
//            get { return MaterialEffectParam.Name; }
//        }

//        /// <summary>
//        /// Gets the parameter's value (null -> Value is not updated when calling Apply)
//        /// </summary>
//        public object Value
//        {
//            get { return mValue; }
//            set
//            {
//                if (mValue != value)
//                {
//                    mValue = value;
//                    if (null != ValueChanged)
//                    {
//                        ValueChanged.Invoke(this);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the effect parameter whose value is configured by this material parameter
//        /// </summary>
//        public MaterialEffectParam MaterialEffectParam
//        {
//            get { return mMaterialEffectParam; }
//        }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// Creates a new material parameter with null value
//        /// </summary>
//        /// <param name="materialEeffectParam">- Effect parameter to which will be this parameter binded</param>
//        public MaterialParam(MaterialEffectParam materialEeffectParam)
//            : this(materialEeffectParam, null)
//        {
//        }

//        /// <summary>
//        /// Creates a new manual parameter with a given value
//        /// </summary>
//        /// <param name="effectParam">- Effect parameter to which will be this parameter binded</param>
//        /// <param name="value">- Initial value of the parameter</param>
//        internal MaterialParam(MaterialEffectParam materialEeffectParam, object value)
//        {
//            mMaterialEffectParam = materialEeffectParam;
//            mValue = value;
//        }

//        /// <summary>
//        /// Applies the material parameter on the binded effect parameter
//        /// </summary>
//        public void Apply()
//        {
//            if (null != Value)
//            {
//                mMaterialEffectParam.Value = Value;
//            }
//        }

//        #endregion Methods
//    }
//}
