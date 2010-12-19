using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of graphic model parts
    /// </summary>
    public class GraphicModelPartCollection : ReadOnlyCollection<GraphicModelPart>
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="graphicModelParts">- Internal storage</param>
        internal GraphicModelPartCollection(IList<GraphicModelPart> graphicModelParts)
            : base(graphicModelParts)
        {
        }

        #endregion Methods
    }
}
