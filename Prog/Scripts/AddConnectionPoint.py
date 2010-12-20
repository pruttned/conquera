##################################################################
#  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
#  Part of the Conquera Project
#
#  This program is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 2 of the License, or
#  (at your option) any later version.
#
#  This program is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
##################################################################

#!BPY
"""
Name: 'ConnectionPoint'
Blender: 243
Group: 'AddMesh'
"""

import BPyAddMesh
import Blender
import math
from Blender import NMesh
from Blender import Mesh, Object, Armature
import random
import bpy
from Blender.Mathutils import Vector, Euler, Quaternion, Matrix
 
Vector = Blender.Mathutils.Vector
RotationMatrix = Blender.Mathutils.RotationMatrix


def CreateCircleMesh(size, axis):
    if not axis in ['x', 'y', 'z']:
        raise Exception("Axis '%s' is not valid. Valid values are 'x', 'y' and 'z'" % axis)
    
    mesh = Mesh.Primitives.Circle(20, size)

    vCnt = len(mesh.verts)
    aSize = size/3
    aSize2 = size/8
    aSize3 =aSize2/2
    v = [(0, aSize, 0), (0,0,0), (aSize3, aSize-aSize2, aSize3), (-aSize3, aSize-aSize2, aSize3), (aSize3, aSize-aSize2, -aSize3), (-aSize3, aSize-aSize2, -aSize3)]
    mesh.verts.extend(v)
    mesh.edges.extend([(vCnt, vCnt+i) for i in range(1,len(v))])


    #axis name
    vCnt = len(mesh.verts)
    if 'x' == axis:
        v = [(aSize3 +aSize2/4, aSize3+aSize2,0), (aSize3*2 +aSize2/4, aSize3*2+aSize2,0), (aSize3*2 +aSize2/4, aSize3+aSize2,0), (aSize3 +aSize2/4, aSize3*2+aSize2,0)]
        e = [(vCnt, vCnt+1), (vCnt+2, vCnt+3)]
    elif 'y' == axis:
        v = [(aSize3 +aSize2/4, aSize3 + aSize3/2 +aSize2,0), (aSize3 + aSize3/2 +aSize2/4, aSize3+aSize2,0), (aSize3*2 +aSize2/4, aSize3 + aSize3/2 +aSize2,0), (aSize3 + aSize3/2 +aSize2/4, aSize3/2 +aSize2,0)]
        e = [(vCnt, vCnt+1), (vCnt+1, vCnt+2), (vCnt+1, vCnt+3)]
    else:
        v = [(aSize3 +aSize2/4, aSize3+aSize2,0), (aSize3*2 +aSize2/4, aSize3*2+aSize2,0), (aSize3*2 +aSize2/4, aSize3+aSize2,0), (aSize3 +aSize2/4, aSize3*2+aSize2,0)]
        e = [(vCnt, vCnt+2), (vCnt+2, vCnt+3), (vCnt+3, vCnt+1)]
    mesh.verts.extend(v)
    mesh.edges.extend(e)

    if  'x' == axis:
        mesh.transform(RotationMatrix(-90, 4, 'z'))
    elif 'z' == axis:
        mesh.transform(RotationMatrix(90, 4, 'x'))
    
    return mesh

def GetSelectedBone(armature):
    for bone in armature.bones.values():
        if(Armature.BONE_SELECTED in bone.options):
            return bone
    return None


size = 0.2

selObj = bpy.data.scenes.active.objects.active

objs = []
for axis in ['x', 'y', 'z']:
    mesh = CreateCircleMesh(size, axis)
    obj = Object.New("Mesh", "ConnectionPoint")
    obj.link(mesh)
    bpy.data.scenes.active.link(obj)
    objs.append(obj)


obj = objs[0]

obj.join([objs[1], objs[2]])
bpy.data.scenes.active.objects.unlink(objs[1])
bpy.data.scenes.active.objects.unlink(objs[2])

obj.addProperty("IsConnectionPoint", True, "BOOL")

obj.select(True)

if selObj:
    if "Armature" == selObj.getType():
        bone = GetSelectedBone(selObj.getData())
        if bone:
            selObj.makeParentBone([obj], bone.name)
            
    else:
        selObj.makeParent([obj])

    obj.setMatrix(Matrix())
    selObj.select(False)
    

Blender.Redraw()
