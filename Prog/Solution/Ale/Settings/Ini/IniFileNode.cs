using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public abstract class IniFileNode
    {
        public string Name { get; private set; }
        public List<string> Comments { get; private set; }

        public IniFileNode(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            Name = name;
            Comments = new List<string>();
        }
    }
}
