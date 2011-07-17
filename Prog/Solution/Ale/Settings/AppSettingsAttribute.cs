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

namespace Ale.Settings
{
    /// <summary>
    /// Defines AppSettings
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppSettingsAttribute : Attribute
    {
        /// <summary>
        /// Name of the file that contains the settings.
        /// It is possible to contaion multiple AppSettings in one file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Name of the section in the ini file.
        /// If null, then the class name is used
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Can't be commited.
        /// Default = false
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">- Name of the file that contains the settings. It is possible to contaion multiple AppSettings in one file. </param>
        public AppSettingsAttribute(string fileName)
        {
            FileName = fileName;
            Readonly = false;
        }
    }
}
