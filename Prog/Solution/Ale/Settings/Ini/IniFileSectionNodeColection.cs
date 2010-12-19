using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public class IniFileSectionNodeColection : IniFileNodeColection<IniFileSectionNode>
    {
        public IniFileSectionNode Add(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var node = new IniFileSectionNode(name);
            Add(node);
            return node;
        }
    }

}
