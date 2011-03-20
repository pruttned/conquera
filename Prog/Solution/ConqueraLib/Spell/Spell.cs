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
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    public abstract class Spell
    {
        private bool mIsCasted = false;

        public abstract GraphicElement Picture { get; }
        public abstract GraphicElement Icon { get; }
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract int Cost { get; }

        protected GameUnit Caster { get; private set; }
        protected GameUnit Target { get; private set; }

        public void BeforeAttackCast(GameUnit caster, GameUnit target)
        {
            if (null == caster) throw new ArgumentNullException("caster");
            if (null == target) throw new ArgumentNullException("target");
            if (mIsCasted) throw new InvalidOperationException("Spell is currently casted");

            Caster = caster;
            Target = target;

            mIsCasted = true;

            BeforeAttackCastImpl();
        }

        public bool BeforeAttackUpdate(AleGameTime time)
        {
            CheckIsCasted();
            return BeforeAttackUpdateImpl(time);
        }

        public void AfterAttackHitCast()
        {
            CheckIsCasted();
            AfterAttackHitCastImpl();
        }

        public bool AfterAttackHitUpdate(AleGameTime time)
        {
            CheckIsCasted();

            if (!AfterAttackHitUpdateImpl(time))
            {
                mIsCasted = false;
            }
            return mIsCasted;
        }
        
        public abstract int ApplyAttackModifiers(int baseAttack);

        protected abstract void BeforeAttackCastImpl();
        protected abstract bool BeforeAttackUpdateImpl(AleGameTime time);

        protected abstract void AfterAttackHitCastImpl();
        protected abstract bool AfterAttackHitUpdateImpl(AleGameTime time);

        private void CheckIsCasted()
        {
            if (!mIsCasted) throw new InvalidOperationException("BeforeAttackCast has not yet been called");
        }

    }







}
