##################################################################
#  Copyright (C) 2010 by Conquera Team
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
Name: 'WaterHexCells'
Blender: 243
Group: 'AddMesh'
"""

import BPyAddMesh
import Blender
import math
from Blender import NMesh
import random
from Blender.Mathutils import Vector, Euler, Quaternion
 
Vector = Blender.Mathutils.Vector
RotationMatrix = Blender.Mathutils.RotationMatrix

def Cup(obj, z=0):
    newFaces = []
    vc=NMesh.Vert(0.0,0.0,z)
    
    obj.verts.append(vc)
    
    vOld = NMesh.Vert(0.0,1.0,z)
    vFirst = vOld
    obj.verts.append(vOld)
    for i in range(1,6):
        mtx = RotationMatrix( i  * 60, 3, 'z' )
        vect = Vector(0,1,z) * mtx
        
        v=NMesh.Vert(vect.x, vect.y, vect.z)

        obj.verts.append(v)
        cf = NMesh.Face([vc, vOld, v])
            
        obj.addFace(cf)
        newFaces.append(cf)
        vOld = v

    cf = NMesh.Face([vc, vOld, vFirst])

    obj.addFace(cf)
    newFaces.append(cf)
    
    for face in newFaces:
        face.uv = [(face.v[i].co.x/2+0.5, face.v[i].co.y/2+0.5) for i in range(0,3)]
 

def Body(obj, cell, bottomZ = 0, topZ = 3):
    newFaces = []

    sideSize = 1.0/float(topZ-bottomZ)

    cRot = 1;
    
    for i in range(0,6):
        if cell[5-i] == 1:
            mtx = RotationMatrix( (i+cRot)  * 60, 3, 'z' )
            bottomVect1 = Vector(0,1,bottomZ) * mtx
            topVect1 = Vector(0,1,topZ) * mtx

            mtx = RotationMatrix( (i+1+cRot)  * 60, 3, 'z' )
            bottomVect2 = Vector(0,1,bottomZ) * mtx
            topVect2 = Vector(0,1,topZ) * mtx
        
            vBottom1=NMesh.Vert(bottomVect1.x, bottomVect1.y, bottomVect1.z)
            vTop1=NMesh.Vert(topVect1.x, topVect1.y, topVect1.z)
            vBottom2=NMesh.Vert(bottomVect2.x, bottomVect2.y, bottomVect2.z)
            vTop2=NMesh.Vert(topVect2.x, topVect2.y, topVect2.z)
            
            obj.verts.append(vBottom1)
            obj.verts.append(vTop1)
            obj.verts.append(vBottom2)
            obj.verts.append(vTop2)
            

            cf = NMesh.Face([vTop1, vTop2, vBottom2, vBottom1])
            obj.addFace(cf)
            cf.uv = [(0, 0), (sideSize, 0), (sideSize, 1), (0, 1)]


x=0
for i in [  [0,0,0,0,0,0],
            [0,0,0,0,0,1],
            [0,0,0,0,1,1],
            [0,0,0,1,0,1],
            [0,0,0,1,1,1],
            [0,0,1,0,0,1],
            [0,0,1,0,1,1],
            [0,0,1,1,0,1],
            [0,0,1,1,1,1],
            [0,1,0,1,0,1],
            [0,1,0,1,1,1],
            [0,1,1,0,1,1],
            [0,1,1,1,1,1],
            [1,1,1,1,1,1],
                        ]:
    meshname = "".join(str(e) for e in i)

    obj = NMesh.GetRaw()
    Cup(obj, 0) 
    Body(obj, i) 

    obj.hasFaceUV(1)
    obj.update()
    
    object = NMesh.PutRaw(obj, meshname, 1)
    
    if object:
        object.name = meshname
    else:
        object = Blender.Object.Get(meshname)
    object.setLocation((x, 0, 0))
    x+=2
 
Blender.Redraw()
