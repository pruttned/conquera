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
using System.Linq;
using System.Text;
using Ale.Input;
using Microsoft.Xna.Framework;
using Ale.Gui;
using Ale;
using Conquera.Gui;

namespace Conquera
{
    public class ReadyGameUnitSelectedGameSceneState : IGameSceneState
    {
        private GameScene mScene;
        private GameUnit SelectedUnit
        {
            get { return mScene.SelectedUnit; }
        }
        private MovementArrow MovementArrow
        {
            get { return mScene.MovementArrow; }
        }

        public ReadyGameUnitSelectedGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            if (null == SelectedUnit) //hotfix
            {
                mScene.State = mScene.States[typeof(IdleGameSceneState)];
            }

            mScene.EnableMouseCameraControl = true;
            MovementArrow.IsVisible = true;
            MovementArrow.StartCell = mScene.GetCell(SelectedUnit.CellIndex);
        }

        public void OnEnd()
        {
            MovementArrow.IsVisible = false;
        }

        public void OnClickOnCell(HexCell cellUnderCur, MouseButton button)
        {
            if (MouseButton.Right == button && mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                if (SelectedUnit.MoveTo(cellUnderCur.Index))
                {
                    mScene.State = mScene.States[typeof(UnitMovingGameSceneState)];
                }
                else
                {
                    if (SelectedUnit.Attack(cellUnderCur.Index))
                    {
                        mScene.State = mScene.States[typeof(BattleGameSceneState)];
                    }
                }
            }
            else
            {
                if (MouseButton.Left == button)
                {
                    mScene.SelectedCell = cellUnderCur;
                    var selectedUnit = mScene.SelectedUnit;
                    mScene.State = mScene.States[typeof(IdleGameSceneState)];
                }
            }
        }

        public void Update(AleGameTime gameTime)
        {
            if (null == mScene.SelectedUnit) //e.g. unit has died
            {
                mScene.State = mScene.States[typeof(IdleGameSceneState)];
            }
            else
            {
                var cellUnderCur = mScene.GetCellUnderCur();

                if (null != cellUnderCur)
                {
                    MovementArrow.EndCell = cellUnderCur;
                    if (SelectedUnit.HasMovedThisTurn || !SelectedUnit.CanMoveTo(cellUnderCur.Index))
                    {
                        if (!SelectedUnit.HasAttackedThisTurn && SelectedUnit.CanAttackTo(cellUnderCur.Index))
                        {
                            MovementArrow.IsVisible = true;
                            Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Attack;
                            MovementArrow.Color = new Vector3(1, 0.1f, 0.1f);
                        }
                        else
                        {
                            MovementArrow.IsVisible = false;
                            Ale.Gui.GuiManager.Instance.Cursor = AlCursors.MoveDisabled;
                        }
                    }
                    else
                    {
                        MovementArrow.IsVisible = true;
                        Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Move;
                        MovementArrow.Color = Vector3.One;
                    }
                }
                else
                {
                    MovementArrow.IsVisible = false;
                    Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Default;
                }
            }
        }
    }

}
