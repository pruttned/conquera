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
