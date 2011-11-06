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
using Ale.SpecialEffects;
using Ale.Tools;

namespace Conquera
{
    public class CameraShakeTimeTriggerAction : ISpecialEffectTimeTriggerAction
    {
        private GameCamera mCamera;

        public string Name
        {
            get { return "CameraShake"; }
        }

        public CameraShakeTimeTriggerAction(GameCamera camera)
        {
            if (null == camera) throw new ArgumentNullException("camera");

            mCamera = camera;
        }
        public void Execute(float timeInAnim, SpecialEffect specialEffect, IDictionary<NameId, string> parameters)
        {
            if (null == specialEffect) throw new ArgumentNullException("specialEffect");
            if (null == parameters) throw new ArgumentNullException("parameters");

            mCamera.Shake();
        }
    }
}
