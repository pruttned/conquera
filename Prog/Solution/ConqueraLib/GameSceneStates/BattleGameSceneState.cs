using System;
using System.Collections.Generic;
using System.Text;
using Ale.Input;
using Ale;

namespace Conquera
{
    public class BattleGameSceneState : IGameSceneState
    {
        private GameScene mScene;

        public BattleGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            if (null == mScene.SelectedUnit)
            {
                mScene.State = mScene.States[typeof(VictoryEvaluationGameSceneState)];
            }
            else
            {
                mScene.EnableMouseCameraControl = true;
            }
        }

        public void OnEnd()
        {
        }

        public void OnClickOnCell(HexCell cellUnderCur, MouseButton button)
        {
        }

        public void Update(AleGameTime gameTime)
        {
            if (null == mScene.SelectedUnit || mScene.SelectedUnit.IsIdle)
            {
                mScene.State = mScene.States[typeof(VictoryEvaluationGameSceneState)];
            }
        }
    }

}
