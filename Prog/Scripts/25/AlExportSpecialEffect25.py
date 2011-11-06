# #################################################################
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
# #################################################################

bl_info = {
    "name": "Ale Special Effect",
    "blender": (2, 5, 7),
    "location": "File > Export > AleSpecialEffect",
    "description": "Ale Special Effect",
    "warning": "",
    "category": "Import-Export"}

import bpy
import sys
from collections import OrderedDict
from bpy.props import *
from xml.dom.minidom import Document
import math
from mathutils import Vector, Euler, Matrix
import re
from AlExportModel25 import VertexCollection, WriteSubmesh

def WriteTransformation(xmlDoc, parentElement, transfMatrix):
    translation = transfMatrix.to_translation()
    orientation = transfMatrix.to_quaternion()
    scaleVect = transfMatrix.to_scale()
    
    #scale must be uniform
    if(0.000001 < abs(scaleVect.x - scaleVect.y) or 0.000001 < abs(scaleVect.x - scaleVect.z)):
        raise Exception("Only uniform scale is suported")
    uniformScale = scaleVect.x
    
    if (0.000001 < abs(translation.x) or 0.000001 < abs(translation.y) or 0.000001 < abs(translation.z)):
        transElement = xmlDoc.createElement("translation")
        parentElement.appendChild(transElement)
        transElement.setAttribute("x", "%.6f" % translation.x)
        transElement.setAttribute("y", "%.6f" % translation.y)
        transElement.setAttribute("z", "%.6f" % translation.z)

    if (0.000001 < abs(orientation.x) or 0.000001 < abs(orientation.y) or 0.000001 < abs(orientation.z) or 0.000001 < abs(orientation.w - 1.0)):
        orientationElement = xmlDoc.createElement("orientation")
        parentElement.appendChild(orientationElement)
        orientationElement.setAttribute("x", "%.6f" % orientation.x)
        orientationElement.setAttribute("y", "%.6f" % orientation.y)
        orientationElement.setAttribute("z", "%.6f" % orientation.z)
        orientationElement.setAttribute("w", "%.6f" % orientation.w)

    if (0.00001 < abs(uniformScale - 1)):
        scaleElement = xmlDoc.createElement("scale")
        parentElement.appendChild(scaleElement)
        scaleElement.setAttribute("v", "%.6f" % uniformScale)

