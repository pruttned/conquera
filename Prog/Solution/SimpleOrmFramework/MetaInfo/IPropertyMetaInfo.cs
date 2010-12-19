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
using System.Linq;
using System.Text;
using System.Data.Common;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    interface IPropertyMetaInfo
    {
        /// <summary>
        /// 
        /// </summary>
        string PropertyName
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        SofDbCommandParameter[] SaveProperty(IDataObject obj, OrmManager ormManager);
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="origObj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        SofDbCommandParameter[] UpdateProperty(IDataObject newObj, IDataObject origObj, OrmManager ormManager);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        void DeleteProperty(IDataObject obj, OrmManager ormManager);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string[] GetMainTableCreateCommandColumns();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        void ExecuteAdditionalCreateTableCommands(DataLayerManager dataLayerManager);

        /// <summary>
        /// Init prop from db
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="dbReader"></param>
        /// <param name="ormManager"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        void InitializeProperty(IDataObject newObject, DbDataReader dbReader, OrmManager ormManager, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects);

        /// <summary>
        /// Init prop from src object
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="srcObject"></param>
        /// <param name="ormManager"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        void InitializeProperty(IDataObject newObject, IDataObject srcObject, OrmManager ormManager, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects);

        /// <summary>
        /// Remove strong referenced data objects stored in property from cache
        /// </summary>
        /// <param name="ormManager"></param>
        /// <param name="obj"></param>
        void RemoveFromCache(OrmManager ormManager, IDataObject obj);

        /// <summary>
        /// Repairs object's Id and Ids of all his strong referenced childs.
        /// Repairing of an Id means, the it will be set to -1 if it is invalid.
        /// This state can occure after rollbacking the transaction in which was object saved or its saving has been started.
        /// </summary>
        /// <param name="ormManager"></param>
        /// <param name="obj"></param>
        void RepairId(OrmManager ormManager, IDataObject obj);

        void ClearId(OrmManager ormManager, IDataObject dataObject);
    }
}
