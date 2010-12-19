using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    static class AttributeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T FindAttribute<T>(MemberInfo type, bool inherit)
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), inherit);
            if (null == attributes || 0 == attributes.Length)
            {
                return default(T);
            }

            return (T)attributes[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T FindAttribute<T>(MemberInfo type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            if (null == attributes || 0 == attributes.Length)
            {
                return default(T);
            }

            return (T)attributes[0];
        }
    }

}
