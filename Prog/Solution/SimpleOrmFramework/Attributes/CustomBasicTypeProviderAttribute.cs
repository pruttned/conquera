using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Defines provider for a basic type (complex type that isn't data object - for instance Point, Rect, Vector3, ...).
    /// Custom basic type must be value type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomBasicTypeProviderAttribute : Attribute
    {
        private Type mBasicType;
        private Type mProviderType;

        /// <summary>
        /// Gets the basic type
        /// </summary>
        public Type BasicType
        {
            get { return mBasicType; }
        }

        /// <summary>
        /// Gets the provider for basic type
        /// </summary>
        public Type ProviderType
        {
            get{return mProviderType;}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicType"></param>
        /// <param name="providerType">- Must implement ICustomBasicTypeProvider</param>
        public CustomBasicTypeProviderAttribute(Type basicType, Type providerType)
        {
            if (!typeof(ICustomBasicTypeProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException(string.Format("Type '{0}' doesn't implement '{1}'", basicType.FullName, typeof(ICustomBasicTypeProvider).FullName));
            }

            mBasicType = basicType;
            mProviderType = providerType;
        }
    }
}
