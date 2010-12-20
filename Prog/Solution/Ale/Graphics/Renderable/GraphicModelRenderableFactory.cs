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
using Ale.Content;

namespace Ale.Graphics
{
    public class GraphicModelRenderableFactory : IRenderableFactory
    {
        private RenderableProvider mRenderableProvider;

        public int Id
        {
            get { return 2; }
        }

        public string Name
        {
            get { return "GraphicModel"; }
        }
        
        public GraphicModelRenderableFactory(RenderableProvider renderableProvider)
        {
            mRenderableProvider = renderableProvider;
        }

        public Renderable CreateRenderable(string name, ContentGroup content)
        {
            GraphicModelDesc desc = content.Load<GraphicModelDesc>(name);
            return new GraphicModel(desc,mRenderableProvider, content);
        }

        public Renderable CreateRenderable(long id, ContentGroup content)
        {
            GraphicModelDesc desc = content.Load<GraphicModelDesc>(id);
            return new GraphicModel(desc, mRenderableProvider, content);
        }
    }
}
