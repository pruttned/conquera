using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Custom basic type provider
    /// </summary>
    public interface ICustomBasicTypeProvider
    {
        /// <summary>
        /// Gets properties or fields of basic type that should be stored in db
        /// </summary>
        /// <returns></returns>
        MemberInfo[] GetDataProperties();

        /// <summary>
        /// Create uninicialized instance of basic type
        /// </summary>
        /// <returns></returns>
        object CreateInstance();
    }
}
