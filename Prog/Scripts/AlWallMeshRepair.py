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
Name: 'Al Wall Mesh Repair'
Blender: 244
Group: 'Mesh'
Tooltip: 'Ensures that the vertices of the wall's borders will have correct positions'
"""

import Blender
import bpy
import BPyObject
import BPyMesh
import BPySys
import BPyMessages
import sys
from Blender import *
from xml.dom.minidom import Document
import math
from operator import itemgetter  


def CheckXyBorder(vert, border):
    if abs(vert.co.x - border) < 0.002 : # x  border
        vert.co.x = border
    if abs(vert.co.x + border) < 0.002 : # x -border
        vert.co.x = -border
    if abs(vert.co.y - border) < 0.002 : # y  border
        vert.co.y = border
    if abs(vert.co.y + border) < 0.002 : # y  -border
        vert.co.y = -border

def CheckZBorder(vert, border):
    if abs(vert.co.z - border) < 0.002 : # z   border
        vert.co.z = border

#-----START-----

bxy1 = 0.5
bxy2 = 0.2
bz1 = 2.01
bz2 = 2
bz3 = 0

print "===================="


isEditmode = Window.EditMode()
if isEditmode:
    Window.EditMode(0)
Mesh.Mode(Mesh.SelectModes['VERTEX'])
Window.WaitCursor(1)

mesh = bpy.data.scenes.active.objects.active.getData(mesh=1)

print "Before:"
for v in mesh.verts:
    print v

for v in mesh.verts:
    CheckXyBorder(v, bxy1)
    CheckXyBorder(v, bxy2)
    CheckZBorder(v, bz1)
    CheckZBorder(v, bz2)
    CheckZBorder(v, bz3)


mesh.update()

print "After:"
for v in mesh.verts:
    print v

if isEditmode:
    Window.EditMode(1)
Window.WaitCursor(0)

