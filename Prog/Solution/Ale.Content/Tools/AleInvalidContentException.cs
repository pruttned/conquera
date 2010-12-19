using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Ale.Content.Tools
{
    class AleInvalidContentException : InvalidContentException
    {
        #region Methods

        public AleInvalidContentException(string message)
            : base(message)
        {
        }

        public AleInvalidContentException(string message, Exception innerException)
            : base(BuildFullMessage(message, innerException))
        {
        }

        private static string BuildFullMessage(string message, Exception innerException)
        {
            return string.Format("{0}\n==========Inner error============\n{1}", message, innerException.ToString());
        }

        #endregion Methods
    }
}
