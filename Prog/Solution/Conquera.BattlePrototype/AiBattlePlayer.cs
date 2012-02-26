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
using System.Windows.Media;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Conquera.BattlePrototype
{
    //public class AiBattlePlayer : BattlePlayer
    //{
    //    Dictionary<HexTerrainTile, AttackMapCell> mAttackMap = new Dictionary<HexTerrainTile, AttackMapCell>();
    //    Dictionary<HexTerrainTile, DefenseMapCell> mDefenseMap = new Dictionary<HexTerrainTile, DefenseMapCell>();
    //    HashSet<Microsoft.Xna.Framework.Point> mPossibleMoves = new HashSet<Microsoft.Xna.Framework.Point>();
    //    List<AttackMapCell> mUnitPossibleAtackPositions = new List<AttackMapCell>();
    //    List<BattleUnit> mAttackingUnits = new List<BattleUnit>();
    //    List<BattleUnit> mUnitsToMove = new List<BattleUnit>();
    //    List<DefenseMapCell> mPossibleDefensePositions = new List<DefenseMapCell>();

    //    Window1 mWindow;

    //    SimplePathFinder mPathFinder = new SimplePathFinder();

    //    public AiBattlePlayer(Color color, int index, Window1 window)
    //        : base(color, index)
    //    {
    //        mWindow = window;
    //    }
    //    protected override void OnTurnStartImpl(int turnNum, bool isActive)
    //    {
    //        if (isActive)
    //        {
    //            Stopwatch stp = new Stopwatch();
    //            stp.Start();

    //            Microsoft.Xna.Framework.Point center = new Microsoft.Xna.Framework.Point(9, 6);
    //            UpdateAttackMap();

    //            mAttackingUnits.Clear();
    //            mUnitsToMove.Clear();

    //            //attacks
    //            foreach (var unit in Units)
    //            {
    //                if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //                {
    //                    mPossibleMoves.Clear();
    //                    unit.GetPossibleMoves(mPossibleMoves);

    //                    if (0 < mPossibleMoves.Count)
    //                    {
    //                        mUnitPossibleAtackPositions.Clear();

    //                        //unit's possible attack positions
    //                        foreach (var pos in mPossibleMoves)
    //                        {
    //                            var tile = mWindow.Terrain[pos];
    //                            AttackMapCell attackMapCell;
    //                            if (mAttackMap.TryGetValue(tile, out attackMapCell))
    //                            {
    //                                if (attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.5 < unit.BaseDefense+unit.Hp)
    //                                {
    //                                    mUnitPossibleAtackPositions.Add(attackMapCell);
    //                                }
    //                            }
    //                        }
    //                        if (0 < mUnitPossibleAtackPositions.Count)
    //                        {
    //                            int i = MathExt.Random.Next(mUnitPossibleAtackPositions.Count);
    //                            unit.Move(mUnitPossibleAtackPositions[i].Tile.Index);
    //                            mAttackingUnits.Add(unit);
    //                        }
    //                        else
    //                        {
    //                            mUnitsToMove.Add(unit);
    //                        }
    //                    }
    //                }
    //            }

    //            if (0 < mUnitsToMove.Count)
    //            {
    //                if (0 < mAttackingUnits.Count)
    //                {
    //                    foreach (var attUnit in mAttackingUnits)
    //                    {
    //                        //max damage for unit
    //                        //abilities (first strike/berseker are not considered here)
    //                        int damage = 0;
    //                        int enemiesCnt = 0;
    //                        mWindow.Terrain.ForEachSibling(attUnit.TileIndex,
    //                            sibling =>
    //                            {
    //                                if (null != sibling.Unit && sibling.Unit.Player != this)
    //                                {
    //                                    damage += sibling.Unit.Attack.Y;
    //                                    enemiesCnt++;
    //                                }
    //                            });
    //                        int maxDamage = damage - attUnit.Defense;

    //                        mWindow.Terrain.ForEachSibling(attUnit.TileIndex,
    //                            sibling =>
    //                            {
    //                                if (sibling.IsPassableAndEmpty)
    //                                {
    //                                    DefenseMapCell defenseMapCell;
    //                                    if (!mDefenseMap.TryGetValue(sibling, out defenseMapCell))
    //                                    {
    //                                        defenseMapCell = new DefenseMapCell(sibling);
    //                                        mDefenseMap[sibling] = defenseMapCell;
    //                                    }

    //                                    defenseMapCell.DefenseReqIndex += maxDamage + enemiesCnt;
    //                                }
    //                            });
    //                    }
    //                }
    //                foreach (var unit in mUnitsToMove)
    //                {
    //                    if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //                    {
    //                        mPossibleMoves.Clear();
    //                        unit.GetPossibleMoves(mPossibleMoves);

    //                        if (0 != mDefenseMap.Count)
    //                        {
    //                            mPossibleDefensePositions.Clear();

    //                            //unit's possible defense positions
    //                            foreach (var pos in mPossibleMoves)
    //                            {
    //                                var tile = mWindow.Terrain[pos];
    //                                DefenseMapCell defenseMapCell;
    //                                if (mDefenseMap.TryGetValue(tile, out defenseMapCell))
    //                                {
    //                                    AttackMapCell attackMapCell;
    //                                    if (!mAttackMap.TryGetValue(tile, out attackMapCell) || attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.5 < unit.BaseDefense + unit.Hp)
    //                                    {
    //                                        mPossibleDefensePositions.Add(defenseMapCell);
    //                                    }
    //                                }
    //                            }
    //                            if (0 < mPossibleDefensePositions.Count)
    //                            {
    //                                mPossibleDefensePositions.OrderBy(d => d.DefenseReqIndex);
    //                                unit.Move(mPossibleDefensePositions[0].Tile.Index);
    //                                mDefenseMap.Remove(mPossibleDefensePositions[0].Tile);
    //                            }
    //                        }

    //                        if (!unit.HasMovedThisTurn)
    //                        {
    //                            var pathToCenter = mPathFinder.FindPath(unit.TileIndex, center, mWindow.Terrain);
    //                            if (null != pathToCenter)
    //                            {
    //                                Point pathPoint;
    //                                if (pathToCenter.Count > unit.MovementDistance)
    //                                {
    //                                    pathPoint = pathToCenter[unit.MovementDistance];
    //                                }
    //                                else
    //                                {
    //                                    pathPoint = pathToCenter[pathToCenter.Count - 1]; ;
    //                                }
    //                                Point movePoint = new Point();
    //                                int minDist = -1;
    //                                foreach (var possibleMove in mPossibleMoves)
    //                                {
    //                                    int dist = HexHelper.GetDistance(possibleMove, pathPoint);
    //                                    if (-1 == minDist || minDist > dist)
    //                                    {
    //                                        minDist = dist;
    //                                        movePoint = possibleMove;
    //                                    }
    //                                }
    //                                unit.Move(movePoint);
    //                            }
    //                        }
    //                    }
    //                }
    //            }



    //            //{
    //            //    List<BattleUnit> attackingUnits = new List<BattleUnit>();
    //            //    //  var enemiesInRange = new List<BattleUnit>();
    //            //    foreach (var unit in Units)
    //            //    {
    //            //        if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //            //        {
    //            //            //enemiesInRange.Clear();
    //            //            //unit.ForEachEnemyInRange(
    //            //            //    enemy => 
    //            //            //    {
    //            //            //        enemiesInRange.Add(enemy);
    //            //            //    });
    //            //            //if (0 < enemiesInRange.Count)
    //            //            //{

    //            //            //}
    //            //            //else
    //            //            {
    //            //                mPossibleMoves.Clear();
    //            //                unit.GetPossibleMoves(mPossibleMoves);

    //            //                if (0 < mPossibleMoves.Count)
    //            //                {
    //            //                    mUnitPossibleAtackPositions.Clear();

    //            //                    //unit's possible attack positions
    //            //                    foreach (var pos in mPossibleMoves)
    //            //                    {
    //            //                        var tile = mWindow.Terrain[pos];
    //            //                        AttackMapCell attackMapCell;
    //            //                        if (mAttackMap.TryGetValue(tile, out attackMapCell))
    //            //                        {
    //            //                            if (attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.7 < unit.BaseDefense)
    //            //                            {
    //            //                                mUnitPossibleAtackPositions.Add(attackMapCell);
    //            //                            }

    //            //                        }
    //            //                    }
    //            //                    if (0 < mUnitPossibleAtackPositions.Count)
    //            //                    {
    //            //                        int i = MathExt.Random.Next(mUnitPossibleAtackPositions.Count);
    //            //                        unit.Move(mUnitPossibleAtackPositions[i].Tile.Index);
    //            //                        attackingUnits.Add(unit);
    //            //                    }
    //            //                    else
    //            //                    {
    //            //                        //defense support
    //            //                        foreach (var attackingUnit in attackingUnits)
    //            //                        {

    //            //                        }



    //            //                        var pathToCenter = mPathFinder.FindPath(unit.TileIndex, center, mWindow.Terrain);
    //            //                        if (null != pathToCenter)
    //            //                        {
    //            //                            Point pathPoint;
    //            //                            if (pathToCenter.Count > unit.MovementDistance)
    //            //                            {
    //            //                                pathPoint = pathToCenter[unit.MovementDistance];
    //            //                            }
    //            //                            else
    //            //                            {
    //            //                                pathPoint = pathToCenter[pathToCenter.Count - 1]; ;
    //            //                            }
    //            //                            Point movePoint = new Point();
    //            //                            int minDist = -1;
    //            //                            foreach (var possibleMove in mPossibleMoves)
    //            //                            {
    //            //                                int dist = HexHelper.GetDistance(possibleMove, pathPoint);
    //            //                                if (-1 == minDist || minDist > dist)
    //            //                                {
    //            //                                    minDist = dist;
    //            //                                    movePoint = possibleMove;
    //            //                                }
    //            //                            }
    //            //                            unit.Move(movePoint);
    //            //                        }


    //            //                        //mPossibleMoves.Sort((a, b) => Comparer<int>.Default.Compare(Math.Abs(a.X - center.X) + Math.Abs(a.Y - center.Y), Math.Abs(b.X - center.X) + Math.Abs(b.Y - center.Y)));
    //            //                        //int i = 0;
    //            //                        //for (; i < mPossibleMoves.Count && MathExt.Random.NextDouble() < 0.5; ++i) { }
    //            //                        //if (mPossibleMoves.Count <= i)
    //            //                        //{
    //            //                        //    i = MathExt.Random.Next(mPossibleMoves.Count);
    //            //                        //}
    //            //                        //unit.Move(mPossibleMoves[i]);
    //            //                    }
    //            //                }
    //            //            }
    //            //        }
    //            //    }
    //            //}

    //            stp.Stop();
    //            Logger.Log(stp.ElapsedMilliseconds);

    //            mWindow.EndTurn();
    //        }
    //    }

    //    void UpdateAttackMap()
    //    {
    //        mAttackMap.Clear(); //todo AttackMapCell pool?
    //        foreach (var player in mWindow.Players)
    //        {
    //            if (player != this)
    //            {
    //                foreach (var unit in player.Units)
    //                {
    //                    mWindow.Terrain.ForEachSibling(unit.TileIndex,
    //                        sibling =>
    //                        {
    //                            if (sibling.IsPassableAndEmpty)
    //                            {
    //                                AttackMapCell attackMapCell;
    //                                if (!mAttackMap.TryGetValue(sibling, out attackMapCell))
    //                                {
    //                                    attackMapCell = new AttackMapCell(sibling);
    //                                    mAttackMap[sibling] = attackMapCell;
    //                                }
    //                                attackMapCell.MinDmg += unit.Attack.X;
    //                                attackMapCell.MaxDmg += unit.Attack.Y;
    //                                attackMapCell.Targets.Add(unit);
    //                            }
    //                        });
    //                }
    //            }
    //        }
    //    }


    //}

    //class AttackMapCell
    //{
    //    public HexTerrainTile Tile;
    //    public int MinDmg;
    //    public int MaxDmg;
    //    public List<BattleUnit> Targets;

    //    public AttackMapCell(HexTerrainTile tile)
    //    {
    //        Tile = tile;
    //        Targets = new List<BattleUnit>();
    //    }
    //}

    //class DefenseMapCell
    //{
    //    public HexTerrainTile Tile;
    //    public int DefenseReqIndex;

    //    public DefenseMapCell(HexTerrainTile tile)
    //    {
    //        Tile = tile;
    //    }
    //}




    public class AiBattlePlayer : BattlePlayer
    {
        Window1 mWindow;
        SimplePathFinder mPathFinder = new SimplePathFinder();
        InfluenceMap mInfluenceMap;
        public AiBattlePlayer(Window1 window, Color color, int index)
            : base(window, color, index)
        {
            mWindow = window;
            mInfluenceMap = new InfluenceMap(window.Terrain.Width, window.Terrain.Height, this);
        }
        protected override void OnTurnStartImpl(int turnNum, bool isActive)
        {
            if (isActive)
            {
                Stopwatch stp = new Stopwatch();
                stp.Start();

                var terrain = mWindow.Terrain;

                mInfluenceMap.UpdateMap(terrain);

                for (int i = 0; i < terrain.Width; ++i)
                {
                    for (int j = 0; j < terrain.Height; ++j)
                    {
                        var infCell = mInfluenceMap[i, j];
                        Color c = new Color();
                        c.A = 255;
                        c.R = (byte)Math.Min(infCell.EnemyPower * 255 * 2, 255);
                        c.B = (byte)Math.Min(infCell.FirendlyPower * 255 * 2, 255);

                        byte g = 0;
                        byte g2 = 0;
                        if (infCell.EnemyPower > 0.5f)
                        {
                            g = (byte)Math.Min((infCell.EnemyPower - 0.5f)*2 * 255, 255);
                        }
                        if (infCell.FirendlyPower > 0.5f)
                        {
                            g2 = (byte)Math.Min((infCell.FirendlyPower -0.5f)*2 * 255, 255);
                        }
                        c.G = Math.Max(g, g2);
                        terrain[i,j].OverlayBackground = new SolidColorBrush(c);
                        terrain[i, j].OverlayText = infCell.ToString();
                    }
                }

                //rotate units
                {
                    foreach (var unit in Units)
                    {
                        Point rotTarget = new Point();
                        float maxEnemyPower = 0;
                        terrain.ForEachSibling(unit.TileIndex, tile =>
                            {
                                var tileIndex = tile.Index;
                                var infCell = mInfluenceMap[tileIndex.X, tileIndex.Y];
                                if (maxEnemyPower < infCell.EnemyPower)
                                {
                                    maxEnemyPower = infCell.EnemyPower;
                                    rotTarget = tileIndex;
                                }
                            });
                        if (0 < maxEnemyPower)
                        {
                            unit.Direction = HexHelper.GetDirectionToSibling(unit.TileIndex, rotTarget); 
                        }
                    }
                }
                stp.Stop();
                Logger.Log(stp.ElapsedMilliseconds);
            }
        }
    }


    public class InfluenceMap
    {
        InfluenceMapCell[,] mCells2;
        InfluenceMapCell[,] mCells;
        BattlePlayer mPlayer;
        int mWidth;
        int mHeight;

        public InfluenceMapCell this[int i, int j]
        {
            get
            {
                return mCells[i, j];
            }
        }


        public InfluenceMap(int width, int height, BattlePlayer player)
        {
            mPlayer = player;
            mWidth = width;
            mHeight = height;

            mCells2 = new InfluenceMapCell[mWidth, mHeight];
            mCells = new InfluenceMapCell[mWidth, mHeight];
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    mCells2[i, j] = new InfluenceMapCell();
                    mCells[i, j] = new InfluenceMapCell();
                }
            }
        }

        public void UpdateMap(HexTerrain terrain)
        {
            ResetMap(mCells2);
            ResetMap(mCells);

            if (terrain.Width != mWidth || terrain.Height != mHeight)
            {
                throw new ArgumentException("Invalid terrain size");
            }



            float decay = 0.9f;
            float momentum = 0.5f;
            float baseV = 1f;
            for (int k = 0; k < 5; ++k)
            {

                //base values
                for (int i = 0; i < mWidth; ++i)
                {
                    for (int j = 0; j < mHeight; ++j)
                    {
                        var unit = terrain[i, j].Unit;
                        if (null != unit)
                        {
                            if (mPlayer == unit.Player) //friendly
                            {
                                mCells[i, j].FirendlyPower = baseV;
                            }
                            else //enemy
                            {
                                mCells[i, j].EnemyPower = baseV;
                            }
                        }
                    }
                }

                //propagate
                Point index = new Point();
                for (; index.X < mWidth; ++index.X)
                {
                    for (index.Y = 0; index.Y < mHeight; ++index.Y)
                    {
                        if (terrain[index].IsPassable)
                        {
                            float maxEnemyPower = 0;
                            float maxFriendlyPower = 0;
                            terrain.ForEachSibling(index, tile =>
                                {
                                    if (tile.IsPassable)
                                    {
                                        var siblingIndex = tile.Index;
                                        var siblingInfCell = mCells[siblingIndex.X, siblingIndex.Y];
                                        
                                        float enemyPower = siblingInfCell.EnemyPower * decay;
                                        if (maxEnemyPower < enemyPower)
                                        {
                                            maxEnemyPower = enemyPower;
                                        }

                                        float friendlyPower = siblingInfCell.FirendlyPower * decay;
                                        if (maxFriendlyPower < friendlyPower)
                                        {
                                            maxFriendlyPower = friendlyPower;
                                        }
                                    }
                                });
                            mCells2[index.X, index.Y].EnemyPower = MathHelper.Lerp(mCells[index.X, index.Y].EnemyPower, maxEnemyPower, momentum);
                            mCells2[index.X, index.Y].FirendlyPower = MathHelper.Lerp(mCells[index.X, index.Y].FirendlyPower, maxFriendlyPower, momentum);
                            //mCells2[index.X, index.Y].EnemyPower = mCells[index.X, index.Y].EnemyPower + maxEnemyPower*momentum;
                            //mCells2[index.X, index.Y].FirendlyPower = mCells[index.X, index.Y].FirendlyPower + maxFriendlyPower* momentum;
                        }
                    }
                }
                 
                //swap buffers
                var auxCells = mCells;
                mCells = mCells2;
                mCells2 = auxCells;
            }


            //base values
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    var unit = terrain[i, j].Unit;
                    if (null != unit)
                    {
                        if (mPlayer == unit.Player) //friendly
                        {
                            mCells[i, j].FirendlyPower = baseV;
                        }
                        else //enemy
                        {
                            mCells[i, j].EnemyPower = baseV;
                        }
                    }
                }
            }
        }

        private void ResetMap(InfluenceMapCell[,] map)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    map[i, j].Reset();
                }
            }
        }
    }

    public class InfluenceMapCell
    {
        public float FirendlyPower { get; set; }
        public float EnemyPower { get; set; }

        public void Reset()
        {
            FirendlyPower = 0.0f;
            EnemyPower = 0.0f;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", FirendlyPower.ToString(), EnemyPower.ToString());
        }
    }
}
