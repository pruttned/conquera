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
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;


namespace Conquera
{
    public sealed class UnitAttackAreaRenderable : IDisposable
    {
        private BattleScene mGameScene;
        private HexAreaRenderable mHexAreaRenderable = null;
        private BattleUnit mLastAttacker = null;
        private BattleUnit mLastTarget = null;
        private bool mIsDisposed = false;
        private Material mMaterial;

        public UnitAttackAreaRenderable(BattleScene gameScene)
        {
            mGameScene = gameScene;
            mMaterial = gameScene.Content.Load<Material>("AttackAreaMat");
        }

        public void Show(BattleUnit attacker, BattleUnit target)
        {
            //todo
            throw new NotImplementedException();
            //if (attacker != mLastAttacker || target != mLastTarget)
            //{
            //    mLastAttacker = attacker;
            //    mLastTarget = target;

            //    Vector4 fullColor = new Vector4(Color.Red.ToVector3(), 0.9f);
            //    Vector4 faintColor = new Vector4(Color.Red.ToVector3(), 0.3f);

            //    if (null != mHexAreaRenderable)
            //    {
            //        mGameScene.Octree.DestroyObject(mHexAreaRenderable);
            //    }
            //    var targets = attacker.GetAdditionalAttackTargets(target);
            //    List<HexAreaRenderableCell> cells = new List<HexAreaRenderableCell>(targets.Count);
            //    foreach (var t in targets)
            //    {
            //        var targetUnit = mGameScene.GetCell(t.Position).GameUnit;
            //        if (null != targetUnit && targetUnit.OwningPlayer != attacker.OwningPlayer)
            //        {
            //            cells.Add(new HexAreaRenderableCell(t.Position, fullColor));
            //        }
            //        else
            //        {
            //            cells.Add(new HexAreaRenderableCell(t.Position, faintColor));
            //        }
            //    }
            //    mHexAreaRenderable = new HexAreaRenderable(mGameScene.GraphicsDeviceManager.GraphicsDevice, cells, mMaterial);
            //    mGameScene.Octree.AddObject(mHexAreaRenderable);
            //}
        }

        public void Hide()
        {
            if (null != mHexAreaRenderable)
            {
                mGameScene.Octree.DestroyObject(mHexAreaRenderable);
                mHexAreaRenderable = null;

                mLastTarget = null;
                mLastAttacker = null;
            }
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                Hide();

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

    }
    public sealed class UnitActionAreaRenderable : IDisposable
    {
        private static List<HexTerrainTile> Siblings = new List<HexTerrainTile>();
        private static List<Point> PosibleMoves = new List<Point>();

        private HexAreaRenderable mHexAreaRenderable = null;
        private BattleScene mGameScene;
        private bool mIsDisposed = false;
        private BattleUnit mLastUnit = null;

        public UnitActionAreaRenderable(BattleScene gameScene)
        {
            mGameScene = gameScene;
        }

        public void Show(BattleUnit unit)
        {
            if (mLastUnit != unit)
            {
                mLastUnit = unit;

                if (null != mHexAreaRenderable)
                {
                    mGameScene.Octree.DestroyObject(mHexAreaRenderable);
                }

                var cells = InitCells(unit);
                if (0 != cells.Count)
                {
                    mHexAreaRenderable = new HexAreaRenderable(mGameScene.GraphicsDeviceManager.GraphicsDevice, cells, mGameScene.Content);
                    mGameScene.Octree.AddObject(mHexAreaRenderable);
                }
            }
        }

        public void Hide()
        {
            if (null != mHexAreaRenderable)
            {
                mGameScene.Octree.DestroyObject(mHexAreaRenderable);
                mHexAreaRenderable = null;
            }
            mLastUnit = null;
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                Hide();

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        private static IList<HexAreaRenderableCell> InitCells(BattleUnit unit)
        {
            //todo
            throw new NotImplementedException();

            //List<HexAreaRenderableCell> cells = new List<HexAreaRenderableCell>();

            //Vector4 moveColor = new Vector4(Color.Green.ToVector3(), 0.8f);
            //Vector4 attackColor = new Vector4(Color.Red.ToVector3(), 0.8f);

            //if (!unit.HasMovedThisTurn)
            //{
            //    PosibleMoves.Clear();
            //    unit.GetPossibleMoves(PosibleMoves);
            //    foreach (var index in PosibleMoves)
            //    {
            //        cells.Add(new HexAreaRenderableCell(index, moveColor));
            //    }
            //}
            //if (!unit.HasAttackedThisTurn)
            //{
            //    Siblings.Clear();
            //    unit.BattleScene.Terrain.GetSiblings(unit.Tile.Index, Siblings);
            //    foreach (var cell in Siblings)
            //    {
            //        if (null != cell.GameUnit && cell.OwningPlayer != unit.OwningPlayer)
            //        {
            //            cells.Add(new HexAreaRenderableCell(cell.Index, attackColor));
            //        }
            //    }
            //}
            //return cells;
        }
    }
}
