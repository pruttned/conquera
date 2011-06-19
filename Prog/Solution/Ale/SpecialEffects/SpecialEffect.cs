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
    public class SpecialEffect : IDisposable
    {
        private bool mIsDisposed = false;

        private Vector3 mPos;
        private SpecialEffectDesc mDesc;
        private List<SpecialEffectObject> mObjects = null;
        private float mEndTime = -1;
        private float mStartTime;
        private bool mPendingDestroy = false;


        public SpecialEffect(SpecialEffectDesc desc, Vector3 pos, IAleServiceProvider services)
        {
            mDesc = desc;
            mPos = pos;

            if (null != desc.Objects)
            {
                mObjects = new List<SpecialEffectObject>();
                foreach (var objDesc in desc.Objects)
                {
                    mObjects.Add(objDesc.CreateObjectInstance(services, pos));
                }
            }
        }

        public bool EnqueRenderableUnits(IRenderer renderer, AleGameTime gameTime, bool firstInFrame)
        {
            //todo marker actions
            //pozor na efekty kde bude duration=0 a nebude mat objekty a vsetky marker animacie sa vykoaju v case = 0

            if (firstInFrame) 
            {
                if (0 > mEndTime)
                {
                    mEndTime = gameTime.TotalTime + mDesc.Duration;
                    mStartTime = gameTime.TotalTime;
                }
                else
                {
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
                            if (!mObjects[i].EnqueRenderableUnits(renderer, gameTime, mStartTime, true))
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
            }
            else //only render without update
            {
                for (int i = mObjects.Count - 1; i >= 0; --i)
                {
                    mObjects[i].EnqueRenderableUnits(renderer, gameTime, mStartTime, false);
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
