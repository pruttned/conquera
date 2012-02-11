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
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;

namespace Conquera
{
    public class ReadyGameUnitSelectedGameSceneState : IGameSceneState
    {
        private BattleScene mScene;
        private BattleUnit SelectedUnit
        {
            get { return mScene.SelectedUnit; }
        }
        private MovementArrow MovementArrow
        {
            get { return mScene.MovementArrow; }
        }

        public ReadyGameUnitSelectedGameSceneState(BattleScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            mScene.Show3dCursor = true;
            if (null == SelectedUnit) //hotfix
            {
                mScene.State = mScene.GetGameSceneState(GameSceneStates.Idle);
            }

            mScene.UnitActionAreaRenderable.Show(SelectedUnit);

            mScene.EnableMouseCameraControl = true;
            MovementArrow.StartCell = mScene.Terrain[SelectedUnit.TileIndex];
        }

        public void OnEnd()
        {
            mScene.UnitAttackAreaRenderable.Hide();
            mScene.UnitActionAreaRenderable.Hide();

            MovementArrow.IsVisible = false;
        }

        public void OnClickOnTile(HexTerrainTile tileUnderCur, MouseButton button)
        {
            if (MouseButton.Right == button)
            {
                if (null != tileUnderCur)
                {
                    if (SelectedUnit.MoveTo(tileUnderCur.Index))
                    {
                        mScene.State = mScene.GetGameSceneState(GameSceneStates.UnitMoving);
                    }
                    else
                    {
                        if (SelectedUnit.Attack(tileUnderCur.Index))
                        {
                            mScene.State = mScene.GetGameSceneState(GameSceneStates.Battle);
                        }
                    }
                }
            }
            else
            {
                if (MouseButton.Left == button)
                {
                    mScene.SelectedTile = tileUnderCur;
                    var selectedUnit = mScene.SelectedUnit;
                    mScene.State = mScene.GetGameSceneState(GameSceneStates.Idle);
                }
            }

        }

        public void Update(AleGameTime gameTime)
        {
            //todo
            throw new NotImplementedException();

            //if (null == mScene.SelectedUnit) //e.g. unit has died
            //{
            //    mScene.State = mScene.GetGameSceneState(GameSceneStates.Idle);
            //}
            //else
            //{
            //    var cellUnderCur = mScene.GetTileUnderCur();

            //    if (null != cellUnderCur && !GuiManager.Instance.HandlesMouse)
            //    {
            //        MovementArrow.EndCell = cellUnderCur;
            //        if (SelectedUnit.HasMovedThisTurn || !SelectedUnit.CanMoveTo(cellUnderCur.Index))
            //        {
            //            if (!SelectedUnit.HasAttackedThisTurn && SelectedUnit.CanAttackTo(cellUnderCur.Index))
            //            {
            //                mScene.UnitAttackAreaRenderable.Show(SelectedUnit, cellUnderCur.GameUnit);

            //                MovementArrow.IsVisible = true;
            //                Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Attack;
            //                MovementArrow.Color = new Vector3(1, 0.1f, 0.1f);
            //            }
            //            else
            //            {
            //                mScene.UnitAttackAreaRenderable.Hide();
            //                MovementArrow.IsVisible = false;
            //                Ale.Gui.GuiManager.Instance.Cursor = AlCursors.MoveDisabled;
            //            }
            //        }
            //        else
            //        {
            //            mScene.UnitAttackAreaRenderable.Hide();
            //            MovementArrow.IsVisible = true;
            //            Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Move;
            //            MovementArrow.Color = Vector3.One;
            //        }
            //    }
            //    else
            //    {
            //        mScene.UnitAttackAreaRenderable.Hide();
            //        MovementArrow.IsVisible = false;
            //        Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Default;
            //    }
            //}
        }
    }
}
