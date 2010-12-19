using System;
using System.Collections.Generic;
using System.Text;
using Ale.Input;
using Ale;

namespace Conquera
{
    public class CameraAnimationGameSceneState : IGameSceneState
    {
        private GameScene mScene;

        private IGameSceneState mPreviousGameSceneState;

        public IGameSceneState PreviousGameSceneState
        {
            get { return mPreviousGameSceneState; }
            set
            {
                if (value.GetType() == typeof(CameraAnimationGameSceneState))
                {
                    throw new ArgumentException("PreviousGameSceneState can't be CameraAnimationGameSceneState"); //TODO: ked sa kamera animuje(zoom) a dam end turn cez medzeru, tak sem spadne
                }
                mPreviousGameSceneState = value;
            }
        }

        public CameraAnimationGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            mScene.EnableMouseCameraControl = false;

            if (null == PreviousGameSceneState) throw new ArgumentNullException("PreviousGameSceneState");
        }

        public void OnEnd()
        {
        }

        public void OnClickOnCell(HexCell cellUnderCur, MouseButton button)
        {
        }

        public void Update(AleGameTime gameTime)
        {
            if (!mScene.GameCamera.IsAnimating)
            {
                mScene.State = PreviousGameSceneState;
            }
        }
    }

}
