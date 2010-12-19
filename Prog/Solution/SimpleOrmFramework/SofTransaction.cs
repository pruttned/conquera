using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Transaction
    /// </summary>
    public sealed class SofTransaction : IDisposable
    {
        private SQLiteTransaction mTransaction;
        public bool mIsDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        internal SofTransaction(SQLiteTransaction transaction)
        {
            mTransaction = transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                GC.SuppressFinalize(this);

                mTransaction.Dispose();

                mIsDisposed = true;
            }
        }

        /// <summary>
        /// Commits the transaction
        /// </summary>
        public void Commit()
        {
            mTransaction.Commit();
        }


        /// <summary>
        /// Rollback the transaction
        /// </summary>
        public void Rollback()
        {
            mTransaction.Rollback();
        }
    }
}
