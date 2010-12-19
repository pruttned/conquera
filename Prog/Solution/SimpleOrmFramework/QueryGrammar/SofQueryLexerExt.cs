using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOrmFramework
{
    /// <summary>
    /// SofQueryLexer ext
    /// </summary>
    internal partial class SofQueryLexer
    {
        public override void EmitErrorMessage(string msg)
        {
            throw new SofQueryParsingException(string.Format("Syntax error in sofQuery: \"{0}\"", msg));
        }
    }
}
