using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;

namespace Ale.Graphics
{
    public interface IRenderableFactory
    {
        int Id
        {
            get;
        }

        string Name
        {
            get;
        }

        Renderable CreateRenderable(string name, ContentGroup content);
        Renderable CreateRenderable(long id, ContentGroup content);
    }
}
