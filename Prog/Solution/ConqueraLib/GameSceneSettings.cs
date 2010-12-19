using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOrmFramework;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    internal class GameSceneSettings : BaseDataObject
    {
        [DataProperty(NotNull = true)]
        public long TerrainId { get; set; }

        /// <summary>
        /// Only fo ormManager.FindObject
        /// </summary>
        [DataProperty(NotNull = true)]
        private int Key { get; set; }

        public GameSceneSettings()
        {
            Key = 1;
        }
    }
}
