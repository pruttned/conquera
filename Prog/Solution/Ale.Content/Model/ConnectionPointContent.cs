using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Content
{
    public class ConnectionPointContent
    {
        public string Name { get; set; }

        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }

        /// <summary>
        /// If null, then mesh is the parent
        /// </summary>
        public string ParentBoneName { get; set; }
    }
}
