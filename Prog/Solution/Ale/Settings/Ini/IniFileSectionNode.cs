using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public class IniFileSectionNode : IniFileNode
    {
        public IniFileParameterNodeColection Parameters { get; private set; }

        public IniFileSectionNode(string name)
            : base(name)
        {
            Parameters = new IniFileParameterNodeColection();
        }
    }
}
