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
