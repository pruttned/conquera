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

namespace Ale.Graphics
{
    /// <summary>
    /// Queue that contains renderables ready for rendering grouped by material and ordered by their distance
    /// from a currently active camera
    /// </summary>
    class RenderQueue
    {
        #region Fields

        /// <summary>
        /// Queued opaque renderable units
        /// </summary>
        List<RenderQueueEntry> mOpaqueRenderableUnits = new List<RenderQueueEntry>(30);

        List<RenderQueueEntry> mLightReceivingOpaqueRenderableUnits = new List<RenderQueueEntry>(30);

        /// <summary>
        /// Queued transparent renderable units
        /// </summary>
        List<RenderQueueEntry> mTransparentRenderableUnits = new List<RenderQueueEntry>(30);
 
        #endregion Fields

        public int OpaqueRenderableUnitsCnt { get { return mOpaqueRenderableUnits.Count; } }
        public int LightReceivingOpaqueRenderableUnitsCnt { get { return mLightReceivingOpaqueRenderableUnits.Count; } }
        public int TransparentRenderableUnitsCnt { get { return mTransparentRenderableUnits.Count; } }

        #region Methods

        /// <summary>
        /// Enqueue a renderable for rendering in actual frame
        /// </summary>
        /// <param name="renderableUnit">- Renderable unit that should be enqueued for rendering</param>
        /// <param name="technique">- Technique that should be used while rendering the renderable</param>
        /// <param name="camera">- Currently active camera</param>
        public void Enqueue(IRenderableUnit renderableUnit, MaterialTechnique technique, ICamera camera)
        {
            MaterialPassCollection passes = technique.Passes;
            for (int i = 0; i < passes.Count; ++i)
            {
                var materialEffectPass = passes[i].MaterialEffectPass;
                if (materialEffectPass.IsTransparent) //transparent
                {
                    mTransparentRenderableUnits.Add(new RenderQueueEntry(renderableUnit, passes[i], camera, true));
                }
                else //opaque
                {
                    if (materialEffectPass.ReceivesLight)
                    {
                        mLightReceivingOpaqueRenderableUnits.Add(new RenderQueueEntry(renderableUnit, passes[i], camera, false));
                    }
                    else
                    {
                        mOpaqueRenderableUnits.Add(new RenderQueueEntry(renderableUnit, passes[i], camera, false));
                    }
                }
            }
        }

        /// <summary>
        /// Clears the queue (its capacity is not changed)
        /// </summary>
        public void Clear()
        {
            mOpaqueRenderableUnits.Clear();
            mTransparentRenderableUnits.Clear();
            mLightReceivingOpaqueRenderableUnits.Clear();
        }

        /// <summary>
        /// Executes a given action on all opaque objects.
        /// Objects are grouped by material(first by effect and then by first texture) 
        /// and sorted front to back before executing foreach loop. Grouping is preferred over the sorting.
        /// </summary>
        /// <param name="action">- Action that should be executed</param>
        public void ForEachOpaqueRenderable(Action<IRenderableUnit, MaterialPass> action)
        {
            if (0 < OpaqueRenderableUnitsCnt)
            {
                mOpaqueRenderableUnits.Sort();

                //front to back
                for (int i = 0; i < mOpaqueRenderableUnits.Count; ++i)
                {
                    action(mOpaqueRenderableUnits[i].RenderableUnit, mOpaqueRenderableUnits[i].MaterialPass);
                }
            }
        }

        public void ForEachLightReceivingOpaqueRenderable(Action<IRenderableUnit, MaterialPass> action)
        {
            if (0 < LightReceivingOpaqueRenderableUnitsCnt)
            {
                mLightReceivingOpaqueRenderableUnits.Sort();

                //front to back
                for (int i = 0; i < mLightReceivingOpaqueRenderableUnits.Count; ++i)
                {
                    action(mLightReceivingOpaqueRenderableUnits[i].RenderableUnit, mLightReceivingOpaqueRenderableUnits[i].MaterialPass);
                }
            }
        }

        /// <summary>
        /// Executes a given action on all transparent objects.
        /// Objects are grouped by material(first by effect and then by first texture) and
        /// sorted back to front before executing foreach loop. Sorting is preferred over the grouping.
        /// </summary>
        /// <param name="action">- Action that should be executed</param>
        public void ForEachTransparentRenderable(Action<IRenderableUnit, MaterialPass> action)
        {
            if (0 < TransparentRenderableUnitsCnt)
            {
                mTransparentRenderableUnits.Sort();

                //back to front
                for (int i = mTransparentRenderableUnits.Count - 1; i >= 0; --i)
                {
                    action(mTransparentRenderableUnits[i].RenderableUnit, mTransparentRenderableUnits[i].MaterialPass);
                }
            }
        }

        #endregion Methods
    }
}
