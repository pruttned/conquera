using System;
using System.Collections.Generic;
using System.Text;
using Ale.Input;
using Ale;

namespace Conquera
{
    public class VictoryEvaluationGameSceneState : IGameSceneState
    {
        private GameScene mScene;

        public VictoryEvaluationGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            //todo
            mScene.State = mScene.States[typeof(IdleGameSceneState)];
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
