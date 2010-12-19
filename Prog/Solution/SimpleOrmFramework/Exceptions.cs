using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Base exception
    /// </summary>
    [Serializable]
    public class SofException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SofException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public SofException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SofException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Error in Sof query
    /// </summary>
    [Serializable]
    public class SofQueryParsingException : SofException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SofQueryParsingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public SofQueryParsingException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SofQueryParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Type is not supported by Sof
    /// </summary>
    [Serializable]
    public class UnsupportedTypeException : SofException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UnsupportedTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public UnsupportedTypeException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UnsupportedTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Weak reference was not initialized before saving the data object
    /// </summary>
    [Serializable]
    public class WeakReferenceNotInitializedException : SofException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WeakReferenceNotInitializedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public WeakReferenceNotInitializedException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WeakReferenceNotInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
