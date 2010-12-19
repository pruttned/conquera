using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of mesh parts
    /// </summary>
    public class MeshPartCollection : ReadOnlyCollection<MeshPart>
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="meshParts">- Internal storage</param>
        internal MeshPartCollection(IList<MeshPart> meshParts)
            : base(meshParts)
        {
        }

        #endregion Methods
    }
}
