using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Ale.Sound
{
    /// <summary>
    /// Base exception
    /// </summary>
    [Serializable]
    public class SoundSubsystemException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SoundSubsystemException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public SoundSubsystemException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SoundSubsystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
