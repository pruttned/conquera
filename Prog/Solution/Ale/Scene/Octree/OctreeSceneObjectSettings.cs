using System;
using SimpleOrmFramework;

namespace Ale.Scene
{
    [DataObject(MaxCachedCnt = 5)]
    public class OctreeSceneObjectSettings  : BaseDataObject
    {
        [DataProperty(Unique = true, NotNull = true)]
        public string Name { get; set; }
        [DataProperty]
        public long GraphicModel { get; set; }
    }
}
