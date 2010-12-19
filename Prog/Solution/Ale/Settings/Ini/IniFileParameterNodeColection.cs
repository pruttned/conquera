using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public class IniFileParameterNodeColection : IniFileNodeColection<IniFileParameterNode>
    {
        public IniFileParameterNode Add(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var node = new IniFileParameterNode(name, value);
            Add(node);
            return node;
        }
    }

}
