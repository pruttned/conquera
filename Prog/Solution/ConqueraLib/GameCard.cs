using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Gui;
using SimpleOrmFramework;

namespace Conquera
{
    //[DataObject(MaxCachedCnt = 0)]
    public class GameCard //: BaseDataObject
    {
        public enum GameCardColors { Black, Green, Purple }

        public GraphicElement Picture { get; set; }
        public GraphicElement Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttackPurple { get; set; }
        public int AttackGreen { get; set; }
        public int AttackBlack { get; set; }
        public int DefensePurple { get; set; }
        public int DefenseGreen { get; set; }
        public int DefenseBlack { get; set; }
    }

}
