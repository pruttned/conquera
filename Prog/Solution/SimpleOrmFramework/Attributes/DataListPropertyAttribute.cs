﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Marks the list property that should be serialized in the db.
    /// It can be placed above properties of types that implements generic ICollection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataListPropertyAttribute : Attribute, IDataPropertyAttribute
    {
        private bool mWeakReference = false;
        private bool mNotNull = false;

        /// <summary>
        /// If property is weak reference, then items in list are not updated according to their state in list.
        /// (For instance if list is not a weak reference and some item is removed from this list, then during save of the parent object, object stored in the item 
        /// will be removed from db)
        /// Default = false.
        /// </summary>
        public bool WeakReference
        {
            get { return mWeakReference; }
            set { mWeakReference = value; }
        }

        /// <summary>
        /// Whether nulls are not allowed in list.
        /// Defaul = false.
        /// </summary>
        public bool NotNull
        {
            get { return mNotNull; }
            set { mNotNull = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IPropertyMetaInfo IDataPropertyAttribute.CreatePropertyMetaInfo(PropertyInfo propertyInfo, Dictionary<Type, ICustomBasicTypeProvider> customBasicTypeProviders)
        {
            Type propertyType = propertyInfo.PropertyType;
            Type itemType = GetCollectionItemType(propertyType);
            
            if (null == itemType || !typeof(IList).IsAssignableFrom(propertyType))
            {
                throw new ArgumentException(string.Format("Type '{0}' of the property '{1}' in type '{2}' must implement ICollection<T> and (nongeneric)IList", propertyInfo.PropertyType.FullName, propertyInfo.Name, propertyInfo.DeclaringType.FullName));
            }

            ICustomBasicTypeProvider customBasicTypeProvider;
            if (customBasicTypeProviders.TryGetValue(itemType, out customBasicTypeProvider))
            {
                return new CustomBasicTypeListPropertyMetaInfo(propertyInfo, itemType, this, customBasicTypeProvider);
            }
            else
            {
                return new ListPropertyMetaInfo(propertyInfo, itemType, this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionType"></param>
        /// <returns>type or null</returns>
        private Type GetCollectionItemType(Type collectionType)
        {
            foreach (Type interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType)
                {
                    Type genericType = interfaceType.GetGenericTypeDefinition();
                    if (typeof(ICollection<>) == genericType)
                    {
                        Type[] genericAttributes = interfaceType.GetGenericArguments();
                        return genericAttributes[0];
                    }
                }
            }
            return null;
        }
    }
}
