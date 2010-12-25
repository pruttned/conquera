using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Conquera
{
    [DataObject]
    public class StringResource : BaseDataObject
    {
        [DataProperty(NotNull=true, Unique=true)]
        public string Name { get; private set; }
        [DataProperty(NotNull = true)]
        public string Text { get; private set; }

        public StringResource(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private StringResource()
        {
        }
    }
}
