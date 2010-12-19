using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Transactions;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;

namespace SimpleOrmFramework
{
    /// <summary>
    /// MAnages meta infos of data objects
    /// </summary>
    sealed class DataObjectTypeMetaInfoManager : IDisposable
    {
        private DataLayerManager mDataLayerManager;
        private bool mIsDisposed = false;
        private List<DataObjectTypeMetaInfo> mCommitPendingMetaInfos = new List<DataObjectTypeMetaInfo>();
        private Dictionary<Type, DataObjectTypeMetaInfo> mTypeDataObjectMetaInfos = new Dictionary<Type, DataObjectTypeMetaInfo>();
        private Dictionary<int, DataObjectTypeMetaInfo> mTypeIdDataObjectMetaInfos = new Dictionary<int, DataObjectTypeMetaInfo>();

        private Dictionary<string, Type> mTypes = new Dictionary<string, Type>();

        /// <summary>
        /// 
        /// </summary>
        public DataLayerManager DataLayerManager
        {
            get { return mDataLayerManager; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        public DataObjectTypeMetaInfoManager(DataLayerManager dataLayerManager)
        {
            mDataLayerManager = dataLayerManager;
            mDataLayerManager.TransactionCommit += new TransactionFinishHandler(mDataLayerManager_TransactionCommit);
            mDataLayerManager.TransactionRollback += new TransactionFinishHandler(mDataLayerManager_TransactionRollback);

            //find DataObject types
            GetDataObjectTypes(Assembly.GetExecutingAssembly());
            foreach(string file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(file);
                }
                catch(BadImageFormatException){} 
                if (null != assembly)
                {
                    GetDataObjectTypes(Assembly.LoadFile(file));
                }
            }
        }

        private void GetDataObjectTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAssignableFrom(type) && null != AttributeHelper.FindAttribute<DataObjectAttribute>(type, false))
                {
                    mTypes.Add(string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name), type);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataObjectTypeMetaInfo GetDataObjectMetaInfoFromType(Type type)
        {
            DataObjectTypeMetaInfo dataObjectMetaInfo;

            if (!mTypeDataObjectMetaInfos.TryGetValue(type, out dataObjectMetaInfo))
            {
                bool changedDb;
                dataObjectMetaInfo = DataObjectTypeMetaInfo.TryCreateFromType(this, type, out changedDb);
                if (null == dataObjectMetaInfo)
                {
                    throw new ArgumentException(string.Format("Type '{0}' doesn't have requiered attribute '{1}'", type.Name, typeof(DataObjectAttribute).Name));
                }

                if (changedDb && mDataLayerManager.HasActiveTransaction) //pending changes in transaction
                {
                    mCommitPendingMetaInfos.Add(dataObjectMetaInfo);
                }

                mTypeDataObjectMetaInfos.Add(type, dataObjectMetaInfo);
            }

            return dataObjectMetaInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public DataObjectTypeMetaInfo GetDataObjectMetaInfoFromTypeId(int typeId)
        {
            DataObjectTypeMetaInfo dataObjectMetaInfo;

            if (!mTypeIdDataObjectMetaInfos.TryGetValue(typeId, out dataObjectMetaInfo))
            {
                string typeName = (string)mDataLayerManager.ExecuteScalarCommand(
                    "SELECT [TypeName] FROM [Sys.DataObjectType] WHERE [Id]={0}", typeId.ToString());

                if (null == typeName)
                {
                    throw new ArgumentException(string.Format("Type with id '{0}' doesn't exists", typeId));
                }

                Type type = mTypes[typeName];

                //this will add it also to mTypeDataObjectMetaInfos
                dataObjectMetaInfo = GetDataObjectMetaInfoFromType(type);

                mTypeIdDataObjectMetaInfos.Add(typeId, dataObjectMetaInfo);
            }

            return dataObjectMetaInfo;
        }



        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                GC.SuppressFinalize(this);

                mDataLayerManager.Dispose();

                mIsDisposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void mDataLayerManager_TransactionCommit()
        {
            mCommitPendingMetaInfos.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void mDataLayerManager_TransactionRollback()
        {
            foreach (DataObjectTypeMetaInfo metaInfo in mCommitPendingMetaInfos)
            {
                mTypeDataObjectMetaInfos.Remove(metaInfo.DescribedType);
                mTypeIdDataObjectMetaInfos.Remove(metaInfo.DescribedTypeId);
            }
            mCommitPendingMetaInfos.Clear();
        }
    }
}
