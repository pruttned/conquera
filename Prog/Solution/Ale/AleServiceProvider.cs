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
using System.Linq;
using System.Text;

namespace Ale
{
    public interface IAleServiceProvider : IServiceProvider
    {
        T GetServiceWithCheck<T>();
        T GetService<T>();
        object GetServiceWithCheck(Type serviceType);
        void RegisterService(Type serviceType, object service);
    }

    public class AleServiceProvider : IAleServiceProvider
    {
        private IServiceProvider mParent;
        private Dictionary<Type, object> mServices = new Dictionary<Type, object>();

        public AleServiceProvider()
            :this(null)
        {
        }

        public AleServiceProvider(IServiceProvider parent)
        {
            mParent = parent;
        }

        public object GetService(Type serviceType)
        {
            if (null == serviceType) throw new ArgumentNullException("serviceType");
            
            //check own services
            object service;
            if (mServices.TryGetValue(serviceType, out service))
            {
                return service;
            }

            //check parent
            if(null != mParent)
            {
                return mParent.GetService(serviceType);
            }
            return null;
        }

        public object GetServiceWithCheck(Type serviceType)
        {
            var service = GetService(serviceType);
            if (null == service)
            {
                throw new KeyNotFoundException(string.Format("Service of type '{0}' has not been registered", serviceType.FullName));
            }
            return service;
        }

        public T GetServiceWithCheck<T>()
        {
            return (T)GetServiceWithCheck(typeof(T));
        }
        public  T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }


        public void RegisterService(Type serviceType, object service)
        {
            if (null == serviceType) throw new ArgumentNullException("serviceType");
            if (null == service) throw new ArgumentNullException("service");

            if(!serviceType.IsAssignableFrom(service.GetType()))
            {
                throw new ArgumentException(string.Format("Service '{0}' doesn't implements '{1}'", serviceType.FullName, service.GetType().FullName));
            }

            mServices.Add(serviceType, service);
        }
    }
}
