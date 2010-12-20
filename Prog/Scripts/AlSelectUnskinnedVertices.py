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
Name: 'Select unskinned vertices'
Blender: 244
Group: 'Mesh'
Tooltip: 'Selects all vertices with no weight assigned'
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


#-----START-----

activeScene= bpy.data.scenes.active

activeObject = bpy.data.scenes.active.objects.active

if activeObject is None:
    raise Exception("You must select an mesh object")
    
if "Mesh" != activeObject.type:
    raise Exception("Selected object must be of Mesh type")

mesh = activeObject.getData(mesh=1)

selVerCnt = 0

isEditmode = Blender.Window.EditMode()
if isEditmode:
    Blender.Window.EditMode(0)
Blender.Window.WaitCursor(1)
for v in mesh.verts:
    if 0 == len(mesh.getVertexInfluences(v.index)):
        v.sel = 1
        selVerCnt += 1
    else:
        v.sel = 0
if isEditmode:
    Blender.Window.EditMode(1)

Blender.Window.WaitCursor(0)

Draw.PupMenu("Selected vertex count = %d" % selVerCnt)

Redraw()
