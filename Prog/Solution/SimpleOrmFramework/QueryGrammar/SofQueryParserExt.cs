using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleOrmFramework
{
    /// <summary>
    /// SofQueryParser ext
    /// </summary>
    internal partial class SofQueryParser
    {
        public override void EmitErrorMessage(string msg)
        {
            throw new SofQueryParsingException(string.Format("Syntax error in sofQuery: \"{0}\"", msg));
        }
    }
}
