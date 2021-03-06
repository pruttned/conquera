////////////////////////////////////////////////////////////////////////
////  Copyright (C) 2010 by Conquera Team
////  Part of the Conquera Project
////
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 2 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SimpleOrmFramework;

//namespace Conquera
//{
//    [DataObject(MaxCachedCnt = 0)]
//    public class GameSceneContextState : BaseDataObject
//    {
//        private BattleScene mGameScene;

//        [DataProperty(NotNull = true)]
//        public int TurnNum { get; set; }

//        [DataProperty(NotNull = true)]
//        public int CurrentPlayerIndex { get; set; }

//        /// <summary>
//        /// Only fo ormManager.FindObject
//        /// </summary>
//        [DataProperty(NotNull = true)]
//        private int Key { get; set; }

//        [DataProperty(NotNull = true)]
//        public string GameMap { get; set; }

//        [DataListProperty(NotNull = true)]
//        public List<GamePlayer> Players { get; private set; }

//        public GamePlayer CurrentPlayer
//        {
//            get { return Players[CurrentPlayerIndex]; }
//        }

//        public GameSceneContextState()
//        {
//            Key = 1;
//            Players = new List<GamePlayer>();
//        }

//        public void Init(BattleScene scene)
//        {
//            mGameScene = scene;
//        }

//        public void EndTurn()
//        {
//            TurnNum++;

//            CurrentPlayerIndex++;
//            if (CurrentPlayerIndex >= Players.Count)
//            {
//                CurrentPlayerIndex = 0;
//            }
//        }

//        protected override void BeforeSaveImpl(OrmManager ormManager)
//        {
//            base.BeforeSaveImpl(ormManager);
//        }
//    }

//}
