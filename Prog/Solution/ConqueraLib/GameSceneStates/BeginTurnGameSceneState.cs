using System;
using System.Collections.Generic;
using System.Text;
using Ale.Input;
using Ale;

namespace Conquera
{
    public class BeginTurnGameSceneState : IGameSceneState
    {
        private GameScene mScene;

        public BeginTurnGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
        }

        public void OnEnd()
        {
        }

        public void OnClickOnCell(HexCell cellUnderCur, MouseButton button)
        {
        }

        public void Update(AleGameTime gameTime)
        {
        }
    }
}
