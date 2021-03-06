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

namespace Ale.Graphics
{
    /// <summary>
    /// Identifies a batched model
    /// </summary>
	public class BatchedModelIdentifier
    {
        private int mIdInBatch;
        private GeometryBatch mBatch;
        private IStaticGeometry mStaticGeometry;
        
		public int IdInBatch 
        {
			get { return mIdInBatch; }
		}
        
		public GeometryBatch Batch 
        {
			get { return mBatch; }
		}

        public IStaticGeometry StaticGeometry
        {
            get { return mStaticGeometry; }
        }

        internal BatchedModelIdentifier(IStaticGeometry staticGeometry, GeometryBatch batch, int idInBatch)
		{
            mStaticGeometry = staticGeometry;
			mIdInBatch = idInBatch;
			mBatch  = batch;
		}
    }
}
