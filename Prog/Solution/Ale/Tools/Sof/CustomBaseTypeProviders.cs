using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Ale.Tools
{
    /// <summary>
    /// Takes all public fields of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FieldCustomBasicTypeProvider<T> : ICustomBasicTypeProvider where T :  new()
    {
        #region ICustomBasicTypeProvider Members

        public MemberInfo[] GetDataProperties()
        {
            return Array.ConvertAll<FieldInfo, MemberInfo>(typeof(T).GetFields(), 
                delegate(FieldInfo field)
                {
                    return (MemberInfo)field;
                });
        }

        public object CreateInstance()
        {
            return new T();
        }

        #endregion
    }

    /// <summary>
    /// Takes all public properties of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyCustomBasicTypeProvider<T> : ICustomBasicTypeProvider where T : new()
    {
        #region ICustomBasicTypeProvider Members

        public MemberInfo[] GetDataProperties()
        {
            return Array.ConvertAll<PropertyInfo, MemberInfo>(typeof(T).GetProperties(),
                delegate(PropertyInfo field)
                {
                    return (MemberInfo)field;
                });
        }

        public object CreateInstance()
        {
            return new T();
        }

        #endregion
    }

}
