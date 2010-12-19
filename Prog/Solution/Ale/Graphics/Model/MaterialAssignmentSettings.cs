using System;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class MaterialAssignmentsettings : BaseDataObject
    {
        [DataProperty(NotNull=true, CaseSensitive=false)]
        public string MaterialGroup {get; set;}

        [DataProperty(NotNull = true, CaseSensitive = false)]
        public long Material { get; set; }

        public MaterialAssignmentsettings()
        {}

        public MaterialAssignmentsettings(string materialGroup, long material)
        {
            MaterialGroup = materialGroup;
            Material = material;
        }
    }
}
