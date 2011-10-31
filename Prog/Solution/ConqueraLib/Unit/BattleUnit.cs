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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Ale.Content;
using Ale.Graphics;
using System.Collections.ObjectModel;
using Ale.Scene;
using Ale;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public class BattleUnit : OctreeSceneObject
    {
        //promoted collections
        private static List<HexTerrainTile> Siblings = new List<HexTerrainTile>(6);

        public delegate void CellIndexChangedHandler(BattleUnit obj, Point oldValue);
        public event CellIndexChangedHandler TileIndexChanged;

        private Point mTileIndex;
        private IGameUnitState mState;
        private Dictionary<string, IGameUnitState> mStates = new Dictionary<string, IGameUnitState>();
        
        public BattlePlayer OwningPlayer { get; internal set; }

        public Point TileIndex
        {
            get { return mTileIndex; }
            set
            {
                if (mTileIndex != value)
                {
                    Point oldValue = mTileIndex;
                    mTileIndex = value;

                    if (IsInScene)
                    {
                        UpdatePositionFromIndex();
                    }

                    OnTileIndexChanged(oldValue);

                    if (null != TileIndexChanged)
                    {
                        TileIndexChanged.Invoke(this, oldValue);
                    }
                }
            }
        }

        public HexTerrainTile Tile
        {
            get
            {
                return BattleScene.Terrain[TileIndex];
            }
        }

        public BattleScene BattleScene
        {
            get { return (BattleScene)Scene; }
        }

        public GameUnitDesc GameUnitDesc
        {
            get { return (GameUnitDesc)Desc; }
        }

        public SkeletalAnimationPlayer AnimationPlayer
        {
            get { return GraphicModel.AnimationPlayer; }
        }

        public bool IsIdle { get; private set; }

        public int Hp { get; private set; }
        
        public int MaxHp 
        {//add modifiers?
            get { return GameUnitDesc.MaxHp; } 
        }

        public bool HasMovedThisTurn
        {
            get { return (LastMovedTurn == BattleScene.TurnNum); }
        }

        public bool HasAttackedThisTurn
        {
            get { return (LastAttackedTurn == BattleScene.TurnNum); }
        }

        public IGameUnitState State
        {
            get { return mState; }
            set
            {
                mState = value;
                mState.OnStart();
                IsIdle = mState.IsIdle;
            }
        }

        public Dictionary<string, IGameUnitState> States
        {
            get { return mStates; }
        }

        private int LastMovedTurn { get; set; }

        private int LastAttackedTurn { get; set; }

        private long DescId { get; set; }

        public BattleUnit(long descId, BattlePlayer owningPlayer, bool isReady)
            :this()
        {
            DescId = descId;
            OwningPlayer = owningPlayer;

            if (isReady)
            {
                LastMovedTurn = -1;
                LastAttackedTurn = -1;
            }
            else
            {
                LastAttackedTurn = LastMovedTurn = owningPlayer.Scene.TurnNum;
            }
        }

        public override void Update(AleGameTime gameTime)
        {
            mState.Update(gameTime);
        }

        struct Seed
        {
            public HexTerrainTile Tile;
            public int Live;

            public Seed(HexTerrainTile tile, int live)
            {
                Tile = tile;
                Live = live;
            }
        }

        private static HashSet<Point> CheckedPoints = new HashSet<Point>();
        private static Queue<Seed> Seeds = new Queue<Seed>();
        
        public List<AdditionalAttackTarget> GetAdditionalAttackTargets(BattleUnit target)
        {
            return GameUnitDesc.GetAdditionalAttackTargets(TileIndex, target.TileIndex);
        }

        public void GetAdditionalAttackTargets(BattleUnit target, List<AdditionalAttackTarget> points)
        {
            GameUnitDesc.GetAdditionalAttackTargets(TileIndex, target.TileIndex, points);
        }

        /// <summary>
        /// Gets all poitions where can unit move
        /// </summary>
        /// <param name="points"></param>
        public void GetPossibleMoves(List<Point> points)
        {
            Seeds.Clear();
            CheckedPoints.Clear();

            Seeds.Enqueue(new Seed(Tile, GameUnitDesc.MovementDistance));
            while (0 < Seeds.Count)
            {
                var seed = Seeds.Dequeue();
                Siblings.Clear();

                BattleScene.Terrain.GetSiblings(seed.Tile.Index, Siblings);
                foreach (var sibling in Siblings)
                {
                    Point index = sibling.Index;
                    if (sibling.IsPassable && !CheckedPoints.Contains(index))
                    {
                        points.Add(index);
                        CheckedPoints.Add(index);
                        if (0 < seed.Live - 1)
                        {
                            Seeds.Enqueue(new Seed(sibling, seed.Live - 1));
                        }
                    }
                }
            }
        }

        public bool CanMoveTo(Point index)
        {
            if (index == TileIndex)
            {
                return false;
            }
            HexTerrainTile srcCell = Tile;
            HexTerrainTile targetCell = BattleScene.Terrain[index];

            if (srcCell == targetCell)
            {
                return false;
            }

            if (targetCell.IsPassable && GameUnitDesc.MovementDistance >= HexHelper.GetDistance(srcCell.Index, targetCell.Index))
            {
                Point targetIndex = targetCell.Index;

                //temp

                Seeds.Clear();
                CheckedPoints.Clear();

                Seeds.Enqueue(new Seed(Tile, GameUnitDesc.MovementDistance));
                while (0 < Seeds.Count)
                {
                    var seed = Seeds.Dequeue();
                    Siblings.Clear();
                    BattleScene.Terrain.GetSiblings(seed.Tile.Index, Siblings);
                    foreach (var sibling in Siblings)
                    {
                        if (sibling.Index == targetIndex)
                        {
                            return true;
                        }

                        Point siblingIndex = sibling.Index;
                        if (sibling.IsPassable && !CheckedPoints.Contains(siblingIndex))
                        {
                            CheckedPoints.Add(siblingIndex);
                            if (0 < seed.Live - 1)
                            {
                                Seeds.Enqueue(new Seed(sibling, seed.Live - 1));
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool CanAttackTo(Point cell)
        {
            return false;
            //todo !!

            //if (cell == CellIndex)
            //{
            //    return false;
            //}

            //HexTerrainTile srcCell = Tile;
            //HexTerrainTile targetCell = BattleScene.GetCell(cell);

            //if (null == targetCell.GameUnit)
            //{
            //    return false;
            //}

            //return null != targetCell.GameUnit && OwningPlayer != targetCell.GameUnit.OwningPlayer &&  srcCell.IsSiblingTo(targetCell);
        }

        public bool MoveTo(Point tileIndex)
        {
            CheckIdle();

            if (tileIndex != TileIndex)
            {
                if (!HasMovedThisTurn && CanMoveTo(tileIndex))
                {
                    LastMovedTurn = BattleScene.TurnNum;

                    var state = (MovingGameUnitState)States["Move"];
                    state.TargetCell = tileIndex;
                    State = state;

                    return true;
                }
            }
            return false;
        }

        public bool Attack(Point tileIndex)
        {
            CheckIdle();

            //todo
            throw new NotImplementedException();
            //if (!HasAttackedThisTurn && CanAttackTo(tileIndex))
            //{
            //    LastAttackedTurn = BattleScene.TurnNum;
            //    LastMovedTurn = BattleScene.TurnNum;

            //    BattleUnit target = BattleScene.Terrain[tileIndex].GameUnit;
            //    RotateTo(target.TileIndex);

            //    var state = (AttackingGameUnitState)States["Attack"];
            //    state.TargetUnit = target;
            //    State = state;
                
            //    return true;
            //}
            return false;
        }


        public void RotateTo(Point cell)
        {
            Vector3 targetPos = GetPositionFromIndex(cell);

            Vector3 movementVect = targetPos - Position;
            movementVect.Normalize();

            float angle = (float)Math.Atan2(movementVect.Y, movementVect.X) + 1.570f;
            Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, angle);
        }

        public int ComputeDamageTo(BattleUnit unit)
        {
            return AleMathUtils.Random.Next(GameUnitDesc.MinAttack, GameUnitDesc.MaxAttack + 1);
        }

        public void Heal(int amount)
        {
            if(amount > 0 && Hp < MaxHp)
            {
                int oldHp = Hp;
                Hp = Math.Min(Hp + amount, MaxHp);
                
                int realHealAmount = Hp-oldHp;

                BattleScene.FireTileNotificationLabel(string.Format("+{0}", realHealAmount), CellNotificationIcons.Hearth, Color.Red, TileIndex);
            }

        }

        public void ReceiveDamage(int damage)
        {
            ReceiveDamage(damage, true);
        }

        public int ReceiveDamage(int damage, bool blood)
        {
            int oldHp = Hp;
            Hp = Math.Max(Hp - damage, 0);
            int realDamage = Hp - oldHp;

            BattleScene.FireTileNotificationLabel(string.Format("-{0}", Math.Abs(realDamage)), CellNotificationIcons.BrokenHearth, Color.Red, TileIndex);
            if (Hp == 0)
            {
                BattleScene.KillUnit(this);
            }
            else
            {
                if (blood)
                {
                    BattleScene.ParticleSystemManager.CreateFireAndForgetParticleSystem(GameUnitDesc.BloodParticleSystem, Position);
                }
            }

            return realDamage;
        }

        /// <summary>
        /// Gets the unti position on a given cell
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetPositionFromIndex(Point index)
        {
            var tile = BattleScene.Terrain[index];
            return tile.CenterPos + tile.Desc.UnitPosition;
        }

        protected BattleUnit()
        {
            IsIdle = true;

            States.Add("Idle", new IdleGameUnitState(this));
            States.Add("Move", new MovingGameUnitState(this));
            States.Add("Attack", new AttackingGameUnitState(this));
        }

        protected override bool IsSceneValid(BaseScene scene)
        {
            return (scene is BattleScene);
        }

        protected override void OnAddToSceneImpl(BaseScene scene)
        {
            base.OnAddToSceneImpl(scene);

            State = States["Idle"];

            UpdatePositionFromIndex();

            foreach(var part in GraphicModel.GraphicModelParts)
            {
                var colorParam = PlayerColorMaterialEffectParametersUpdater.TryGetPlayerColorParam(part.Material.MaterialEffect);
                if (null != colorParam)
                {
                    part.CustomMaterialEffectParametersUpdater = new PlayerColorMaterialEffectParametersUpdater(colorParam, OwningPlayer.Color);
                }
            }

            GraphicModel.AnimationPlayer.Animation = GameUnitDesc.IdleAnimation;
            GraphicModel.AnimationPlayer.Play(true, true);
            GraphicModel.AnimationPlayer.AnimStopped += new SkeletalAnimationPlayer.AnimStoppedHandler(AnimationPlayer_AnimStopped);

            if (((IDataObject)this).Id <= 0) //not loaded
            {
                Hp = GameUnitDesc.MaxHp;
            }
        }

        protected virtual void OnTileIndexChanged(Point oldValue)
        {
        }

        protected override OctreeSceneObjectDesc LoadDesc(ContentGroup content)
        {
            return content.Load<GameUnitDesc>(DescId);
        }    

        private void UpdatePositionFromIndex()
        {
            Position = GetPositionFromIndex(mTileIndex);
        }

        private void CheckIdle()
        {
            if(!IsIdle) throw new InvalidOperationException("Operation can be called only in idle state");
        }

        private void AnimationPlayer_AnimStopped(SkeletalAnimationPlayer animation)
        {
            GraphicModel.AnimationPlayer.Animation = GameUnitDesc.IdleAnimation;
            GraphicModel.AnimationPlayer.Play(true);
        }
    }

    class PlayerColorMaterialEffectParametersUpdater : IMaterialEffectParametersUpdater
    {
        private Vector3MaterialEffectParam mColorParam;
        private Vector3 mColor;

        public PlayerColorMaterialEffectParametersUpdater(Vector3MaterialEffectParam colorParam, Vector3 color)
        {
            mColor = color;
            mColorParam = colorParam;
        }

        public static Vector3MaterialEffectParam TryGetPlayerColorParam(MaterialEffect effect)
        {
            return (Vector3MaterialEffectParam)effect.ManualParameters["gPlayerColor"];
        }

        public void UpdateMaterialEffectParameters()
        {
            mColorParam.Value = mColor;
        }
    }
}
