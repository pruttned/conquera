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
using Ale.Scene;
using Ale.Content;

namespace Ale.Editor
{
    public interface IRenderInfo
    {
        BaseScene Scene { get; }
        void Update(object assetSettings, SceneManager sceneManager, ContentGroup content);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class RenderInfoAttribute : Attribute
    {
        public string SourceDirectory { get; private set; }

        public RenderInfoAttribute(string sourceDirectory)
        {
            SourceDirectory = sourceDirectory;
        }
    }
}
