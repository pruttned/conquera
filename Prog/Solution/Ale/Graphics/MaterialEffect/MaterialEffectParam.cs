using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Microsoft.Xna.Framework.Graphics.EffectParameter proxy that updates parameter's value only if it is really neaded
    /// </summary>
    public abstract class MaterialEffectParam
    {
        #region Fields

        private string mName;

        /// <summary>
        /// Effect parameter to which is this proxy binded
        /// </summary>
        private EffectParameter mEffectParameter;

        private Type mParamType;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the parameter's name
        /// </summary>
        public string Name
        {
            get { return mEffectParameter.Name; }
        }

        /// <summary>
        /// Gets the semantic meaning, or usage, of the parameter
        /// </summary>
        public string Semantic
        {
            get { return mEffectParameter.Semantic; }
        }

        /// <summary>
        /// Annotations
        /// </summary>
        public EffectAnnotationCollection Annotations
        {
            get { return mEffectParameter.Annotations; }
        }

        /// <summary>
        /// Gets the type of value that can be stored in parameter
        /// </summary>
        public Type ParamType
        {
            get { return mParamType; }
        }

        internal bool IsUsedInTechnique(EffectTechnique technique)
        {
            return technique.IsParameterUsed(mEffectParameter);
        }

        /// <summary>
        /// Effect parameter to which is this proxy binded
        /// </summary>
        protected EffectParameter EffectParameter
        {
            get { return mEffectParameter; }
        }

        #endregion Properties

        /// <summary>
        /// Creates a new manual parameter with null value
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which will be this parameter binded</param>
        /// <param name="paramType"></param>
        internal MaterialEffectParam(EffectParameter effectParameter, Type paramType)
        {
            mName = effectParameter.Name;
            mEffectParameter = effectParameter;
            mParamType = paramType;
        }

        /// <summary>
        /// Creates a new parameter or returns null if it has unsupported type
        /// </summary>
        /// <param name="effectParameter">- Effect parameter to which should be this material effect paramter binded</param>
        /// <returns>New material effect parameter or null if it has unsupported type</returns>
        public static MaterialEffectParam TryCreateParam(EffectParameter effectParameter)
        {
            bool array = (0 != effectParameter.Elements.Count);

            switch (effectParameter.ParameterClass)
            {
                case EffectParameterClass.MatrixRows:
                    {
                        if (array)
                        {
                            return new MatrixArrayMaterialEffectParam(effectParameter);
                        }
                        return new MatrixMaterialEffectParam(effectParameter);
                    }
                case EffectParameterClass.Vector:
                    {
                        switch (effectParameter.ColumnCount)
                        {
                            case 2:
                                if (array)
                                {
                                    return new Vector2ArrayMaterialEffectParam(effectParameter);
                                }
                                return new Vector2MaterialEffectParam(effectParameter);
                            case 3:
                                if (array)
                                {
                                    return new Vector3ArrayMaterialEffectParam(effectParameter);
                                }
                                return new Vector3MaterialEffectParam(effectParameter);
                            case 4:
                                if (array)
                                {
                                    return new Vector4ArrayMaterialEffectParam(effectParameter);
                                }
                                return new Vector4MaterialEffectParam(effectParameter);
                        }
                        break;
                    }
                case EffectParameterClass.Scalar:
                    {
                        switch (effectParameter.ParameterType)
                        {
                            case EffectParameterType.Single:
                                {
                                    if (array)
                                    {
                                        return new FloatArrayMaterialEffectParam(effectParameter);
                                    }
                                    return new FloatMaterialEffectParam(effectParameter);

                                }
                            case EffectParameterType.Int32:
                                {
                                    if (array)
                                    {
                                        return new IntArrayMaterialEffectParam(effectParameter);
                                    }
                                    return new IntMaterialEffectParam(effectParameter);
                                }
                            case EffectParameterType.Bool:
                                {
                                    if (array)
                                    {
                                        return new BoolArrayMaterialEffectParam(effectParameter);
                                    }
                                    return new BoolMaterialEffectParam(effectParameter);
                                }
                        }
                        break;
                    }
                case EffectParameterClass.Object:
                    {
                        switch (effectParameter.ParameterType)
                        {
                            case EffectParameterType.Texture2D:
                                {
                                    return new Texture2DMaterialEffectParam(effectParameter);
                                }
                            case EffectParameterType.Texture3D:
                                {
                                    return new Texture3DMaterialEffectParam(effectParameter);
                                }
                            case EffectParameterType.TextureCube:
                                {
                                    return new TextureCubeMaterialEffectParam(effectParameter);
                                }
                        }
                        break;
                    }
            }

            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericMaterialEffectParam<T> : MaterialEffectParam
    {
        private T mValue;
        private static bool mIsGenericEquatable;

        protected static bool IsGenericEquatable
        {
            get { return mIsGenericEquatable; }
            set { mIsGenericEquatable = value; }
        }

        /// <summary>
        /// Gets/sets the value
        /// </summary>
        public T Value
        {
            get { return mValue; }
            set
            {
                if (ValueChanged(value))
                {
                    mValue = value;
                    SetValueInEffect(mValue);
                }
            }
        }

        static GenericMaterialEffectParam()
        {
            mIsGenericEquatable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));
        }

        protected GenericMaterialEffectParam(EffectParameter effectParameter, T initValue)
            : base(effectParameter, typeof(T))
        {
            mValue = initValue;
        }

        abstract protected void SetValueInEffect(T value);

        protected virtual bool ValueChanged(T newValue)
        {
            if (null == mValue && null != newValue)
            {
                return true;
            }
            if (null == mValue && null == newValue)
            {
                return false;
            }

            if (IsGenericEquatable) //prevent boxing
            {
                return !((IEquatable<T>)mValue).Equals(newValue);
            }
            return !mValue.Equals(newValue);
        }
    }

    /// <summary>
    /// Int effect parameter
    /// </summary>
    public class IntMaterialEffectParam : GenericMaterialEffectParam<int>
    {
        internal IntMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueInt32())
        {
        }

        protected override void SetValueInEffect(int value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Float effect parameter
    /// </summary>
    public class FloatMaterialEffectParam : GenericMaterialEffectParam<float>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal FloatMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueSingle())
        {
        }

        protected override void SetValueInEffect(float value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Bool effect parameter
    /// </summary>
    public class BoolMaterialEffectParam : GenericMaterialEffectParam<bool>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal BoolMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueBoolean())
        {
        }

        protected override void SetValueInEffect(bool value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector2 effect parameter
    /// </summary>
    public class Vector2MaterialEffectParam : GenericMaterialEffectParam<Vector2>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector2MaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector2())
        {
        }

        protected override void SetValueInEffect(Vector2 value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector3 effect parameter
    /// </summary>
    public class Vector3MaterialEffectParam : GenericMaterialEffectParam<Vector3>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector3MaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector3())
        {
        }

        protected override void SetValueInEffect(Vector3 value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector4 effect parameter
    /// </summary>
    public class Vector4MaterialEffectParam : GenericMaterialEffectParam<Vector4>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector4MaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector4())
        {
        }

        protected override void SetValueInEffect(Vector4 value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Matrix effect parameter
    /// </summary>
    public class MatrixMaterialEffectParam : GenericMaterialEffectParam<Matrix>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal MatrixMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueMatrix())
        {
        }

        protected override void SetValueInEffect(Matrix value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Texture2D effect parameter
    /// </summary>
    public class Texture2DMaterialEffectParam : GenericMaterialEffectParam<Texture2D>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Texture2DMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueTexture2D())
        {
        }

        protected override void SetValueInEffect(Texture2D value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Texture3D effect parameter
    /// </summary>
    public class Texture3DMaterialEffectParam : GenericMaterialEffectParam<Texture3D>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Texture3DMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueTexture3D())
        {
        }

        protected override void SetValueInEffect(Texture3D value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// TextureCube effect parameter
    /// </summary>
    public class TextureCubeMaterialEffectParam : GenericMaterialEffectParam<TextureCube>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal TextureCubeMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueTextureCube())
        {
        }

        protected override void SetValueInEffect(TextureCube value)
        {
            EffectParameter.SetValue(value);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                         Arrays
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public abstract class ArrayMaterialEffectParam<T> : GenericMaterialEffectParam<T[]>
    {
        static ArrayMaterialEffectParam()
        {
            IsGenericEquatable = typeof(IEquatable<T>).IsAssignableFrom(typeof(T));
        }

        protected ArrayMaterialEffectParam(EffectParameter effectParameter, T[] initValue)
            : base(effectParameter, initValue)
        {
        }

        protected override bool ValueChanged(T[] newValue)
        {
            return true;
            //to dole je somarina  lebo som porovnaval rovnake pole s rovnakym polom - menili sa prvky a teda sa zmenili aj vo Value. Musel by som si ho kopirovat aby to fungovalo... mozno niekedy inokedy

            //if (null == Value)
            //{
            //    if (null != newValue)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}


            //if (Value.Length != newValue.Length)
            //{
            //    return true;
            //}

            //if (IsGenericEquatable)
            //{
            //    for (int i = 0; i < Value.Length; ++i)
            //    {
            //        if (!((IEquatable<T>)Value[i]).Equals(newValue[i]))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < Value.Length; ++i)
            //    {
            //        if (!Value[i].Equals(newValue[i]))
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;
        }
    }

    /// <summary>
    /// Int[] effect parameter
    /// </summary>
    public class IntArrayMaterialEffectParam : ArrayMaterialEffectParam<int>
    {
        internal IntArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueInt32Array(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(int[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Float[] effect parameter
    /// </summary>
    public class FloatArrayMaterialEffectParam : ArrayMaterialEffectParam<float>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal FloatArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueSingleArray(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(float[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Bool[] effect parameter
    /// </summary>
    public class BoolArrayMaterialEffectParam : ArrayMaterialEffectParam<bool>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal BoolArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueBooleanArray(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(bool[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector2[] effect parameter
    /// </summary>
    public class Vector2ArrayMaterialEffectParam : ArrayMaterialEffectParam<Vector2>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector2ArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector2Array(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(Vector2[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector3[] effect parameter
    /// </summary>
    public class Vector3ArrayMaterialEffectParam : ArrayMaterialEffectParam<Vector3>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector3ArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector3Array(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(Vector3[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Vector4[] effect parameter
    /// </summary>
    public class Vector4ArrayMaterialEffectParam : ArrayMaterialEffectParam<Vector4>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal Vector4ArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueVector4Array(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(Vector4[] value)
        {
            EffectParameter.SetValue(value);
        }
    }

    /// <summary>
    /// Matrix[] effect parameter
    /// </summary>
    public class MatrixArrayMaterialEffectParam : ArrayMaterialEffectParam<Matrix>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="effectParameter"></param>
        internal MatrixArrayMaterialEffectParam(EffectParameter effectParameter)
            : base(effectParameter, effectParameter.GetValueMatrixArray(effectParameter.Elements.Count))
        {
        }

        protected override void SetValueInEffect(Matrix[] value)
        {
            EffectParameter.SetValue(value);
        }
    }
}
