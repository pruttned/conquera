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
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;

namespace Ale.SpecialEffects
{
    public class SpecialEffect : IDisposable
    {
        private bool mIsDisposed = false;

        private Vector3 mPos;
        private SpecialEffectDesc mDesc;
        private ISpecialEffectManager mSpecialEffectManager;
        private List<SpecialEffectObject> mObjects = null;
        private float mEndTime = -1;
        private float mStartTime;
        private bool mPendingDestroy = false;

        private int mFirstTriggerActionIndex = 0;

        private bool HasTimeTriggersToExecute
        {
            get { return null != mDesc.TimeTriggers && mFirstTriggerActionIndex < mDesc.TimeTriggers.Count; }
        }

        public SpecialEffect(SpecialEffectDesc desc, Vector3 pos, IAleServiceProvider services)
        {
            mDesc = desc;
            mPos = pos;
            mSpecialEffectManager = services.GetService<ISpecialEffectManager>();

            if (null != desc.Objects)
            {
                mObjects = new List<SpecialEffectObject>();
                foreach (var objDesc in desc.Objects)
                {
                    mObjects.Add(objDesc.CreateObjectInstance(services, pos));
                }
            }
        }

        public SpecialEffectObject GetObject(NameId name)
        {
            var obj = mObjects.Find(o => name == o.Desc.Name);
            if (null == obj)
            {
                throw new ArgumentException(string.Format("Object '{0}' doesn't exists in special effect (may have been already destroyed)", name));
            }
            return obj;
        }

        public bool EnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime, bool firstInFrame)
        {
            if (firstInFrame)
            {
                if (0 > mEndTime)
                {
                    mEndTime = gameTime.TotalTime + mDesc.Duration;
                    mStartTime = gameTime.TotalTime;
                }
                float timeInAnimation = gameTime.TotalTime - mStartTime;

                if (HasTimeTriggersToExecute)
                {
                    while (mFirstTriggerActionIndex < mDesc.TimeTriggers.Count &&
                        mDesc.TimeTriggers[mFirstTriggerActionIndex].Time <= timeInAnimation)
                    {
                        var actionDesc = mDesc.TimeTriggers[mFirstTriggerActionIndex];
                        var action = mSpecialEffectManager.GetTriggerAction(actionDesc.Action);
                        action.Execute(timeInAnimation, this, actionDesc.Params);
                        mFirstTriggerActionIndex++;
                    }
                }

                if (!mPendingDestroy)
                {
                    if (mEndTime <= gameTime.TotalTime)
                    {
                        if (null != mObjects)
                        {
                            for (int i = mObjects.Count - 1; i >= 0; --i)
                            {
                                if (mObjects[i].Destroy())
                                {
                                    RemoveObject(i);
                                }
                            }
                            if (0 == mObjects.Count)
                            {
                                return false;
                            }

                            mPendingDestroy = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (null != mObjects)
                {
                    for (int i = mObjects.Count - 1; i >= 0; --i)
                    {
                        if (!mObjects[i].EnqueRenderableUnits(renderer, gameTime, timeInAnimation, true))
                        {//ready to be removed
                            RemoveObject(i);
                        }
                    }
                    if (mPendingDestroy && 0 == mObjects.Count) //all object has been removed
                    {
                        return false;
                    }
                }
            }
            else //only render without update
            {
                float timeInAnimation = gameTime.TotalTime - mStartTime;
                if (null != mObjects)
                {
                    for (int i = mObjects.Count - 1; i >= 0; --i)
                    {
                        mObjects[i].EnqueRenderableUnits(renderer, gameTime, timeInAnimation, false);
                    }
                }
            }

            return true;
        }

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
                    if (null != mObjects)
                    {
                        foreach (var obj in mObjects)
                        {
                            obj.Dispose();
                        }
                    }
                }
                mIsDisposed = true;
            }
        }

        private void RemoveObject(int index)
        {
            mObjects[index].Dispose();
            mObjects.RemoveAt(index);
        }
    }
}
