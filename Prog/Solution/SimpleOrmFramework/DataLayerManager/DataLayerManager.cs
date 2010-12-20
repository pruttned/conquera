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
using System.Transactions;
using System.Data.Common;
using System.Data.SQLite;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void TransactionFinishHandler();

    /// <summary>
    /// Provides methods for operating with db
    /// </summary>
    sealed class DataLayerManager : IDisposable
    {
        public event TransactionFinishHandler TransactionCommit;
        public event TransactionFinishHandler TransactionRollback;

        private SQLiteConnection mConnection;
        private bool mIsDisposed = false;
        private bool mHasActiveTransaction = false;

        /// <summary>
        /// 
        /// </summary>
        public bool HasActiveTransaction
        {
            get { return mHasActiveTransaction; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public DataLayerManager(string connectionString)
        {
            mConnection = new SQLiteConnection(connectionString);
            mConnection.Commit += new SQLiteCommitHandler(mConnection_Commit);
            mConnection.RollBack += new EventHandler(mConnection_Rollback);

            mConnection.Open();
        }

        #region ExecuteCommand
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQueryCommand(string commandText)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int ExecuteNonQueryCommand(string commandText, params object[] args)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = string.Format(commandText, args);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQueryCommand(string commandText, params SofDbCommandParameter[] parameters)
        {
            return ExecuteNonQueryCommand(commandText, (IEnumerable<SofDbCommandParameter>)parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQueryCommand(string commandText, IEnumerable<SofDbCommandParameter> parameters)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                FillCommandParamters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalarCommand(string commandText)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object ExecuteScalarCommand(string commandText, params object[] args)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = string.Format(commandText, args);
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalarCommand(string commandText, params SofDbCommandParameter[] parameters)
        {
            return ExecuteScalarCommand(commandText, (IEnumerable<SofDbCommandParameter>)parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalarCommand(string commandText, IEnumerable<SofDbCommandParameter> parameters)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                FillCommandParamters(command, parameters);
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderCommand(string commandText)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderCommand(string commandText, params object[] args)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = string.Format(commandText, args);
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderCommand(string commandText, params SofDbCommandParameter[] parameters)
        {
            return ExecuteReaderCommand(commandText, (IEnumerable<SofDbCommandParameter>)parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReaderCommand(string commandText, IEnumerable<SofDbCommandParameter> parameters)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = commandText;
                FillCommandParamters(command, parameters);
                return command.ExecuteReader();
            }
        }

        #endregion ExecuteCommand

        #region Table

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        public void CrateTable(string tableName, IList<string> columns)
        {
            if (0 == columns.Count)
            {
                throw new ArgumentException("No columns were specified");
            }
            StringBuilder commandBuilder = new StringBuilder(15 + columns.Count * 10);
            commandBuilder.Append("CREATE TABLE [").Append(tableName).Append("] (");

            commandBuilder.Append(columns[0]);
            for (int i = 1; i < columns.Count; ++i)
            {
                commandBuilder.Append(',').Append(columns[i]);
            }
            commandBuilder.Append(')');

            ExecuteNonQueryCommand(commandBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableExists(string tableName)
        {
            using (SQLiteCommand command = mConnection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name";
                command.Parameters.AddWithValue("@name", tableName);

                return (0 != (long)command.ExecuteScalar());
            }
        }
        #endregion Table

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SofTransaction BeginTransaction()
        {
            mHasActiveTransaction = true;
            return new SofTransaction(mConnection.BeginTransaction());
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                GC.SuppressFinalize(this);

                mConnection.Close();
                mConnection.Dispose(); //?

                mIsDisposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mConnection_Commit(object sender, CommitEventArgs e)
        {
            if (null != TransactionCommit)
            {
                TransactionCommit.Invoke();
            }
            mHasActiveTransaction = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mConnection_Rollback(object sender, EventArgs e)
        {
            if (null != TransactionRollback)
            {
                TransactionRollback.Invoke();
            }
            mHasActiveTransaction = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        private void FillCommandParamters(SQLiteCommand command, IEnumerable<SofDbCommandParameter> parameters)
        {
            foreach (SofDbCommandParameter param in parameters)
            {
                command.Parameters.AddWithValue(param.Name, param.Value);
            }
        }
    }
}