class Exporter(bpy.types.Operator):
    '''Save Ale Special Effect'''
    bl_idname = "export_ale_special_effect.alsfx"
    bl_label = "Export Ale Special Effect"

    filepath = StringProperty(name="File Path", description="Filepath", maxlen= 1024, default= "", subtype='FILE_PATH')

    def execute(self, context):
        scene = bpy.context.scene

        scene.frame_set(0)

        xmlDoc = Document()
        
        specialEffectElement = xmlDoc.createElement("specialEffect")
        xmlDoc.appendChild(specialEffectElement)
        objsElement = xmlDoc.createElement("objects")
        specialEffectElement.appendChild(objsElement)
        
        animatedObjects = []
        endFrame = -1   
        fps = bpy.context.scene.render.fps
        
        for obj in scene.objects:
            if "ignore_" != obj.name[0:7] and obj.type == "MESH":
                
                objElm = None
                
                #dummy
                if obj.name.startswith("dummy_") : 
                    objElm = xmlDoc.createElement("dummy")
                    objsElement.appendChild(objElm)
                    objElm.setAttribute("name", obj.name[6:])
                    WriteTransformation(xmlDoc, objElm, obj.matrix_world)
                    
                #Particle system
                elif obj.name.startswith("psys_"):
                    objElm = xmlDoc.createElement("particleSystem")
                    objsElement.appendChild(objElm)
                    objElm.setAttribute("name", obj.name[5:])
                    
                    if "psys" not in obj:
                        raise Exception("Missing psys property in {}".format(obj.name))
                    objElm.setAttribute("psys", obj["psys"])
                    
                    WriteTransformation(xmlDoc, objElm, obj.matrix_world)
                    
                #Mesh object
                else:
                    objElm = xmlDoc.createElement("meshObject")
                    objsElement.appendChild(objElm)
                    objElm.setAttribute("name", obj.name)
                    WriteTransformation(xmlDoc, objElm, obj.matrix_world)
                    
                    meshElement = xmlDoc.createElement("mesh")
                    meshElement.setAttribute("name", obj.name)
                    objElm.appendChild(meshElement)

                    bMesh = obj.to_mesh(scene, True, "PREVIEW")
                    verticies = VertexCollection(bMesh, None)
                    for i in range(0, len(bMesh.materials)):
                        WriteSubmesh(xmlDoc, meshElement, verticies, bMesh, i)
                    verticies.WriteToXml(xmlDoc, meshElement)

            #Has animation
            if obj.animation_data is not None and obj.animation_data.action is not None:
                animElm = xmlDoc.createElement("anim")
                objElm.appendChild(animElm)
                animatedObjects.append((obj, animElm)) 
                frameRange = obj.animation_data.action.frame_range
                if endFrame == -1 or endFrame < frameRange.y:
                    endFrame = frameRange.y
                animElm.setAttribute("duration", "%.6f" % ((frameRange.y  - frameRange.x)/float(fps)))

        #max time marker
        if 0 < len(scene.timeline_markers):
            for timeMarker in scene.timeline_markers:
                frame = timeMarker.frame
                if endFrame == -1 or endFrame < frame:
                    endFrame = frame
                
        #special effect duration        
        endFrame = int(endFrame)
        specialEffectElement.setAttribute("duration", "%.6f" % ((frameRange.y  - frameRange.x)/float(fps)))

        #Animations
        if 0 < len(animatedObjects):
            for frame in range(0, endFrame + 1):
                frameTime = frame/float(fps)
                scene.frame_set(frame)
                
                for obj, animElm in animatedObjects:
                    frameRange = obj.animation_data.action.frame_range
                    if frame >= frameRange.x and frame <= frameRange.y:
                        keyframeElement = xmlDoc.createElement("keyframe")
                        keyframeElement.setAttribute("time", "%.6f" % frameTime)
                        animElm.appendChild(keyframeElement)
                        WriteTransformation(xmlDoc, keyframeElement, obj.matrix_world)


        #Time markers
        if 0 < len(scene.timeline_markers):
            timeTriggersElement = xmlDoc.createElement("timeTriggers")
            specialEffectElement.appendChild(timeTriggersElement)
            for timeMarker in scene.timeline_markers:
                name = timeMarker.name
                frame = timeMarker.frame
                frameTime = frame/float(fps)
                text = bpy.data.texts[name]
                for line in text.lines:
                    lineText = line.body
                    actionName = lineText[0:lineText.index('(')].strip()
                    timeTriggerElement = xmlDoc.createElement("timeTrigger")
                    timeTriggersElement.appendChild(timeTriggerElement)
                    timeTriggerElement.setAttribute("action", actionName)
                    timeTriggerElement.setAttribute("time", "%.6f" % frameTime)
                    #action params
                    for m in re.finditer("\s*(?P<param>[\w_]+)\s*=\s*\"(?P<value>[^\"]+)\"", lineText):
                        paramElement = xmlDoc.createElement("param")
                        timeTriggerElement.appendChild(paramElement)
                        paramElement.setAttribute("name", m.group("param"))
                        paramElement.setAttribute("value", m.group("value"))
                    
        #write xmlDoc to file
        outFile = open(self.filepath, "w")
        xmlDoc.writexml(outFile, addindent="    ", newl="\n")
        outFile.close()
        
        return {'FINISHED'}



    def invoke(self, context, event):
        wm = context.window_manager
        wm.fileselect_add(self)
        return {'RUNNING_MODAL'}




# package manages registering

def menu_func(self, context):
    import os
    default_path = os.path.splitext(bpy.data.filepath)[0] + ".alsfx"
    self.layout.operator("export_ale_special_effect.alsfx", text="Ale Special Effect (.alsfx)").filepath = default_path

def register():
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_export.append(menu_func)

def unregister():
    bpy.utils.unregister_module(__name__)
    bpy.types.INFO_MT_file_export.remove(menu_func)
    
if __name__ == "__main__":
    register()
