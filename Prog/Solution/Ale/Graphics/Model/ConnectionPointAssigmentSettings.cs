using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class ConnectionPointAssigmentSettings : BaseDataObject
    {
        [DataProperty(CaseSensitive = false, NotNull = true)]
        public string ConnectionPoint { get; set; }
        [DataProperty]
        public long Renderable { get; set; }
        [DataProperty]
        public int RenderableFactory { get; set; }

        public ConnectionPointAssigmentSettings()
        {
        }

        public ConnectionPointAssigmentSettings(string connectionPoint, long renderable, int renderableFactory)
        {
            ConnectionPoint = connectionPoint;
            Renderable = renderable;
            RenderableFactory = renderableFactory;
        }
    }
}
