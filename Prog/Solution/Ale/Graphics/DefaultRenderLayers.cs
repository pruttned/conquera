using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Graphics
{
    /// <summary>
    /// Default render layers
    /// </summary>
    public static class DefaultRenderLayers
    {
        /// <summary>
        /// Water
        /// </summary>
        public const int Water = 1000;

        /// <summary>
        /// Ground
        /// </summary>
        public const int Ground = 2000;

        /// <summary>
        /// Ground wall
        /// </summary>
        public const int GroundWall = 1500;

        /// <summary>
        /// Objects that are lying on the ground (shadow, road, ...)
        /// </summary>
        public const int GroundLyingObjects = 3000;

        /// <summary>
        /// Objects that are standing on the ground
        /// </summary>
        public const int GroundStandingObjects = 4000;

        /// <summary>
        /// Cursor3dCellSel
        /// </summary>
        public const int Cursor3dCellSel = 5000;

        /// <summary>
        /// Cursor3D
        /// </summary>
        public const int Cursor3D = 6000;

        /// <summary>
        /// MovementArrow
        /// </summary>
        public const int MovementArrow = 7000;
    }
}
