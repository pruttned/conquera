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

namespace Conquera
{
    public abstract class Spell
    {
        public event EventHandler TotalCountChanged;
        public event EventHandler AvailableCountChanged;

        private int mTotalCount;
        private int mAvailableCount;

        private bool mIsCasted = false;

        public abstract GraphicElement Picture { get; }
        public abstract GraphicElement Icon { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }

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

    public class SpellCollection : ReadOnlyCollection<Spell>
    {
        public SpellCollection()
            : base(new Spell[10])
        {
            //todo: Fill Items with new Spell instances; bind to events of each spells and raise common events here
        }

        public void SetSpellAvailabilitiesToMax()
        {
            foreach(Spell spell in Items)
            {
                spell.AvailableCount = spell.TotalCount;
            }
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
