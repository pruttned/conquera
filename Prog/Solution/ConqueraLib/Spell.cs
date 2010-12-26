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
using System.Collections.ObjectModel;
using Ale.Gui;
using Ale;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;

namespace Conquera
{
    public class SpellSlot
    {
        public event EventHandler TotalCountChanged;
        public event EventHandler AvailableCountChanged;

        private int mTotalCount;
        private int mAvailableCount;

        public Spell Spell { get; private set; }

        public int TotalCount
        {
            get { return mTotalCount; }
            set
            {
                if (value != mTotalCount)
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Value must be greater or equal to zero.");
                    }

                    if (value < AvailableCount)
                    {
                        AvailableCount = value;
                    }

                    mTotalCount = value;
                    EventHelper.RaiseEvent(TotalCountChanged, this);
                }
            }
        }

        public int AvailableCount
        {
            get { return mAvailableCount; }
            set
            {
                if (value != mAvailableCount)
                {
                    if (value < 0 || value > TotalCount)
                    {
                        throw new ArgumentException("Value must be greater or equal to zero and lesser or equal than TotalCount.");
                    }

                    mAvailableCount = value;
                    EventHelper.RaiseEvent(AvailableCountChanged, this);
                }
            }
        }

        public SpellSlot(Spell spell)
        {
            Spell = spell;
        }
    }

    public abstract class Spell
    {
        private bool mIsCasted = false;

        public abstract GraphicElement Picture { get; }
        public abstract GraphicElement Icon { get; }
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }

        protected GameUnit CurrentCaster { get; private set; }
        protected GameUnit CurrentTarget { get; private set; }

        public bool Cast(GameUnit caster, GameUnit target)
        {
            if (null == caster) throw new ArgumentNullException("caster");
            if (null == target) throw new ArgumentNullException("target");
            if (mIsCasted) throw new InvalidOperationException("Spell is currently casted");

            CurrentCaster = caster;
            CurrentTarget = target;

            if (CastImpl(caster, target))
            {
                mIsCasted = true;

                return true;
            }
            return false;
        }

        public bool Update(AleGameTime time)
        {
            if (!mIsCasted)
            {
                return false;
            }
            if (!UpdateImpl(time))
            {
                mIsCasted = false;
            }
            return mIsCasted;
        }

        public abstract void ApplyAttackDefenseModifiers(ref int attack, ref int defense);

        protected abstract bool CastImpl(GameUnit caster, GameUnit target);
        protected abstract bool UpdateImpl(AleGameTime time);
    }

    public class SlayerSpell : Spell
    {
        private static int Damage = 5;

        public override GraphicElement Picture
        {
            get { throw new NotImplementedException(); }
        }

        public override GraphicElement Icon
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return "Slayer"; }
        }

        public override string DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void ApplyAttackDefenseModifiers(ref int attack, ref int defense)
        {
            attack += Damage;
        }

        protected override bool CastImpl(GameUnit caster, GameUnit target)
        {
            return false;
        }

        protected override bool UpdateImpl(AleGameTime time)
        {
            return false;
        }
    }

    public class FireStormSpell : Spell
    {
        private static int Damage = 3;
        private static float FireBallPsysSpeed = 2.5f;
        private static Vector3 FireBallPos = new Vector3(0.2f, 0.4f, 4);
        private static readonly string FireBallPsys = "FireBallPsys";
        private static readonly string ExplosionPsys = "FireExplosionPsys";

        List<ParticleSystemMissile> mMissiles = new List<ParticleSystemMissile>();

        public override GraphicElement Picture
        {
            get { throw new NotImplementedException(); }
        }

        public override GraphicElement Icon
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return "FireStorm"; }
        }

        public override string DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void ApplyAttackDefenseModifiers(ref int attack, ref int defense)
        {
        }

        protected override bool CastImpl(GameUnit caster, GameUnit target)
        {
            foreach (var cell in caster.GameScene.GetCell(caster.CellIndex).GetSiblings())
            {
                if (null != cell.GameUnit && cell.GameUnit.OwningPlayer != caster.OwningPlayer)
                {
                    var missile2 = new ParticleSystemMissile(cell.GameUnit, cell.GameUnit.Position + FireBallPos, cell.GameUnit.Position, FireBallPsys, FireBallPsysSpeed);
                    missile2.OnHit += new ParticleSystemMissile.OnHitHandler(missile_OnHit);
                    mMissiles.Add(missile2);
                }
            }

            return true;
        }

        protected override bool UpdateImpl(AleGameTime time)
        {
            if (0 == mMissiles.Count)
            {
                return false;
            }

            for (int i = mMissiles.Count - 1; i >= 0; --i)
            {
                if (!mMissiles[i].Update(time))
                {
                    mMissiles[i].Dispose();
                    mMissiles.RemoveAt(i);
                }
            }

            return true;
        }

        private void missile_OnHit(ParticleSystemMissile missile, GameUnit target)
        {
            target.ReceiveDamage(Damage, false);
            target.GameScene.ParticleSystemManager.CreateFireAndforgetParticleSystem(
                target.GameScene.Content.Load<ParticleSystemDesc>(ExplosionPsys), target.Position);
            target.GameScene.GameCamera.Shake();
        }
    }

    public class ParticleSystemMissile : IDisposable
    {
        public delegate void OnHitHandler(ParticleSystemMissile missile, GameUnit target);

        public event OnHitHandler OnHit;

        private bool mIsDisposed = false;
        private ParticleSystem mParticleSystem;
        private Vector3LinearAnimator mPosAnimator = new Vector3LinearAnimator();

        public GameUnit Target { get; private set; }
        public GameScene GameScene { get { return Target.GameScene; } }

        public ParticleSystemMissile(GameUnit target, Vector3 srcPos, Vector3 destPos, string pSysName, float speed)
        {
            Target = target;
            mParticleSystem = GameScene.ParticleSystemManager.CreateParticleSystem(GameScene.Content, pSysName);
            mPosAnimator.Animate(speed, srcPos, destPos);
            mParticleSystem.Position = mPosAnimator.CurrentValue;

            GameScene.Octree.AddObject(mParticleSystem);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    mParticleSystem.Dispose();
                }
                mIsDisposed = true;
            }
        }

        public bool Update(AleGameTime time)
        {
            if (mPosAnimator.Update(time))
            {
                mParticleSystem.Position = mPosAnimator.CurrentValue;
            }
            else
            {
                if (mParticleSystem.IsEnabled)
                {
                    mParticleSystem.IsEnabled = false;
                    if (null != OnHit)
                    {
                        OnHit.Invoke(this, Target);
                    }
                }
                else
                {
                    if (!mParticleSystem.IsLoaded)
                    {
                        GameScene.Octree.RemoveObject(mParticleSystem);
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class SpellCollection : ReadOnlyCollection<SpellSlot>
    {
        public static Spell[] Spells;

        public SpellSlot this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
                foreach (var spell in this)
                {
                    if (string.Equals(name, spell.Spell.Name))
                    {
                        return spell;
                    }
                }
                throw new KeyNotFoundException(string.Format("Spell with name '{0}' doesn't exists", name));
            }
        }

        static SpellCollection()
        {
            Spells = new Spell[] 
            { 
                new SlayerSpell(),
                new FireStormSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell(),
                new SlayerSpell()
            };
        }

        public SpellCollection()
            : base(CreateSpellList())
        {
        }

        public void ResetSpellAvailabilities()
        {
            foreach(var spell in Items)
            {
                spell.AvailableCount = spell.TotalCount;
            }
        }

        private static SpellSlot[] CreateSpellList()
        {
            SpellSlot[] slots =  new SpellSlot[Spells.Length];
            for (int i = 0; i < slots.Length; ++i)
            {
                slots[i] = new SpellSlot(Spells[i]);
            }

            return slots;
        }
    }

    public static class EventHelper
    {
        public static void RaiseEvent(EventHandler handler, object sender)
        {
            RaiseEvent(handler, sender, EventArgs.Empty);
        }

        public static void RaiseEvent(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
