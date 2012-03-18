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
        List<UnitMove> mMovements = new List<UnitMove>();

        public InfluenceMap InfluenceMap
        {
            get { return mInfluenceMap; }
        }

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

                mMovements.Clear();
                int unitsToMove = Units.Count;

                foreach (var unit in Units)
                {
                    ResolvePossibleDirections(unit, terrain[unit.TileIndex]);
                    if (unit.HasEnabledMovement && 0 < unit.MovementDistance)
                    {
                        unit.ForEachPassableCellInRange(unit.MovementDistance, OccupationIgnoreMode.IgnoreFriendly, tile =>
                            {
                                ResolvePossibleDirections(unit, tile);
                            });
                    }
                }
                mMovements.Sort();
                
                bool reset = false;
                do
                {
                    reset = false;
                    for (int i = mMovements.Count - 1; i >= 0 && 0 < unitsToMove && !reset; --i)
                    {
                        var movement = mMovements[i];
                        if (movement.Unit.HasMovedThisTurn)
                        {
                            mMovements.RemoveAt(i);
                        }
                        else
                        {
                            if (mMovements[i].CheckMovementPossibility())
                            {
                                mMovements[i].Execute(turnNum);
                                mMovements.RemoveAt(i);
                                reset = true;
                                unitsToMove--;
                            }
                        }
                        //     mMovements[i]
                    }
                } while (reset);

                //rotate units
                {
                    foreach (var unit in Units)
                    {
                        Point rotTarget = new Point();
                        float maxEnemyPower = 0;
                        terrain.ForEachSibling(unit.TileIndex, (tile, dir) =>
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

        //temp
        private void ResolvePossibleDirections(BattleUnit unit, HexTerrainTile tile)
        {
            //todo cache attack targets based on unit's type
            for (int i = 0; i <= 5; ++i)
            {
                HexDirection direction = (HexDirection)i;
                float attackDamage = 0;
                unit.ForEachAttackTarget(tile.Index, direction, (targetUnit, attType) =>
                {
                    float subAttackDamage = unit.GetDieAgainst(targetUnit.GetType()).HitProbability;
                    if (AttackType.Main == attType)
                    {
                        subAttackDamage *= 2;
                    }
                    attackDamage += subAttackDamage;
                }); 
                float hitProbability = (unit is Cavalry ? mInfluenceMap[tile.Index].HitProbabilityAgainstCavalryUnit : mInfluenceMap[tile.Index].HitProbabilityAgainstFootUnit);
                float priority = attackDamage - hitProbability;
                if (0 < attackDamage)
                {
                    mMovements.Add(new UnitMove(unit, tile, direction, priority));
                }
            }
        }
    }

    public class UnitMove : IComparable<UnitMove>
    {
        public BattleUnit Unit { get; private set; }
        public HexDirection Direction { get; private set; }
        public HexTerrainTile TargetTile { get; private set; }
        public float Priority { get; private set; }

        public UnitMove(BattleUnit unit, HexTerrainTile targetTile, HexDirection direction, float priority)
        {
            Unit = unit;
            TargetTile = targetTile;
            Priority = priority;
            Direction = direction;
        }

        #region IComparable<UnitMove> Members

        public int CompareTo(UnitMove other)
        {
            return Comparer<float>.Default.Compare(Priority, other.Priority);
        }

        public bool CheckMovementPossibility()
        {
            return Unit.CanMoveTo(TargetTile.Index);
        }

        /// <summary>
        /// Possibility of the movement is not checked
        /// </summary>
        public void Execute(int turnNum)
        {
            Unit.Move(turnNum, TargetTile.Index);
        }

        #endregion
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
        public InfluenceMapCell this[Point index]
        {
            get
            {
                return mCells[index.X, index.Y];
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

            //base values
            {
                Point index = new Point();
                for (; index.X < mWidth; ++index.X)
                {
                    for (index.Y = 0; index.Y < mHeight; ++index.Y)
                    {
                        terrain.ForEachSibling(index, (t, d) =>
                            {
                                if (HexHelper.GetDirectionToSibling(index, t.Index) != d)
                                {
                                    throw new Exception();
                                }
                            });
                        mCells[index.X, index.Y].InitPropagatedValue(mPlayer, terrain[index], terrain);
                    }
                }
            }

            //propagate
            for (int k = 0; k < 5; ++k)
            {
                Point index = new Point();
                for (; index.X < mWidth; ++index.X)
                {
                    for (index.Y = 0; index.Y < mHeight; ++index.Y)
                    {
                        if (terrain[index].IsPassable)
                        {
                            mCells2[index.X, index.Y].CopyPropagatedValuesFrom(mCells[index.X, index.Y]);
                            terrain.ForEachSibling(index, (tile, dir) =>
                            {
                                if (tile.IsPassable)
                                {
                                    var sibblingIndex = tile.Index;
                                    mCells2[index.X, index.Y].PropagateFrom(mCells[sibblingIndex.X, sibblingIndex.Y]);
                                }
                            });
                        }
                    }
                }

                //swap buffers
                var auxCells = mCells;
                mCells = mCells2;
                mCells2 = auxCells;
            }

            //after propagation
            foreach (var player in mPlayer.Window.Players)
            {
                if (player != mPlayer)
                {
                    foreach (var unit in player.Units)
                    {
                        unit.ForEachAttackPoint((tile, attackType) =>
                            {
                                this[tile.Index].UpdateAsEnemyAttackPoint(unit, tile, attackType);
                            });
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
        public float FirendlyPower { get; private set; }
        public float EnemyPower { get; private set; }
        public float EnemySpearmanThreat { get; private set; }
        public float EnemyCavalryThreat { get; private set; }

        public int AttackRollCnt { get; private set; }
        public bool IsProtectedBySpearman { get; private set; }

        public float HitProbabilityAgainstFootUnit { get; private set; }
        public float HitProbabilityAgainstCavalryUnit { get; private set; }

        public void Reset()
        {
            FirendlyPower = 0.0f;
            EnemyPower = 0.0f;
            AttackRollCnt = 0;
            HitProbabilityAgainstFootUnit = 0.0f;
            HitProbabilityAgainstCavalryUnit = 0.0f;
            IsProtectedBySpearman = false;
            EnemySpearmanThreat = 0.0f;
            EnemyCavalryThreat = 0.0f;
        }

        public void InitPropagatedValue(BattlePlayer player, HexTerrainTile tile, HexTerrain terrain)
        {
            float baseV = 0.3f;
            float siblingBaseV = 0.15f; 
            
            Point index = tile.Index;
            float friendlyPower = 0;
            float enemyPower = 0;
            float enemySpearmanThreat = 0;
            float enemyCavalryThreat = 0;

            terrain.ForEachSibling(tile.Index, (t, dir) =>
            {
                var siblingUnit = t.Unit;
                if (null != siblingUnit)
                {
                    if (player == siblingUnit.Player) //friendly
                    {
                        friendlyPower += siblingBaseV;
                    }
                    else //enemy
                    {
                        if (siblingUnit is Spearman)
                        {
                            enemySpearmanThreat += siblingBaseV;
                        }
                        else if (siblingUnit is Cavalry)
                        {
                            enemyCavalryThreat += siblingBaseV;
                        }
                        enemyPower += siblingBaseV;
                    }
                }
            });

            var unit = terrain[index].Unit;
            if (null != unit)
            {
                if (player == unit.Player) //friendly
                {
                    friendlyPower += baseV;
                }
                else //enemy
                {
                    enemyPower += baseV;
                    if (unit is Spearman)
                    {
                        enemySpearmanThreat += baseV;
                    }
                    else if (unit is Cavalry)
                    {
                        enemyCavalryThreat += baseV;
                    }
                }
            }
            FirendlyPower = friendlyPower;
            EnemyPower = enemyPower;
            EnemySpearmanThreat = enemySpearmanThreat;
            EnemyCavalryThreat = enemyCavalryThreat;
        }

        public void PropagateFrom(InfluenceMapCell influenceMapCell)
        {
            float decay = 0.7f;

            float enemyPower = influenceMapCell.EnemyPower * decay;
            if (EnemyPower < enemyPower)
            {
                EnemyPower = enemyPower;
            }

            float friendlyPower = influenceMapCell.FirendlyPower * decay;
            if (FirendlyPower < friendlyPower)
            {
                FirendlyPower = friendlyPower;
            }

            float enemySpearmanThreat = influenceMapCell.EnemySpearmanThreat * decay;
            if (EnemySpearmanThreat < enemySpearmanThreat)
            {
                EnemySpearmanThreat = enemySpearmanThreat;
            }

            float enemyCavalryThreat = influenceMapCell.EnemyCavalryThreat * decay;
            if (EnemyCavalryThreat < enemyCavalryThreat)
            {
                EnemyCavalryThreat = enemyCavalryThreat;
            }
        }

        internal void CopyPropagatedValuesFrom(InfluenceMapCell influenceMapCell)
        {
            EnemyPower = influenceMapCell.EnemyPower;
            FirendlyPower = influenceMapCell.FirendlyPower;
            EnemySpearmanThreat = influenceMapCell.EnemySpearmanThreat;
            EnemyCavalryThreat = influenceMapCell.EnemyCavalryThreat;
        }

        public void UpdateAsEnemyAttackPoint(BattleUnit unit, HexTerrainTile tile, AttackType attackType)
        {
            if (unit is Spearman)
            {
                IsProtectedBySpearman = true;
            }
            int attackRollCnt = (AttackType.Main == attackType ? 2 : 1);
            float cavalryHitProbabilty = unit.GetDieAgainst(typeof(Cavalry)).HitProbability;
            float footUnitHitProbabilty = unit.GetDieAgainst(typeof(Swordsman)).HitProbability;
            if (0 == AttackRollCnt)
            {
                HitProbabilityAgainstCavalryUnit = cavalryHitProbabilty;
                HitProbabilityAgainstFootUnit = footUnitHitProbabilty;
            }
            else
            {
                HitProbabilityAgainstCavalryUnit = (HitProbabilityAgainstCavalryUnit + cavalryHitProbabilty) - (HitProbabilityAgainstCavalryUnit * cavalryHitProbabilty);
                HitProbabilityAgainstFootUnit = (HitProbabilityAgainstFootUnit + footUnitHitProbabilty) - (HitProbabilityAgainstFootUnit * footUnitHitProbabilty);
            }
            if (AttackType.Main == attackType)
            {
                HitProbabilityAgainstCavalryUnit = (HitProbabilityAgainstCavalryUnit + cavalryHitProbabilty) - (HitProbabilityAgainstCavalryUnit * cavalryHitProbabilty);
                HitProbabilityAgainstFootUnit = (HitProbabilityAgainstFootUnit + footUnitHitProbabilty) - (HitProbabilityAgainstFootUnit * footUnitHitProbabilty);
            }

            AttackRollCnt += attackRollCnt;
        }
    }


    public interface IAiPlayerMapInformationDisplay
    {
        void UpdateMapDisplay(AiBattlePlayer player, HexTerrain terrain);
    }

    public abstract class BaseAiPlayerMapInformationDisplay : IAiPlayerMapInformationDisplay
    {
        private string mName;

        public BaseAiPlayerMapInformationDisplay(string name)
        {
            mName = name;
        }
        public void UpdateMapDisplay(AiBattlePlayer player, HexTerrain terrain)
        {
            Point index = new Point();
            for (; index.X < terrain.Width; ++index.X)
            {
                for (index.Y = 0; index.Y < terrain.Height; ++index.Y)
                {
                    UpdateCell(player, terrain[index]);
                }
            }
        }

        protected abstract void UpdateCell(AiBattlePlayer player, HexTerrainTile tile);

        public override string ToString()
        {
            return mName;
        }
    }

    public abstract class RateMapInformationDisplay : BaseAiPlayerMapInformationDisplay
    {
        public RateMapInformationDisplay(string name)
            : base(name)
        { }

        protected abstract float GetValue(InfluenceMapCell cell);
        
        protected override void UpdateCell(AiBattlePlayer player, HexTerrainTile tile)
        {
            Point index = tile.Index;

            var infCell = player.InfluenceMap[index];
            float value = GetValue(infCell);
            Color c = new Color();
            c.R = (byte)Math.Min(value * 255 * 2, 255);

            if (value > 0.5f)
            {
                c.G = (byte)Math.Min((value - 0.5f) * 2 * 255, 255);
            }
            tile.OverlayBackground = c;
            tile.OverlayText = value.ToString("0.000");
        }
    }
    
    public abstract class BalanceMapInformationDisplay : BaseAiPlayerMapInformationDisplay
    {
        public BalanceMapInformationDisplay(string name)
            : base(name)
        { }

        protected abstract float GetValue1(InfluenceMapCell cell);
        protected abstract float GetValue2(InfluenceMapCell cell);

        protected override void UpdateCell(AiBattlePlayer player, HexTerrainTile tile)
        {
            Point index = tile.Index;

            var infCell = player.InfluenceMap[index];
            float value1 = GetValue1(infCell);
            float value2 = GetValue2(infCell);

            Color c = new Color();
            c.B = (byte)Math.Min(value1 * 255 * 2, 255);
            c.R = (byte)Math.Min(value2 * 255 * 2, 255);

            byte g = 0;
            byte g2 = 0;
            if (value1 > 0.5f)
            {
                g = (byte)Math.Min((value1 - 0.5f) * 2 * 255, 255);
            }
            if (value2 > 0.5f)
            {
                g2 = (byte)Math.Min((value2 - 0.5f) * 2 * 255, 255);
            }
            c.G = Math.Max(g, g2);
            tile.OverlayBackground = c;
            tile.OverlayText = string.Format("{0:0.000}/{1:0.000}", value1, value2);
        }
    }

    public class PowerBalanceMapInformationDisplay : BalanceMapInformationDisplay
    {
        public PowerBalanceMapInformationDisplay()
            : base("Power Balance")
        { }

        protected override float GetValue1(InfluenceMapCell cell)
        {
            return cell.FirendlyPower;
        }

        protected override float GetValue2(InfluenceMapCell cell)
        {
            return cell.EnemyPower;
        }
    }

    public class IsProtectedBySpearmanMapInformationDisplay : BaseAiPlayerMapInformationDisplay
    {
        public IsProtectedBySpearmanMapInformationDisplay()
            : base("Is Protected By Spearman")
        { }

        protected override void UpdateCell(AiBattlePlayer player, HexTerrainTile tile)
        {
            Point index = tile.Index;

            tile.OverlayText = null;
            if (player.InfluenceMap[index].IsProtectedBySpearman)
            {
                tile.OverlayBackground = Color.FromRgb(255, 0, 0);
            }
            else
            {
                tile.OverlayBackground = Color.FromRgb(0, 255, 100);
            }
        }
    }

    public class DamageAgainstCavalryMapInformationDisplay : RateMapInformationDisplay
    {
        public DamageAgainstCavalryMapInformationDisplay()
            : base("Damage against cavalry")
        { }
        
        protected override float GetValue(InfluenceMapCell cell)
        {
            return cell.HitProbabilityAgainstCavalryUnit;
        }
    }

    public class DamageAgainstFootUnitsMapInformationDisplay : RateMapInformationDisplay
    {
        public DamageAgainstFootUnitsMapInformationDisplay()
            : base("Damage against foot units")
        { }

        protected override float GetValue(InfluenceMapCell cell)
        {
            return cell.HitProbabilityAgainstFootUnit;
        }
    }

    public class CavalryThreatMapInformationDisplay : RateMapInformationDisplay
    {
        public CavalryThreatMapInformationDisplay()
            : base("Cavalry threat")
        { }

        protected override float GetValue(InfluenceMapCell cell)
        {
            return cell.EnemyCavalryThreat;
        }
    }

    public class SpearmanThreatMapInformationDisplay : RateMapInformationDisplay
    {
        public SpearmanThreatMapInformationDisplay()
            : base("Spearman threat")
        { }

        protected override float GetValue(InfluenceMapCell cell)
        {
            return cell.EnemySpearmanThreat;
        }
    }
}
