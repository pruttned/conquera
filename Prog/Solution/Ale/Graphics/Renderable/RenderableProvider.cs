//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using Ale.Content;

namespace Ale.Graphics
{
    public sealed class RenderableProvider : IDisposable
    {
        Dictionary<string, IRenderableFactory> mFactoriesByName = new Dictionary<string, IRenderableFactory>();
        Dictionary<int, IRenderableFactory> mFactoriesById = new Dictionary<int, IRenderableFactory>();
        private bool mIsDisposed = false;

        public ICollection<IRenderableFactory> Factories
        {
            get { return mFactoriesById.Values; }
        }

        /// <summary>
        /// If factory implements IDisposable, then it will be diposed on RenderableProvider.Dispose
        /// </summary>
        /// <param name="factory"></param>
        public void RegisterFactory(IRenderableFactory factory)
        {
            mFactoriesById.Add(factory.Id, factory);
            mFactoriesByName.Add(factory.Name, factory);
        }

        bool IsFactoryRegistered(string factory)
        {
            return mFactoriesByName.ContainsKey(factory);
        }

        bool IsFactoryRegistered(int factory)
        {
            return mFactoriesById.ContainsKey(factory);
        }

        public IRenderableFactory GetFactory(string name)
        {
            IRenderableFactory renderableFactory;
            if (!mFactoriesByName.TryGetValue(name, out renderableFactory))
            {
                throw new KeyNotFoundException(string.Format("Renderable factory with name '{0}' is not registered in RenderableProvider", name));
            }
            return renderableFactory;
        }

        public IRenderableFactory GetFactory(int id)
        {
            IRenderableFactory renderableFactory;
            if (!mFactoriesById.TryGetValue(id, out renderableFactory))
            {
                throw new KeyNotFoundException(string.Format("Renderable factory with id '{0}' is not registered in RenderableProvider", id));
            }
            return renderableFactory;
        }

        public bool TryGetFactory(string name, out IRenderableFactory factory)
        {
            return mFactoriesByName.TryGetValue(name, out factory);
        }

        public bool GetFactory(int id, out IRenderableFactory factory)
        {
            return mFactoriesById.TryGetValue(id, out factory);
        }

        public Renderable CreateRenderable(string factory, string renderable, ContentGroup content)
        {
            return GetFactory(factory).CreateRenderable(renderable, content);
        }
        public Renderable CreateRenderable(string factory, long renderable, ContentGroup content)
        {
            return GetFactory(factory).CreateRenderable(renderable, content);
        }
        public Renderable CreateRenderable(int factory, string renderable, ContentGroup content)
        {
            return GetFactory(factory).CreateRenderable(renderable, content);
        }
        public Renderable CreateRenderable(int factory, long renderable, ContentGroup content)
        {
            return GetFactory(factory).CreateRenderable(renderable, content);
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (IRenderableFactory factory in mFactoriesByName.Values)
                {
                    if (factory is IDisposable)
                    {
                        ((IDisposable)factory).Dispose();
                    }
                }

                mFactoriesByName.Clear();
                mFactoriesById.Clear();

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
