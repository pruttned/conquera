using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    interface IDataPropertyAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="customBasicTypeProviders"></param>
        /// <returns></returns>
        IPropertyMetaInfo CreatePropertyMetaInfo(PropertyInfo propertyInfo, Dictionary<Type, ICustomBasicTypeProvider> customBasicTypeProviders);
    }
}
