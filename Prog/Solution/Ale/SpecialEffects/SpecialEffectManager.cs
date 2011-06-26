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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;
using Ale.Scene;

namespace Ale.SpecialEffects
{
    public interface ISpecialEffectManager : ISceneDrawableComponent, IDisposable
    {
        void FireSpecialEffect(NameId effect, Vector3 position);
        void FireSpecialEffect(SpecialEffectDesc effect, Vector3 position);
        void RegisterTriggerAction(ISpecialEffectTimeTriggerAction action);
        ISpecialEffectTimeTriggerAction GetTriggerAction(NameId action);
    }

    public sealed class SpecialEffectManager : ISpecialEffectManager
    {
        private bool mIsDisposed = false;

        private ContentGroup mContent;
        private IAleServiceProvider mServices;
        private Dictionary<NameId, SpecialEffectDesc> mDescs = new Dictionary<NameId, SpecialEffectDesc>();
        private Dictionary<NameId, ISpecialEffectTimeTriggerAction> mTriggerActions = new Dictionary<NameId, ISpecialEffectTimeTriggerAction>();
        private long mLastRenderFrame = -1;

        private List<SpecialEffect> mActiveEffects = new List<SpecialEffect>();

        public SpecialEffectManager(ContentGroup content, IAleServiceProvider services)
        {
            mContent = content;
            mServices = services;
        }

        public void FireSpecialEffect(NameId effect, Vector3 position)
        {
            SpecialEffectDesc desc;
            if (!mDescs.TryGetValue(effect, out desc))
            {
                desc = mContent.Load<SpecialEffectDesc>(effect.Name);
                mDescs.Add(effect, desc);
            }
            FireSpecialEffect(desc, position);
        }
        public void FireSpecialEffect(SpecialEffectDesc effect, Vector3 position)
        {
            var specialEffect = new SpecialEffect(effect, position, mServices);

            mActiveEffects.Add(specialEffect);
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (var effect in mActiveEffects)
                {
                    effect.Dispose();
                }

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        public void EnqueRenderableUnits(AleGameTime gameTime, IRenderer renderer, ScenePass scenePass)
        {
            if (0 < mActiveEffects.Count)
            {
                if (mLastRenderFrame != gameTime.FrameNum)
                {
                    mLastRenderFrame = gameTime.FrameNum;

                    for (int i = mActiveEffects.Count - 1; i >= 0; --i)
                    {
                        if (!mActiveEffects[i].EnqueRenderableUnits(renderer, gameTime, true))
                        {
                            mActiveEffects[i].Dispose();
                            mActiveEffects.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    for (int i = mActiveEffects.Count - 1; i >= 0; --i)
                    {
                        mActiveEffects[i].EnqueRenderableUnits(renderer, gameTime, false);
                    }
                }
            }
        }

        public void RegisterTriggerAction(ISpecialEffectTimeTriggerAction action)
        {
            if (null == action) throw new ArgumentNullException("action");

            mTriggerActions[action.Name] = action;
        }

        public ISpecialEffectTimeTriggerAction GetTriggerAction(NameId action)
        {
            return mTriggerActions[action];
        }

    }
}
