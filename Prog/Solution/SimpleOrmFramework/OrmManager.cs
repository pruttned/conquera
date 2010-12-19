//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using System.Transactions;
using System.Collections;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using System.Data.Common;
using System.IO;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Main Sof class that provides functions for loading, saving and deleting data objects.
    /// </summary>
    public sealed class OrmManager : IDisposable
    {
        Dictionary<long, IDataObject> mCachedObjects = new Dictionary<long, IDataObject>();
        Dictionary<Type, LinkedList<long>> mCachedObjectsByType = new Dictionary<Type, LinkedList<long>>();

        DataObjectTypeMetaInfoManager mDataObjectTypeMetaInfoManager;
        List<long> mCommitPendingObjects = new List<long>();
        private bool mIsDisposed = false;

        /// <summary>
        /// OrmManager version.
        /// </summary>
        public int Version
        {
            get { return 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DataLayerManager DataLayerManager
        {
            get { return DataObjectTypeMetaInfoManager.DataLayerManager; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DataObjectTypeMetaInfoManager DataObjectTypeMetaInfoManager
        {
            get { return mDataObjectTypeMetaInfoManager; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public OrmManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

            mDataObjectTypeMetaInfoManager = new DataObjectTypeMetaInfoManager(new DataLayerManager(connectionString));

            if (DataLayerManager.TableExists("Sys.Version"))
            {
                int version = (int)DataLayerManager.ExecuteScalarCommand("SELECT [Version] FROM [Sys.Version] LIMIT 1");
                //no updates are implemented now so just throw
                if (version != Version)
                {
                    throw new SofException(string.Format("Db has a incompatible version '{0}'. Framework version is '{1}'", version, Version));
                }
            }

            //init sys tables
            using (SofTransaction transaction = DataLayerManager.BeginTransaction())
            {
                if (!DataLayerManager.TableExists("Sys.DataObjectType"))
                {
                    DataLayerManager.CrateTable("Sys.DataObjectType", new string[] {
                            "[Id] INTEGER PRIMARY KEY",
                            "[TypeName] TEXT NOT NULL UNIQUE"});
                }

                if (!DataLayerManager.TableExists("Sys.DataObject"))
                {
                    DataLayerManager.CrateTable("Sys.DataObject", new string[] {
                            "[Id] INTEGER PRIMARY KEY",
                            "[DataType] INTEGER NOT NULL"});
                }

                if (!DataLayerManager.TableExists("Sys.Version"))
                {
                    DataLayerManager.CrateTable("Sys.Version", new string[] {
                            "[Version] INT NOT NULL"});
                    DataLayerManager.ExecuteNonQueryCommand("INSERT INTO [Sys.Version] VALUES({0})", Version.ToString());
                }


                if (!DataLayerManager.TableExists("Sys.Blob"))
                {
                    DataLayerManager.CrateTable("Sys.Blob", new string[] {
                            "[Name] TEXT NOT NULL UNIQUE", 
                            "[Data] BLOB"});
                }

                transaction.Commit();
            }
        }

        public static string CreateDefaultConnectionString(string dbFile)
        {
            return string.Format("Data Source={0};Version=3;New=False;Compress=False", dbFile);
        }

        /// <summary>
        /// Begins the transaction
        /// </summary>
        /// <returns></returns>
        public SofTransaction BeginTransaction()
        {
            return DataLayerManager.BeginTransaction();
        }

        /// <summary>
        /// Saves (creates or updates) the data object to the db.
        /// Id of the object will be filed with a real value.
        /// You should call RepairObjectId in case of exception which will return all invalid ids (result of rolbacked transaction) to -1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public long SaveObject(IDataObject obj)
        {
            if (null == obj) throw new ArgumentNullException("obj");

            long objectId;

            using (SofTransaction t = DataLayerManager.BeginTransaction())
            {
                Type type = obj.GetType();
                try
                {
                    DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(type);

                    //new object
                    if (0 >= obj.Id)
                    {
                        obj.BeforeSave(this);

                        //insert to Sys.DataObject and generate Id
                        obj.Id = (long)DataLayerManager.ExecuteScalarCommand("INSERT INTO [Sys.DataObject] ([DataType]) VALUES ('{0}'); SELECT last_insert_rowid()", dataObjectMetaInfo.DescribedTypeId);

                        SaveObjectInter(obj, dataObjectMetaInfo);

                        obj.AfterSave(this, true);
                    }
                    else //update
                    {
                        objectId = obj.Id;

                        obj.BeforeSave(this);

                        UpdateObjectInter(obj, LoadObjectLight(obj.Id), dataObjectMetaInfo);

                        obj.AfterSave(this, false);
                    }

                    RemoveObjectFromCahce(obj.Id);

                }
                catch (Exception ex)
                {
                    ex = new SofException(string.Format("Error occured during saving of the object Id={0} (type='{1}'). See inner exception for details", obj.Id.ToString(), type.FullName), ex);
                    ex.Data["Id"] = obj.Id;
                    ex.Data["Type"] = type;

                    throw ex;
                }

                t.Commit();
            }

            return obj.Id;
        }

        /// <summary>
        /// Loads the data object from db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDataObject LoadObject(long id)
        {
            try
            {
                Dictionary<long, IDataObject> loadCache = new Dictionary<long, IDataObject>();

                IDataObject newDataObject = LoadObjectInter(id, loadCache, true, true);

                foreach (IDataObject dataObject in loadCache.Values)
                {
                    dataObject.AfterLoad(this);
                }

                return newDataObject;
            }
            catch (Exception ex)
            {
                ex = new SofException(string.Format("Exception occured during load of the object with Id={0}. See inner exception for details.", id), ex);
                ex.Data["Id"] = id;
                throw ex;
            }
        }

        /// <summary>
        /// Checks whether exists object with a specified id in db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsObjectWithId(long id)
        {
            if (mCachedObjects.ContainsKey(id))
            {
                return true;
            }

            return 0 != (long)DataLayerManager.ExecuteScalarCommand("SELECT COUNT(*) FROM [Sys.DataObject] WHERE [Id]={0} LIMIT 1", id.ToString());
        }

        /// <summary>
        /// Gets the type of object with a specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Type GetTypeOfObject(long id)
        {
            //try cache
            IDataObject dataObject;
            if (mCachedObjects.TryGetValue(id, out dataObject))
            {
                return dataObject.GetType();
            }

            string typeName = (string)DataLayerManager.ExecuteScalarCommand(
                    "SELECT [TypeName] FROM [Sys.DataObjectType] WHERE [Id]=(SELECT [DataType] FROM  [Sys.DataObject] WHERE [Id]={0} LIMIT 1)", id.ToString());

            if (null == typeName)
            {
                throw new SofException(string.Format("Object's (Id={0}) type could not be determined. Check whether is object's id valid", id));
            }

            return Type.GetType(typeName, true);
        }

        /// <summary>
        /// Repairs object's Id and Ids of all his strong referenced childs.
        /// Repairing of an Id means, the it will be set to -1 if it is invalid.
        /// This state can occure after rollbacking the transaction in which was object saved or its saving has been started.
        /// </summary>
        /// <param name="dataObject"></param>
        public void RepairObjectId(IDataObject dataObject)
        {
            if (null == dataObject) throw new ArgumentNullException("dataObject");
            if (0 < dataObject.Id && !ExistsObjectWithId(dataObject.Id))
            {
                dataObject.Id = -1;
            } 
            
            RepairObjectPropertiesId(dataObject, DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(dataObject.GetType()));
        }
        
        /// <summary>
        /// Clears object's Id and Ids of all his strong referenced childs.
        /// Useful for SaveAs functionality
        /// </summary>
        public void ClearObjectId(IDataObject dataObject)
        {
            if (null == dataObject) throw new ArgumentNullException("dataObject");
            
            dataObject.Id = -1;

            ClearObjectPropertiesId(dataObject, DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(dataObject.GetType()));
        }

        /// <summary>
        /// Loads the data object without initializing IDataObject properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal IDataObject LoadObjectLight(long id)
        {
            Dictionary<long, IDataObject> loadCache = new Dictionary<long, IDataObject>();

            try
            {
                return LoadObjectInter(id, loadCache, false, true);
            }
            catch (Exception ex)
            {
                ex = new SofException(string.Format("Exception occured during load of the object with Id={0}. See inner exception for details.", id), ex);
                ex.Data["Id"] = id;
                throw ex;
            }
        }

        /// <summary>
        /// Loads the data object from db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T LoadObject<T>(long id) where T : class, IDataObject
        {
            return (T)LoadObject(id);
        }

        /// <summary>
        /// Deletes data object in the db
        /// </summary>
        /// <param name="id"></param>
        public void DeleteObject(long id)
        {
            IDataObject dataObject = LoadObjectLight(id); //dont call AfterLoad

            using (SofTransaction t = DataLayerManager.BeginTransaction())
            {
                try
                {
                    dataObject.BeforeDelete(this);

                    DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(dataObject.GetType());
                    DataLayerManager.ExecuteNonQueryCommand(string.Format("DELETE FROM [Sys.DataObject] WHERE [Id] = {0}", dataObject.Id.ToString()));

                    DeleteObjectInter(dataObject, dataObjectMetaInfo);

                    RemoveObjectFromCahce(dataObject.Id);
                }
                catch (Exception ex)
                {
                    ex = new SofException(string.Format("Exception occured during deletition of the object with Id={0} (Type = '{1}'). See inner exception for details.", dataObject.Id, dataObject.GetType().FullName), ex);
                    ex.Data["Id"] = id;
                    throw ex;
                }

                t.Commit();
            }
        }

        /// <summary>
        /// Loads data objects that matches a given query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sofQuery"></param>
        /// <returns></returns>
        public T[] LoadObjects<T>(string sofQuery) where T : class, IDataObject
        {
            if (string.IsNullOrEmpty(sofQuery)) throw new ArgumentNullException("sofQuery");

            long[] ids = FindObjects(typeof(T), sofQuery);
            T[] objs = new T[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
            {
                objs[i] = LoadObject<T>(ids[i]);
            }

            return objs;
        }

        /// <summary>
        /// Loads data objects that matches a given query
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sofQuery"></param>
        /// <returns></returns>
        public object[] LoadObjects(Type type, string sofQuery)
        {
            if (string.IsNullOrEmpty(sofQuery)) throw new ArgumentNullException("sofQuery");
            if (null == type) throw new ArgumentNullException("type");

            long[] ids = FindObjects(type, sofQuery);
            object[] objs = new object[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
            {
                objs[i] = LoadObject(ids[i]);
            }

            return objs;
        }

        /// <summary>
        /// Gets the ids of objects that matches a given query
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sofQuery"></param>
        /// <returns></returns>
        public long[] FindObjects(Type type, string sofQuery)
        {
            if (string.IsNullOrEmpty(sofQuery)) throw new ArgumentNullException("sofQuery");
            if (null == type) throw new ArgumentNullException("type");
            
            try
            {
                DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(type);

                ANTLRStringStream stream = new ANTLRStringStream(sofQuery);
                SofQueryLexer lexer = new SofQueryLexer(stream);
                CommonTokenStream tStream = new CommonTokenStream(lexer);
                SofQueryParser parser = new SofQueryParser(tStream);
                SofQueryParser.start_return ret = parser.start();
                CommonTree tree = (CommonTree)ret.Tree;

                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append("SELECT [").Append(dataObjectMetaInfo.TableName).Append("].[Id] FROM ");
                CreateHierarchyTableJoin(dataObjectMetaInfo, queryBuilder);
                queryBuilder.Append(" WHERE ( ");

                EvaluateQuery(tree, queryBuilder, dataObjectMetaInfo);

                queryBuilder.Append(')');

                List<long> ids = new List<long>();
                using (DbDataReader reader = DataLayerManager.ExecuteReaderCommand(queryBuilder.ToString()))
                {
                    while (reader.Read())
                    {
                        ids.Add((long)reader[0]);
                    }
                }

                return ids.ToArray();
            }
            catch (Exception ex)
            {
                ex = new SofException(string.Format("Exception occured during searching for objects of type '{0}'. See inner exception for details.", type.FullName), ex);
                ex.Data["Type"] = type;
                throw ex;
            }
        }

        /// <summary>
        /// Gets the id of the first objects that matches a given query
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sofQuery"></param>
        /// <returns>id or -1</returns>
        public long FindObject(Type type, string sofQuery)
        {
            if (string.IsNullOrEmpty(sofQuery)) throw new ArgumentNullException("sofQuery");
            if (null == type) throw new ArgumentNullException("type");

            try
            {
                DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(type);

                ANTLRStringStream stream = new ANTLRStringStream(sofQuery);
                SofQueryLexer lexer = new SofQueryLexer(stream);
                CommonTokenStream tStream = new CommonTokenStream(lexer);
                SofQueryParser parser = new SofQueryParser(tStream);
                SofQueryParser.start_return ret = parser.start();
                CommonTree tree = (CommonTree)ret.Tree;

                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append("SELECT [").Append(dataObjectMetaInfo.TableName).Append("].[Id] FROM ");
                CreateHierarchyTableJoin(dataObjectMetaInfo, queryBuilder);
                queryBuilder.Append(" WHERE ( ");

                EvaluateQuery(tree, queryBuilder, dataObjectMetaInfo);

                queryBuilder.Append(" )  LIMIT 1");

                object idObj = DataLayerManager.ExecuteScalarCommand(queryBuilder.ToString());
                if (null == idObj)
                {
                    return -1;
                }
                return (long)idObj;
            }
            catch (Exception ex)
            {
                ex = new SofException(string.Format("Exception occured during searching for objects of type '{0}'. See inner exception for details.", type.FullName), ex);
                ex.Data["Type"] = type;
                throw ex;
            }
        }


        /// <summary>
        /// Clears the entire cache
        /// </summary>
        public void ClearCache()
        {
            mCachedObjects.Clear();
            mCachedObjectsByType.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                GC.SuppressFinalize(this);

                mDataObjectTypeMetaInfoManager.Dispose();

                mIsDisposed = true;
            }
        }

        /// <summary>
        /// Removes a specified object from cache (also its stroong references)
        /// </summary>
        /// <param name="id"></param>
        internal void RemoveObjectFromCahce(long id)
        {
            IDataObject dataObject;
            if (mCachedObjects.TryGetValue(id, out dataObject))
            {
                mCachedObjects.Remove(id);
                DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(dataObject.GetType());
                foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
                {
                    property.RemoveFromCache(this, dataObject);
                }

                //it must be in type object cahce
                LinkedList<long> typeCachedObjects;
                if (mCachedObjectsByType.TryGetValue(dataObject.GetType(), out typeCachedObjects))
                {
                    typeCachedObjects.Remove(id);
                }
            }
        }

        #region FindId

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="queryBuilder"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void EvaluateQuery(CommonTree tree, StringBuilder queryBuilder, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            if (SofQueryParser.EXPR == tree.Type)
            {
                IList idChildrens = ((CommonTree)tree.Children[0]).Children;
                string[] idParts = new string[idChildrens.Count];
                for (int i = 0; i < idParts.Length; ++i)
                {
                    idParts[i] = ((CommonTree)idChildrens[i]).Text;
                }

                string oper = ((CommonTree)tree.Children[1]).Text;
                string value = ((CommonTree)tree.Children[2]).Text;

                EvaluateExpr(idParts, 0, oper, value, queryBuilder, dataObjectMetaInfo);
            }
            else
            {
                queryBuilder.Append(tree.Text).Append(' ');

                if (null != tree.Children)
                {
                    foreach (CommonTree child in tree.Children)
                    {
                        EvaluateQuery(child, queryBuilder, dataObjectMetaInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idParts"></param>
        /// <param name="idPartIndex"></param>
        /// <param name="oper"></param>
        /// <param name="value"></param>
        /// <param name="queryBuilder"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void EvaluateExpr(string[] idParts, int idPartIndex, string oper, string value, StringBuilder queryBuilder, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            IPropertyMetaInfo iproperty = dataObjectMetaInfo.FindProperty(idParts[idPartIndex]);
            if (null == iproperty)
            {
                throw new SofQueryParsingException(string.Format("Property '{0}' doesn't exists in type '{1}' or its base types", idParts[idPartIndex], dataObjectMetaInfo.ToString()));
            }
            if (!(iproperty is PropertyMetaInfo))
            {
                throw new SofQueryParsingException(string.Format("Property '{0}' can't be used in query because its type is not supported", idPartIndex));
            }

            PropertyMetaInfo property = (PropertyMetaInfo)iproperty;

            if (idPartIndex == idParts.Length - 1) //simple property
            {
                queryBuilder.Append('[').Append(property.ColumnName).Append(']').Append(oper).Append(value).Append(' ');
            }
            else //complex property
            {
                DataObjectTypeMetaInfo propertyTypeMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(property.PropertyType);
                queryBuilder.Append('[').Append(property.ColumnName).Append("] IN (SELECT [").Append(propertyTypeMetaInfo.TableName).Append("].[Id] FROM [").Append(propertyTypeMetaInfo.TableName).Append("] WHERE (");

                EvaluateExpr(idParts, idPartIndex + 1, oper, value, queryBuilder, propertyTypeMetaInfo);

                queryBuilder.Append("))");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="queryBuilder"></param>
        private void CreateHierarchyTableJoin(DataObjectTypeMetaInfo type, StringBuilder queryBuilder)
        {
            string tableNameWithBrackets = string.Format("[{0}]", type.TableName);
            queryBuilder.Append(tableNameWithBrackets).Append(' ');

            DataObjectTypeMetaInfo baseType = type.BaseType;
            while (null != baseType) //join
            {
                string baseTableNameWithBrackets = string.Format("[{0}]", baseType.TableName);
                queryBuilder.Append("JOIN ").Append(baseTableNameWithBrackets).Append(" ON ").Append(tableNameWithBrackets).Append(".[Id] = ").Append(baseTableNameWithBrackets).Append(".[Id] ");

                baseType = baseType.BaseType;
            }

        }

        #endregion FindId

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        /// <param name="initialize"></param>
        /// <returns></returns>
        internal IDataObject LoadObjectInter(long id, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects, bool initialize)
        {
            IDataObject newObject;
            if (loadCache.TryGetValue(id, out newObject)) //get from load cahce
            {
                return newObject;
            }

            //Get a real object's type and meta info
            Type objectType = GetTypeOfObject(id);
            DataObjectTypeMetaInfo dataObjectMetaInfo = DataObjectTypeMetaInfoManager.GetDataObjectMetaInfoFromType(objectType);

            //create uninitialized object
            newObject = (IDataObject)dataObjectMetaInfo.DefaultCtor.Invoke(null);
            newObject.Id = id;
            //insert uninitialized object to laod cache
            //object is inserted to the loadCahce after it is created but before it is initialized to handle circular references
            loadCache.Add(id, newObject);

            if (initialize)
            {
                //Initialize object
                //from main cache
                IDataObject objFromMainCache;
                if (mCachedObjects.TryGetValue(id, out objFromMainCache))
                {
                    InitializeObject(newObject, objFromMainCache, dataObjectMetaInfo, loadCache, initializeReferencedObjects);
                }
                else
                { //from db
                    InitializeObject(newObject, dataObjectMetaInfo, loadCache, initializeReferencedObjects);

                    //create clone for main cache
                    IDataObject cachedObject = (IDataObject)dataObjectMetaInfo.DefaultCtor.Invoke(null);
                    cachedObject.Id = id;
                    InitializeObject(cachedObject, newObject, dataObjectMetaInfo, new Dictionary<long, IDataObject>(), false); //dont initialize referenced objects 

                    //cache
                    if (0 != dataObjectMetaInfo.MaxCachedCnt)
                    {
                        if (dataObjectMetaInfo.MaxCachedCnt > 0) //has cache limit
                        {
                            //check if cache for this type is not full
                            LinkedList<long> typeCachedObjects;
                            if (mCachedObjectsByType.TryGetValue(objectType, out typeCachedObjects))
                            {
                                if (typeCachedObjects.Count >= dataObjectMetaInfo.MaxCachedCnt)
                                {//remove oldest object
                                    RemoveObjectFromCahce(typeCachedObjects.First.Value); //olderst is first so typeCachedObjects.Remove in typeCachedObjects will find it quickly
                                }
                            }
                            else
                            {//store object in mCachedObjectsByType only if its type has a cache limit
                                typeCachedObjects = new LinkedList<long>();
                                mCachedObjectsByType.Add(objectType, typeCachedObjects);
                            }
                            typeCachedObjects.AddLast(id);
                        }

                        mCachedObjects.Add(id, cachedObject);
                    }
                }
            }
            return newObject;
        }

        /// <summary>
        /// Updates a specified blob data in db.
        /// </summary>
        /// <param name="name">- Unique name of the blob record</param>
        /// <param name="data"></param>
        /// <returns>True if a new record was created. False if a existing record was updated</returns>
        public bool SetBlobData(string name, byte[] data)
        {
            SofDbCommandParameter nameParam = new SofDbCommandParameter("@Name", name, "Name");
            SofDbCommandParameter dataParam = new SofDbCommandParameter("@Data", data, "Data");
            long cnt = (long)DataLayerManager.ExecuteScalarCommand("SELECT COUNT(*) FROM [Sys.Blob] WHERE [Name]=@Name", nameParam);
            if (0 == cnt)//new
            {
                DataLayerManager.ExecuteNonQueryCommand("INSERT INTO [Sys.Blob] Values (@Name, @Data)", nameParam, dataParam);
                return true;
            }
            else //update
            {
                DataLayerManager.ExecuteNonQueryCommand("UPDATE [Sys.Blob] SET [Data] = @Data WHERE [Name]=@Name", nameParam, dataParam);
                return false;
            }
        }

        /// <summary>
        /// Gets the blob data by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Data or null</returns>
        public byte[] GetBlobData(string name)
        {
            SofDbCommandParameter nameParam = new SofDbCommandParameter("@Name", name, "Name");
            return (byte[])DataLayerManager.ExecuteScalarCommand("SELECT (Data) FROM [Sys.Blob] WHERE [Name]=@Name LIMIT 1", nameParam);
        }

        /// <summary>
        /// Deletes a given blob data record
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if record was deleted. False if it doesn't exists</returns>
        public bool DeleteBlobData(string name)
        {
            SofDbCommandParameter nameParam = new SofDbCommandParameter("@Name", name, "Name");
            return (0 < DataLayerManager.ExecuteNonQueryCommand("DELETE FROM [Sys.Blob] WHERE [Name]=@Name", nameParam));
        }

        /// <summary>
        /// Gets whether exists a blob data record with a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool BlobDataExists(string name)
        {
            SofDbCommandParameter nameParam = new SofDbCommandParameter("@Name", name, "Name");
            long cnt = (long)DataLayerManager.ExecuteScalarCommand("SELECT COUNT(*) FROM [Sys.Blob] WHERE [Name]=@Name", nameParam);
            return (0 == cnt);
        }

        /// <summary>
        /// Initializes a new object using values gathered from db
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="dataObjectMetaInfo"></param>
        /// <param name="loadCache"></param>
        private void InitializeObject(IDataObject newObject, DataObjectTypeMetaInfo dataObjectMetaInfo, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects)
        {
            long id = newObject.Id;

            using (DbDataReader dbReader = DataLayerManager.ExecuteReaderCommand("SELECT * FROM [{0}] WHERE [Id]={1}", dataObjectMetaInfo.TableName, id.ToString()))
            {
                if (!dbReader.Read())
                {
                    throw new ArgumentException(string.Format("Object with id '{0}' doesn't exists", id));
                }

                foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
                {
                    property.InitializeProperty(newObject, dbReader, this, loadCache, initializeReferencedObjects);
                }
            }

            //load properties of the base type
            if (null != dataObjectMetaInfo.BaseType)
            {
                InitializeObject(newObject, dataObjectMetaInfo.BaseType, loadCache, initializeReferencedObjects);
            }
        }

        /// <summary>
        /// Initializes a new object using values gathered from src object
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="dataObjectMetaInfo"></param>
        /// <param name="loadCache"></param>
        private void InitializeObject(IDataObject newObject, IDataObject srcObject, DataObjectTypeMetaInfo dataObjectMetaInfo, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects)
        {
            foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
            {
                property.InitializeProperty(newObject, srcObject, this, loadCache, initializeReferencedObjects);
            }

            //load properties of the base type
            if (null != dataObjectMetaInfo.BaseType)
            {
                InitializeObject(newObject, srcObject, dataObjectMetaInfo.BaseType, loadCache, initializeReferencedObjects);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void SaveObjectInter(IDataObject obj, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            List<SofDbCommandParameter> dbParams = new List<SofDbCommandParameter>();
            foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
            {
                dbParams.AddRange(property.SaveProperty(obj, this));
            }

            StringBuilder commandBuilder = new StringBuilder(20 + dataObjectMetaInfo.TableName.Length + dataObjectMetaInfo.Properties.Count * 10);
            commandBuilder.Append("INSERT INTO [").Append(dataObjectMetaInfo.TableName).Append("] ([Id]");
            foreach (SofDbCommandParameter dbParam in dbParams)
            {
                commandBuilder.Append(',').Append(dbParam.Column);
            }
            commandBuilder.Append(") VALUES (").Append(obj.Id);
            foreach (SofDbCommandParameter dbParam in dbParams)
            {
                commandBuilder.Append(',').Append(dbParam.Name);
            }
            commandBuilder.Append(')');
            DataLayerManager.ExecuteNonQueryCommand(commandBuilder.ToString(), dbParams);

            //save properties of the base type
            if (null != dataObjectMetaInfo.BaseType)
            {
                SaveObjectInter(obj, dataObjectMetaInfo.BaseType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="origObj"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void UpdateObjectInter(IDataObject newObj, IDataObject origObj, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            if (newObj.Id != origObj.Id)
            {
                throw new ArgumentException("newObj.Id != originalObj.Id");
            }

            if (0 < dataObjectMetaInfo.Properties.Count)
            {
                List<SofDbCommandParameter> dbParams = new List<SofDbCommandParameter>();
                foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
                {
                    dbParams.AddRange(property.UpdateProperty(newObj, origObj, this));
                }

                if (0 < dbParams.Count) //e.g. only list properties
                {
                    StringBuilder commandBuilder = new StringBuilder(20 + dataObjectMetaInfo.TableName.Length + dataObjectMetaInfo.Properties.Count * 10);
                    commandBuilder.Append("UPDATE [").Append(dataObjectMetaInfo.TableName).Append("] SET ");
                    for (int i = 0; i < dbParams.Count; ++i)
                    {
                        if (0 != i)
                        {
                            commandBuilder.Append(',');
                        }
                        SofDbCommandParameter dbParam = dbParams[i];
                        commandBuilder.Append(dbParam.Column).Append('=').Append(dbParam.Name);
                    }

                    commandBuilder.Append(" WHERE [Id]=").Append(newObj.Id.ToString());
                    DataLayerManager.ExecuteNonQueryCommand(commandBuilder.ToString(), dbParams);
                }
            }

            //base types
            if (null != dataObjectMetaInfo.BaseType)
            {
                UpdateObjectInter(newObj, origObj, dataObjectMetaInfo.BaseType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void DeleteObjectInter(IDataObject dataObject, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            DataLayerManager.ExecuteNonQueryCommand(string.Format("DELETE FROM [{0}] WHERE [Id] = {1}", dataObjectMetaInfo.TableName, dataObject.Id.ToString()));

            //delete properties
            foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
            {
                property.DeleteProperty(dataObject, this);
            }

            //delete base type
            if (null != dataObjectMetaInfo.BaseType)
            {
                DeleteObjectInter(dataObject, dataObjectMetaInfo.BaseType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="dataObjectMetaInfo"></param>
        private void RepairObjectPropertiesId(IDataObject dataObject, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
            {
                property.RepairId(this, dataObject);
            }

            if (null != dataObjectMetaInfo.BaseType)
            {
                RepairObjectPropertiesId(dataObject, dataObjectMetaInfo.BaseType);
            }
        }

        private void ClearObjectPropertiesId(IDataObject dataObject, DataObjectTypeMetaInfo dataObjectMetaInfo)
        {
            foreach (IPropertyMetaInfo property in dataObjectMetaInfo.Properties)
            {
                property.ClearId(this, dataObject);
            }

            if (null != dataObjectMetaInfo.BaseType)
            {
                ClearObjectPropertiesId(dataObject, dataObjectMetaInfo.BaseType);
            }
        }
    }
}
