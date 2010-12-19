using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;

namespace Ale.Graphics
{
    public class ConnectionPointAssigmentDesc
    {
        public NameId ConnectionPoint { get; private set; }
        public long Renderable { get; private set; }
        public int RenderableFactory { get; private set; }

        public ConnectionPointAssigmentDesc(ConnectionPointAssigmentSettings settings)
        {
            ConnectionPoint = settings.ConnectionPoint;
            Renderable = settings.Renderable;
            RenderableFactory = settings.RenderableFactory;
        }
    }
}
