using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public class IniFileParameterNode : IniFileNode
    {
        public string Value { get; set; }

        public IniFileParameterNode(string name)
            : this(name, null)
        {
        }
        public IniFileParameterNode(string name, string value)
            : base(name)
        {
            Value = value;
        }
    }
}
