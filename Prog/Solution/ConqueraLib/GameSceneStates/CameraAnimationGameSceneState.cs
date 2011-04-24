//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

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

            if (null != mScene.SelectedUnit)
            {
                mScene.ShowMovementArea(mScene.SelectedUnit);
            }

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
