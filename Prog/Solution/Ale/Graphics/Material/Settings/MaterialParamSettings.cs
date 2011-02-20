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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    //Sof doesn't support generics

    /// <summary>
    /// Base material param desc
    /// </summary>
    [DataObject]
    public abstract class MaterialParamSettings : BaseDataObject
    {
        private string mName;

        /// <summary>
        /// Name of the parameter in effect file 
        /// </summary>
        [DataProperty(NotNull=true)]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        protected MaterialParamSettings(string name)
        {
            mName = name;
        }

        /// <summary>
        /// for sof
        /// </summary>
        protected MaterialParamSettings()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialEffect"></param>
        /// <returns></returns>
        internal MaterialParam CreateMaterialParam(MaterialEffect materialEffect, ContentManager content)
        {
            MaterialEffectParam effectParam = materialEffect.ManualParameters[Name];
            if (null == effectParam)
            {
                throw new ArgumentException(string.Format("Parameter with name '{0}' could't be found in effect", Name)); 
            }
            return CreateMaterialParamImpl(effectParam, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal abstract MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content);
    }

    /// <summary>
    /// Int material param desc
    /// </summary>
    [DataObject]
    public class IntMaterialParamSettings : MaterialParamSettings
    {
        private int mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public int Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public IntMaterialParamSettings(string name, int value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new IntMaterialParam((IntMaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private IntMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Float material param desc
    /// </summary>
    [DataObject]
    public class FloatMaterialParamSettings : MaterialParamSettings
    {
        private float mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public float Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public FloatMaterialParamSettings(string name, float value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new FloatMaterialParam((FloatMaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private FloatMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Bool material param desc
    /// </summary>
    [DataObject]
    public class BoolMaterialParamSettings : MaterialParamSettings
    {
        private bool mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public bool Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public BoolMaterialParamSettings(string name, bool value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new BoolMaterialParam((BoolMaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private BoolMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Vector2 material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector2), typeof(FieldCustomBasicTypeProvider<Vector2>))]
    public class Vector2MaterialParamSettings : MaterialParamSettings
    {
        private Vector2 mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull=true)]
        public Vector2 Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public Vector2MaterialParamSettings(string name, Vector2 value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector2MaterialParam((Vector2MaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector2MaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Vector3 material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class Vector3MaterialParamSettings : MaterialParamSettings
    {
        private Vector3 mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public Vector3 Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public Vector3MaterialParamSettings(string name, Vector3 value)
            :base(name)
        {
            mValue = value;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector3MaterialParam((Vector3MaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector3MaterialParamSettings()
        {
        }

}

    /// <summary>
    /// Vector4 material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector4), typeof(FieldCustomBasicTypeProvider<Vector4>))]
    public class Vector4MaterialParamSettings : MaterialParamSettings
    {
        private Vector4 mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public Vector4 Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public Vector4MaterialParamSettings(string name, Vector4 value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector4MaterialParam((Vector4MaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector4MaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Matrix material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Matrix), typeof(FieldCustomBasicTypeProvider<Matrix>))]
    public class MatrixMaterialParamSettings : MaterialParamSettings
    {
        private Matrix mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public Matrix Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public MatrixMaterialParamSettings(string name, Matrix value)
            :base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new MatrixMaterialParam((MatrixMaterialEffectParam)effectParam, mValue);
        }

        /// <summary>
        /// for sof
        /// </summary>
        private MatrixMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Texture2D material param desc
    /// </summary>
    [DataObject]
    public class Texture2DMaterialParamSettings : MaterialParamSettings
    {
        private string mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public Texture2DMaterialParamSettings(string name, string value)
            : base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Texture2DMaterialParam((Texture2DMaterialEffectParam)effectParam, content.Load<Texture2D>(mValue));
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Texture2DMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Texture3D material param desc
    /// </summary>
    [DataObject]
    public class Texture3DMaterialParamSettings : MaterialParamSettings
    {
        private string mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public Texture3DMaterialParamSettings(string name, string value)
            : base(name)
        {
            mValue = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Texture3DMaterialParam((Texture3DMaterialEffectParam)effectParam, content.Load<Texture3D>(mValue));
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Texture3DMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// TextureCube material param desc
    /// </summary>
    [DataObject]
    public class TextureCubeMaterialParamSettings : MaterialParamSettings
    {
        private string mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataProperty(NotNull = true)]
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public TextureCubeMaterialParamSettings(string name, string value)
            : base(name)
        {
            mValue = value;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new TextureCubeMaterialParam((TextureCubeMaterialEffectParam)effectParam, content.Load<TextureCube>(mValue));
        }

        /// <summary>
        /// for sof
        /// </summary>
        private TextureCubeMaterialParamSettings()
        {
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                         Arrays
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Int[] material param desc
    /// </summary>
    [DataObject]
    public class IntArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<int> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<int> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public IntArrayMaterialParamSettings(string name, List<int> value)
            : base(name)
        {
            mValue = new List<int>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new IntArrayMaterialParam((IntArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private IntArrayMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Float material param desc
    /// </summary>
    [DataObject]
    public class FloatArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<float> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<float> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public FloatArrayMaterialParamSettings(string name, List<float> value)
            : base(name)
        {
            mValue = new List<float>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new FloatArrayMaterialParam((FloatArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private FloatArrayMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Bool material param desc
    /// </summary>
    [DataObject]
    public class BoolArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<bool> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<bool> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value"></param>
        public BoolArrayMaterialParamSettings(string name, IList<bool> value)
            : base(name)
        {
            mValue = new List<bool>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new BoolArrayMaterialParam((BoolArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private BoolArrayMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Vector2[] material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector2), typeof(FieldCustomBasicTypeProvider<Vector2>))]
    public class Vector2ArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<Vector2> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<Vector2> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector2ArrayMaterialParamSettings(string name)
            : base(name)
        {
            mValue = new List<Vector2>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector2ArrayMaterialParamSettings(string name, IList<Vector2> value)
            : base(name)
        {
            mValue = new List<Vector2>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector2ArrayMaterialParam((Vector2ArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector2ArrayMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Vector3[] material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class Vector3ArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<Vector3> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<Vector3> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector3ArrayMaterialParamSettings(string name)
            : base(name)
        {
            mValue = new List<Vector3>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector3ArrayMaterialParamSettings(string name, IList<Vector3> value)
            : base(name)
        {
            mValue = new List<Vector3>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector3ArrayMaterialParam((Vector3ArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector3ArrayMaterialParamSettings()
        {
        }

    }

    /// <summary>
    /// Vector4 material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Vector4), typeof(FieldCustomBasicTypeProvider<Vector4>))]
    public class Vector4ArrayMaterialParamSettings : MaterialParamSettings
    {
        private List<Vector4> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<Vector4> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector4ArrayMaterialParamSettings(string name)
            : base(name)
        {
            mValue = new List<Vector4>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public Vector4ArrayMaterialParamSettings(string name, IList<Vector4> value)
            : base(name)
        {
            mValue = new List<Vector4>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new Vector4ArrayMaterialParam((Vector4ArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private Vector4ArrayMaterialParamSettings()
        {
        }
    }

    /// <summary>
    /// Matrix[] material param desc
    /// </summary>
    [DataObject]
    [CustomBasicTypeProvider(typeof(Matrix), typeof(FieldCustomBasicTypeProvider<Matrix>))]
    public class MatrixArrayMaterialParamSettings : MaterialParamSettings
    {
         private List<Matrix> mValue;

        /// <summary>
        /// Value
        /// </summary>
        [DataListProperty(NotNull = true)]
        public List<Matrix> Value
        {
            get { return mValue; }
            private set { mValue = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public MatrixArrayMaterialParamSettings(string name)
            : base(name)
        {
            mValue = new List<Matrix>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">- Name of the parameter in effect file </param>
        /// <param name="value">- values are copied</param>
        public MatrixArrayMaterialParamSettings(string name, IList<Matrix> value)
            : base(name)
        {
            mValue = new List<Matrix>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectParam"></param>
        /// <returns></returns>
        internal override MaterialParam CreateMaterialParamImpl(MaterialEffectParam effectParam, ContentManager content)
        {
            return new MatrixArrayMaterialParam((MatrixArrayMaterialEffectParam)effectParam, mValue.ToArray());
        }

        /// <summary>
        /// for sof
        /// </summary>
        private MatrixArrayMaterialParamSettings()
        {
        }
    }
}
