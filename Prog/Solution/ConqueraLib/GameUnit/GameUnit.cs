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
    [DataObject(MaxCachedCnt = 0)]
    [CustomBasicTypeProvider(typeof(Point), typeof(FieldCustomBasicTypeProvider<Point>))]
    public class GameUnit : OctreeSceneObject, IDataObject
    {
        //promoted collections
        private static List<HexCell> Siblings = new List<HexCell>(6);

        public delegate void CellIndexChangedHandler(GameUnit obj, Point oldValue);
        public event CellIndexChangedHandler CellIndexChanged;

        private Point mCellIndex;
        private IGameUnitState mState;
        private Dictionary<string, IGameUnitState> mStates = new Dictionary<string, IGameUnitState>();
        
        public GamePlayer OwningPlayer { get; internal set; }

        [DataProperty(NotNull = true)]
        public Point CellIndex
        {
            get { return mCellIndex; }
            set
            {
                if (mCellIndex != value)
                {
                    Point oldValue = mCellIndex;
                    mCellIndex = value;

                    if (IsInScene)
                    {
                        UpdatePositionFromIndex();
                    }

                    OnCellIndexChanged(oldValue);

                    if (null != CellIndexChanged)
                    {
                        CellIndexChanged.Invoke(this, oldValue);
                    }
                }
            }
        }

        public HexCell Cell
        {
            get
            {
                return GameScene.GetCell(CellIndex);
            }
        }

        public GameScene GameScene
        {
            get { return (GameScene)Scene; }
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

        [DataProperty(NotNull = true)]
        public int Hp { get; private set; }
        
        public int MaxHp 
        {//add modifiers?
            get { return GameUnitDesc.MaxHp; } 
        }

        public bool HasMovedThisTurn
        {
            get { return (LastMovedTurn == GameScene.GameSceneContextState.TurnNum); }
        }

        public bool HasAttackedThisTurn
        {
            get { return (LastAttackedTurn == GameScene.GameSceneContextState.TurnNum); }
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

        long IDataObject.Id { get; set; }

        [DataListProperty(NotNull = true)] //todo
        private List<long> CardsDb { get; set; }

        [DataProperty(NotNull = true)]
        private long DescId { get; set; }

        [DataProperty(NotNull = true)]
        private int LastMovedTurn { get; set; }

        [DataProperty(NotNull = true)]
        private int LastAttackedTurn { get; set; }

        public GameUnit(long descId, GamePlayer owningPlayer)
            :this()
        {
            DescId = descId;
            OwningPlayer = owningPlayer;
        }

        public override void Update(AleGameTime gameTime)
        {
            mState.Update(gameTime);
        }

        public bool CanMoveTo(Point cell)
        {
            if (cell == CellIndex)
            {
                return false;
            }
            HexCell srcCell = Cell;
            HexCell targetCell = GameScene.GetCell(cell);


            if (targetCell.IsPassable && (srcCell.Region == targetCell.Region || srcCell.IsSiblingTo(targetCell)))
            {
                int distance = HexTerrain.GetDistance(srcCell.Index, targetCell.Index);

                if (1 == distance)
                {
                    return true;
                }
                else
                {
                    if (distance == 2)
                    {
                        Siblings.Clear();
                        srcCell.GetSiblings(Siblings);
                        foreach (var sibling in Siblings)
                        {
                            if (sibling.HexTerrainTile.IsPassable && sibling.Region == targetCell.Region)
                            {
                                if (targetCell.IsSiblingTo(sibling))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool CanAttackTo(Point cell)
        {
            if (cell == CellIndex)
            {
                return false;
            }

            HexCell srcCell = Cell;
            HexCell targetCell = GameScene.GetCell(cell);

            if (null == targetCell.GameUnit)
            {
                return false;
            }

            return null != targetCell.GameUnit && OwningPlayer != targetCell.GameUnit.OwningPlayer &&  srcCell.IsSiblingTo(targetCell);
        }

        public bool MoveTo(Point cell)
        {
            CheckIdle();

            if (cell != CellIndex)
            {
                if (!HasMovedThisTurn && CanMoveTo(cell))
                {
                    LastMovedTurn = GameScene.GameSceneContextState.TurnNum;

                    var state = (MovingGameUnitState)States["Move"];
                    state.TargetCell = cell;
                    State = state;

                    return true;
                }
            }
            return false;
        }

        public bool Attack(Point cell)
        {
            CheckIdle();

            if (!HasAttackedThisTurn && CanAttackTo(cell))
            {
                LastAttackedTurn = GameScene.GameSceneContextState.TurnNum;
                LastMovedTurn = GameScene.GameSceneContextState.TurnNum;

                GameUnit target = GameScene.GetCell(cell).GameUnit;
                RotateTo(target.CellIndex);

                if (null != GameScene.ActiveSpellSlot)
                {
                    var state = (CastingSpellBeforeAttackGameUnitState)States["CastingSpellBeforeAttack"];
                    state.TargetUnit = target;
                    State = state;
                }
                else
                {
                    var state = (AttackingGameUnitState)States["Attack"];
                    state.TargetUnit = target;
                    State = state;
                }
                
                return true;
            }
            return false;
        }


        public void RotateTo(Point cell)
        {
            Vector3 targetPos = GetPositionFromIndex(cell);

            Vector3 movementVect = targetPos - Position;
            movementVect.Normalize();

            float angle = (float)Math.Atan2(movementVect.Y, movementVect.X) - 1.570f;
            Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, angle);
        }

        public int ComputeDamageTo(GameUnit unit, Spell activeSpell)
        {
            if (null != activeSpell)
            {
                return activeSpell.ApplyAttackModifiers(GameUnitDesc.Attack);
            }
            else
            {
                return GameUnitDesc.Attack;
            }
        }

        public void Heal(int amount)
        {
            if(amount > 0 && Hp < MaxHp)
            {
                int oldHp = Hp;
                Hp = Math.Min(Hp + amount, MaxHp);
                
                int realHealAmount = Hp-oldHp;

                GameScene.FireCellNotificationLabel(string.Format("+{0}", realHealAmount), CellNotificationIcons.Hearth, Color.Red, CellIndex);
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

            GameScene.FireCellNotificationLabel(string.Format("-{0}", Math.Abs(realDamage)), CellNotificationIcons.BrokenHearth, Color.Red, CellIndex);
            if (Hp == 0)
            {
                GameScene.KillUnit(this);
            }
            else
            {
                if (blood)
                {
                    GameScene.ParticleSystemManager.CreateFireAndForgetParticleSystem(GameUnitDesc.BloodParticleSystem, Position);
                }
            }

            return realDamage;
        }

        public Vector3 GetPositionFromIndex(Point index)
        {
            HexCell cell = GameScene.GetCell(index);
            if (!cell.IsGap)
            {
                return cell.CenterPos + cell.HexTerrainTile.UnitPosition;
            }
            else
            {
                return cell.CenterPos;
            }
        }

        protected GameUnit()
        {
            IsIdle = true;
            LastMovedTurn = -1;
            LastAttackedTurn = -1;

            States.Add("Idle", new IdleGameUnitState(this));
            States.Add("Move", new MovingGameUnitState(this));
            States.Add("CastingSpellBeforeAttack", new CastingSpellBeforeAttackGameUnitState(this));
            States.Add("Attack", new AttackingGameUnitState(this));
            States.Add("CastingSpellAfterHit", new CastingSpellAfterHitGameUnitState(this));
        }

        protected override bool IsSceneValid(BaseScene scene)
        {
            return (scene is GameScene);
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

        protected virtual void OnCellIndexChanged(Point oldValue)
        {
        }

        protected override OctreeSceneObjectDesc LoadDesc(ContentGroup content)
        {
            return content.Load<GameUnitDesc>(DescId);
        }    

        private void UpdatePositionFromIndex()
        {
            Position = GetPositionFromIndex(mCellIndex);
        }

        void IDataObject.AfterLoad(OrmManager ormManager)
        {
        }

        void IDataObject.BeforeSave(OrmManager ormManager)
        {
        }

        void IDataObject.AfterSave(OrmManager ormManager, bool isNew)
        {
        }

        void IDataObject.BeforeDelete(OrmManager ormManager)
        {
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
