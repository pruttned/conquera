#!BPY
"""
Name: 'HexCell'
Blender: 243
Group: 'AddMesh'
"""

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

import BPyAddMesh
import Blender
import math
from Blender import NMesh
import random
from Blender.Mathutils import Vector, Euler, Quaternion
 
Vector = Blender.Mathutils.Vector
RotationMatrix = Blender.Mathutils.RotationMatrix

def Cup(obj, z=3, top=True):
    newFaces = []
    if top:
        vc=NMesh.Vert(0.0,0.0,z)
    else:
        vc=NMesh.Vert(0.0,0.0,z+1)
    
    obj.verts.append(vc)
    
    vOld = NMesh.Vert(0.0,1.0,z)
    vFirst = vOld
    obj.verts.append(vOld)
    for i in range(1,6):
        mtx = RotationMatrix( i  * 60, 3, 'z' )
        vect = Vector(0,1,z) * mtx
        
        v=NMesh.Vert(vect.x, vect.y, vect.z)

        obj.verts.append(v)
        if top:
            cf = NMesh.Face([vc, vOld, v])
        else:
            cf = NMesh.Face([vc, v, vOld])
            
        obj.addFace(cf)
        newFaces.append(cf)
        vOld = v

    if top:
        cf = NMesh.Face([vc, vOld, vFirst])
    else:
        cf = NMesh.Face([vc, vFirst, vOld])

    obj.addFace(cf)
    newFaces.append(cf)
    
    for face in newFaces:
        face.uv = [(face.v[i].co.x/2+0.5, face.v[i].co.y/2+0.5) for i in range(0,3)]
 

def Body(obj, bottomZ = -1, topZ = 0):
    newFaces = []
    vBottomOld = NMesh.Vert(0.0,1.0,bottomZ)
    vTopOld = NMesh.Vert(0.0,1.0,topZ)
    obj.verts.append(vBottomOld)
    obj.verts.append(vTopOld)

    for i in range(1,6):
        mtx = RotationMatrix( i  * 60, 3, 'z' )
        bottomVect = Vector(0,1,bottomZ) * mtx
        topVect = Vector(0,1,topZ) * mtx
        
        vBottom=NMesh.Vert(bottomVect.x, bottomVect.y, bottomVect.z)
        vTop=NMesh.Vert(topVect.x, topVect.y, topVect.z)
        obj.verts.append(vBottom)
        obj.verts.append(vTop)
        cf = NMesh.Face([vTop, vTopOld, vBottomOld, vBottom])
        obj.addFace(cf)
        newFaces.append(cf)
        vBottomOld = vBottom
        vTopOld = vTop


    vBottomLast = NMesh.Vert(0.0,1.0,bottomZ)
    vTopLast = NMesh.Vert(0.0,1.0,topZ)
    obj.verts.append(vBottomLast)
    obj.verts.append(vTopLast)

    cf = NMesh.Face([vTopLast, vTopOld, vBottomOld, vBottomLast])
 
    obj.addFace(cf)
    newFaces.append(cf)


    i=0.0
    topV = 0.5
    fCnt = len(newFaces)
    fLength = 1/float(fCnt)
    for face in newFaces:
        face.uv = [(i+fLength, topV), (i, topV), (i, 0), (i+fLength, 0)]
        print(list(face.uv))
        i+=fLength


obj = NMesh.GetRaw()

#Cup(obj) 
#Cup(obj, 0, False) 
Body(obj) 

obj.hasFaceUV(1)
obj.update()

NMesh.PutRaw(obj, "test", 1)
 
Blender.Redraw()
