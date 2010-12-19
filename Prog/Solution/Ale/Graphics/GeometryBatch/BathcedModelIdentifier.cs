using System;

namespace Ale.Graphics
{
    /// <summary>
    /// Identifies a batched model
    /// </summary>
	public class BathcedModelIdentifier
    {
        private int mIdInBatch;
        private GeometryBatch mBatch;
        private StaticGeometry mStaticGeometry;
        
		public int IdInBatch 
        {
			get { return mIdInBatch; }
		}
        
		public GeometryBatch Batch 
        {
			get { return mBatch; }
		}

        public StaticGeometry StaticGeometry
        {
            get { return mStaticGeometry; }
        }

        internal BathcedModelIdentifier(StaticGeometry staticGeometry, GeometryBatch batch, int idInBatch)
		{
            mStaticGeometry = staticGeometry;
			mIdInBatch = idInBatch;
			mBatch  = batch;
		}
    }
}
