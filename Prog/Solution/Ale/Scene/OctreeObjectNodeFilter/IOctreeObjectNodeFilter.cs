using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Scene
{
    /// <summary>
    /// Gets nodes and objects that passes a given check
    /// </summary>
    public interface IOctreeObjectNodeFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeNode"></param>
        /// <returns></returns>
        NodeFilterResult CheckNode(OctreeSceneNode octreeNode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        bool CheckObject(IOctreeObject octreeObject);
    }

    public enum NodeFilterResult
    {
        /// <summary>
        /// Dont include node - no child is checked
        /// </summary>
        DontInclude,

        /// <summary>
        /// Include whole node - each chil is included without check
        /// </summary>
        Include,

        /// <summary>
        /// Include node but check childs
        /// </summary>
        IncludePartially
    }
}
