using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOrmFramework;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    public class GameSceneContextState : BaseDataObject
    {
        private GameScene mGameScene;

        [DataProperty(NotNull = true)]
        public int TurnNum { get; set; }

        [DataProperty(NotNull = true)]
        public int CurrentPlayerIndex { get; set; }

        /// <summary>
        /// Only fo ormManager.FindObject
        /// </summary>
        [DataProperty(NotNull = true)]
        private int Key { get; set; }

        [DataProperty(NotNull = true)]
        public string GameMap { get; set; }

        [DataListProperty(NotNull = true)]
        public List<GamePlayer> Players { get; private set; }

        public GamePlayer CurrentPlayer
        {
            get { return Players[CurrentPlayerIndex]; }
        }

        public GameSceneContextState()
        {
            Key = 1;
            Players = new List<GamePlayer>();
        }

        public void Init(GameScene scene)
        {
            mGameScene = scene;
        }

        public void EndTurn()
        {
            TurnNum++;

            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= Players.Count)
            {
                CurrentPlayerIndex = 0;
            }
        }

        protected override void BeforeSaveImpl(OrmManager ormManager)
        {
            base.BeforeSaveImpl(ormManager);
        }
    }

}
