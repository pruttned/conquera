using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ale.Content
{
    /// <summary>
    /// Base interface for loader that loads an asset that is not based on xna content pipeline
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contentGroup"></param>
        /// <param name="Id">if -1 then the asset has no Id</param>
        /// <returns></returns>
        object LoadAsset(string name, ContentGroup contentGroup, out long Id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentGroup"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object LoadAsset(long id, ContentGroup contentGroup, out string name);
    }
}
