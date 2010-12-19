#!BPY
"""
Name: 'MyMesh'
Blender: 243
Group: 'AddMesh'
"""

import BPyAddMesh
import Blender
import math
from Blender import NMesh
import random
from Blender.Mathutils import Vector, Euler, Quaternion
 
def RandomNum(base, variation):
    return 2*(random.random() - 0.5)*variation + base
 
def perpendicular(vec):
    #avoid parallel  vector
    if vec.x < vec.y:
        if vec.x < vec.z:
            perp = vec.cross(Vector(1,0,0))
        else:
            perp = vec.cross(Vector(0,0,1))
    else:
        if vec.y < vec.z:
            perp = vec.cross(Vector(0,1,0))
        else:
            perp = vec.cross(Vector(0,0,1))
       
    perp.normalize()
    return perp
    
def randomDeviant(vec, angle):
    up = perpendicular(vec)
    q = Quaternion(vec, random.random() *360)
    up =  q*up
    q = Quaternion(up, angle)
    return q * vec   
    
obj = NMesh.GetRaw()
 
var =90
 
direction = Vector(0.5,0.5,0).normalize()
#direction.normalize()
 
dirPerp = perpendicular(direction)

for i in range(1, 10000):
    v1=NMesh.Vert(0.0,0.0,0.0)

    #random perpendicular vector to direction
    rotPerp  =  Quaternion(direction, random.random() *360) * dirPerp
    #rotate arround  random  perpendicular  vector
    vec =  Quaternion(rotPerp, random.random() * var) * direction

    #vec.normalize()
    v2=NMesh.Vert(vec.x, vec.y, vec.z)
    
    obj.verts.append(v1)
    obj.verts.append(v2)
    obj.addEdge(v1,v2)
 
NMesh.PutRaw(obj, "test", 1)
 
Blender.Redraw()