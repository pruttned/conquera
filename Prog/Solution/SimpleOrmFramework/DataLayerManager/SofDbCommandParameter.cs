using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    internal class SofDbCommandParameter
    {
        public string Name;
        public object Value;
        public string Column;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="column">-Column name to which is this parameter bind</param>
        public SofDbCommandParameter(string name, object value, string column)
        {
            Name = name;
            Value = value;
            Column = column;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SQLiteParameter CreateRealDbParameter()
        {
            return new SQLiteParameter(Name, Value);
        }
    }
}
