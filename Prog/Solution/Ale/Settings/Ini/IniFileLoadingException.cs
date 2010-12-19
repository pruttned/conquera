using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Ale.Settings
{
    [Serializable]
    public class IniFileLoadingException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public IniFileLoadingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public IniFileLoadingException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected IniFileLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
