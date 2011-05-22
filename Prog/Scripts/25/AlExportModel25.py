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
    "name": "Ale Mesh",
    "blender": (2, 5, 7),
    "location": "File > Export > ALM",
    "description": "Exports Ale mesh",
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

def WriteTransformation(xmlDoc, parentElement, translation, orientation):
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

def WriteSubmesh(xmlDoc, meshElement, verticies, mesh, materialIndex):
    submeshElement = xmlDoc.createElement("submesh")
    meshElement.appendChild(submeshElement)
    
    submeshElement.setAttribute("material", mesh.materials[materialIndex].name)
        
    #write faces that belongs to the this submesh (by material)
    for bFace in mesh.faces:
        if bFace.material_index == materialIndex:
            WriteFaceToXml(xmlDoc, submeshElement, verticies, bFace)

def WriteTriangleFaceToXml(xmlDoc, submeshElement, v0Index, v1Index, v2Index):
    faceElement = xmlDoc.createElement("face")
    submeshElement.appendChild(faceElement)
    faceElement.setAttribute("v0", str(v0Index))
    faceElement.setAttribute("v1", str(v1Index))
    faceElement.setAttribute("v2", str(v2Index))

def WriteFaceToXml(xmlDoc, submeshElement, verticies, bFace):
    if 4 < len(bFace.vertices):
        raise Exception("Only faces with three or four vertices are supported")
    
    bVertices = verticies.mBMesh.vertices
    
    vIndexes = []
    for i in range(0, len(bFace.vertices)):
        vIndexes.append(verticies.GetVertexIndex(bFace, i))
        
    if 3 == len(bFace.vertices): #triangle
        WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
    else: # quad => split it to triangles
        if (bVertices[bFace.vertices[2]].co - bVertices[bFace.vertices[0]].co).length < (bVertices[bFace.vertices[3]].co - bVertices[bFace.vertices[1]].co).length: #0-2
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[0])
        else: #1-3
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[1], vIndexes[0])
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[1])
            
            
def GetObjChildrens(obj):
    return filter(lambda o : o.parent == obj, bpy.context.scene.objects)
             
def IsConnectionPointObject(obj):
    return "cp_" == obj.name[0:3]
    
def GetChildConnectionPoints(obj):
    return filter(lambda o : IsConnectionPointObject(o), GetObjChildrens(obj))




class AlmExporter(bpy.types.Operator):
    '''Save Ale model'''
    bl_idname = "export_mesh.alm"
    bl_label = "Export ALM"

    filepath = StringProperty(name="File Path", description="Filepath", maxlen= 1024, default= "", subtype='FILE_PATH')


    def execute(self, context):
        xmlDoc = Document()
        
        modelElement = xmlDoc.createElement("model")
        xmlDoc.appendChild(modelElement)
    
        #write active object as a mesh
        self.WriteSelectedMesh(xmlDoc, modelElement)
    
        #write xmlDoc to file
        outFile = open(self.filepath, "w")
        xmlDoc.writexml(outFile, addindent="    ", newl="\n")
        outFile.close()

        return {'FINISHED'}


    def WriteSelectedMesh(self, xmlDoc, modelElement):
        activeObject = bpy.context.scene.objects.active
        scene = bpy.context.scene
    
        if activeObject is None:
            raise Exception("You must select an object that should be exported")
        
        if "MESH" != activeObject.type:
            raise Exception("Selected object must be Mesh type")

        bBoneIds=None
        
        #ignore armature modifiers    
        armsOrigRestValues = [arm.pose_position for arm in bpy.data.armatures]
        for arm in bpy.data.armatures:
            arm.pose_position = 'REST'
        if armsOrigRestValues:
            for o in bpy.data.objects:
                if 'ARMATURE' == o:
                    o.update_tag()
            scene.frame_set(scene.frame_current)
            
        bMesh = activeObject.to_mesh(scene, True, "PREVIEW")
        
        
        ##########
        #Connection points
        ##########
        connectionPointsElement = xmlDoc.createElement("connectionPoints")			
        arm = activeObject.find_armature()
        if arm is not None:
            for obj in GetChildConnectionPoints(arm): #connection point attached to the armature
                parentBoneName = obj.parent_bone
                if parentBoneName:
                    bone = arm.data.bones[parentBoneName]

                    relTransf = obj.matrix_local
                    connectionPointElement = xmlDoc.createElement("connectionPoint")
                    connectionPointsElement.appendChild(connectionPointElement)
                    connectionPointElement.setAttribute("name", obj.name)
                    connectionPointElement.setAttribute("parentBone", parentBoneName)
                    WriteTransformation(xmlDoc, connectionPointElement, relTransf.to_translation(), relTransf.to_quaternion())
           		
        rootObject = activeObject
        for obj in GetChildConnectionPoints(rootObject): #connection point attached to the mesh
            relTransf = obj.matrix_local
            connectionPointElement = xmlDoc.createElement("connectionPoint")
            connectionPointsElement.appendChild(connectionPointElement)
            WriteTransformation(xmlDoc, connectionPointElement, relTransf.to_translation(), relTransf.to_quaternion())
            connectionPointElement.setAttribute("name", obj.name[3:])
        
        if 0 != len(connectionPointsElement.childNodes):
            modelElement.appendChild(connectionPointsElement)        
        
        
        
      
        #ignore armature modifiers    
        armsOrigRestValues = [arm.pose_position for arm in bpy.data.armatures]
        for arm in bpy.data.armatures:
            arm.pose_position = 'POSE'
        if armsOrigRestValues:
            for o in bpy.data.objects:
                if 'ARMATURE' == o:
                    o.update_tag()
            scene.frame_set(scene.frame_current)
            
        arm = activeObject.find_armature()
    
        
        ##########
        #Armature hierarchy and bind pose
        ##########
        arm = activeObject.find_armature()
        hasArmature = False
        if(arm is not None): #Has armature assigned
            hasArmature = True
    
            #Create bone ids
            bBoneIds = {}
            bBoneIdsByGroup = {}
            boneId = 0
            for boneName in arm.pose.bones.keys():
                #group index
                bBoneIds[boneName] = boneId
                if boneName in activeObject.vertex_groups:
                    groupIndex = activeObject.vertex_groups[boneName].index
                    bBoneIdsByGroup[groupIndex] = boneId
                boneId += 1
            #Bone hierarchy
            bonesElement = xmlDoc.createElement("bones")
            modelElement.appendChild(bonesElement)
    
            bones = {};
            armWorldMat = arm.matrix_world
            objWorldMat = activeObject.matrix_world

            for bBone in arm.data.bones:
                bone = Bone(bBone, bBoneIds, armWorldMat, objWorldMat)
                bones[bBone.name] = bone
                if not bBone.parent:
                    bonesElement.setAttribute("root", str(bBoneIds[bBone.name]))
                bone.WriteBoneHierarchy(xmlDoc, bonesElement)
            
            for bone in bones.values():
                bone.InitParent(bones)

        
        ##########
        #Mesh
        ##########
        meshElement = xmlDoc.createElement("mesh")
        modelElement.appendChild(meshElement)
        meshElement.setAttribute("name", activeObject.name)        

        verticies = VertexCollection(bMesh, bBoneIdsByGroup)
        for i in range(0, len(bMesh.materials)):
            WriteSubmesh(xmlDoc, meshElement, verticies, bMesh, i)
        verticies.WriteToXml(xmlDoc, meshElement)
 
 
        ##########
        #Animations
        ##########
        if hasArmature:
            #anims
            fps = bpy.context.scene.render.fps
            animsElement = xmlDoc.createElement("anims")
            modelElement.appendChild(animsElement)
            for action in bpy.data.actions:
                animElement = xmlDoc.createElement("anim")
                
                #default speed and name
                nameParts = action.name.split('|')
                name = nameParts[0]
                if(1 < len(nameParts)):
                    m = re.search("speed=(?P<speed>[0-9\.]+)", nameParts[1])
                    if m is not None:
                        animElement.setAttribute("defaultSpeed", m.group('speed'))
                        
                animElement.setAttribute("name", name)
                start, end = action.frame_range
                start = int(start)
                end= int(end)
                animElement.setAttribute("duration", "%.6f" % ((end  - start)/float(fps)))
                                         
                animsElement.appendChild(animElement)
                arm.animation_data.action = action
                
                pose = arm.pose
                
                for frame in range(start, end + 1):
                    frameTime = (frame - start)/float(fps)
                    scene.frame_set(frame)
     
                    for bone in bones.values():
                        bone.AddKeyFrame(pose, frame, frameTime)
                
                for bone in bones.values():
                    bone.WriteAnimChannel(xmlDoc, animElement)
                for bone in bones.values():
                    bone.ClearKeyFrames()

    
        ##########
        #Armature - restore
        ##########  
        for i, arm in enumerate(bpy.data.armatures):
            arm.pose_position = armsOrigRestValues[i]

        if armsOrigRestValues:
            for o in bpy.data.objects:
                if 'ARMATURE' == o.type:
                    o.update_tag()
            scene.frame_set(scene.frame_current)        

   
    def invoke(self, context, event):
        wm = context.window_manager
        wm.fileselect_add(self)
        return {'RUNNING_MODAL'}






class VertexCollection:
    __slots__ = ("mVerticies", "mNextVertIndex", "mBMesh", "mBBoneIdsByGroup")

    def __init__(self, bMesh, bBoneIdsByGroup):
        self.mVerticies = OrderedDict()
        self.mNextVertIndex = 0
        self.mBMesh = bMesh
        self.mBBoneIdsByGroup = bBoneIdsByGroup
        
    def GetVertexIndex(self, bFace, vertexIndexInFace):
        newVertex = Vertex(self.mBMesh, self.mBBoneIdsByGroup, bFace, vertexIndexInFace)
        outIndex = self.mVerticies.get(newVertex, -1)
        if -1 == outIndex: #new vertex
            self.mVerticies[newVertex] = self.mNextVertIndex
            outIndex = self.mNextVertIndex
            self.mNextVertIndex+=1
        return  outIndex
    
    def WriteToXml(self, xmlDoc, parentXmlElement):
        verticesElement = xmlDoc.createElement("vertices")
        parentXmlElement.appendChild(verticesElement)

        for vertex in self.mVerticies.keys():
            vertex.WriteToXml(xmlDoc, verticesElement)
           

class Vertex:
    __slots__ = ("mPosition", "mNormal", "mUv", "mBIndex", "mBoneWeights")
    
    #if mBoneIds is None then no skinning data is exported
    def __init__(self, bMesh, bBoneIdsByGroup, bFace, vertexIndexInFace):

        vert = bMesh.vertices[bFace.vertices[vertexIndexInFace]]
        self.mPosition = vert.co  #* gRotMat
        self.mNormal = vert.normal #* gRotMat
        #self.mUv = bFace.index uv[vertexIndexInFace]
        self.mUv = bMesh.uv_textures[0].data[bFace.index].uv[vertexIndexInFace]
        self.mBIndex = vert.index

        self.mBoneWeights = {}
        if bBoneIdsByGroup is not None: #skinning data
            influences = vert.groups
            if 0 == len(influences):
                raise Exception("Vertex '%d' has no bone assigned" % self.mBIndex)
            if 4 < len(influences):
                raise Exception("Vertex '%d' has more then 4 bones assigned" % self.mBIndex)
            for influence in influences:
                self.mBoneWeights[bBoneIdsByGroup[influence.group]] = influence.weight
        
    def __eq__(self, other):
        return ((self.mBIndex == other.mBIndex) and math.fabs(self.mUv[0] - other.mUv[0]) < 0.000001 and math.fabs(self.mUv[1] - other.mUv[1]) < 0.000001)
        
    def __hash__(self):
        return self.mBIndex.__hash__()# ^ self.mUv.__hash__()

    def WriteToXml(self, xmlDoc, parentXmlElement):
        vertexElement = xmlDoc.createElement("vertex")
        parentXmlElement.appendChild(vertexElement)

        #position
        positionElement = xmlDoc.createElement("position")
        vertexElement.appendChild(positionElement)
        positionElement.setAttribute("x", "%.6f" % self.mPosition.x)
        positionElement.setAttribute("y", "%.6f" % self.mPosition.y)
        positionElement.setAttribute("z", "%.6f" % self.mPosition.z)

        #normal
        normalElement = xmlDoc.createElement("normal")
        vertexElement.appendChild(normalElement)
        normalElement.setAttribute("x", "%.6f" % self.mNormal.x)
        normalElement.setAttribute("y", "%.6f" % self.mNormal.y)
        normalElement.setAttribute("z", "%.6f" % self.mNormal.z)
        #uv
        
        uvElement = xmlDoc.createElement("uv")
        vertexElement.appendChild(uvElement)
        uvElement.setAttribute("x", "%.6f" % self.mUv[0])
        uvElement.setAttribute("y", "%.6f" % (1 - self.mUv[1]))
        
        #weights
        for boneId, weight in self.mBoneWeights.items():
            boneWeightElement = xmlDoc.createElement("boneWeight")
            vertexElement.appendChild(boneWeightElement)
            boneWeightElement.setAttribute("bone", str(boneId))
            boneWeightElement.setAttribute("weight", "%.6f" % weight)

class Bone:
    __slots__ = ("mBone", "mId", "mParentId", "mRest", "mAnimKeyFrames", "mParent")
    
    #if mBoneIds is None then no skinning data is exported
    def __init__(self, bone, bBoneIds, armWorldMat, meshWorldMat):
        
        self.mBone = bone
        self.mAnimKeyFrames = {}
        
        self.mId = bBoneIds[bone.name]
        
        if bone.parent is not None:
            self.mParentId = bBoneIds[bone.parent.name]
            self.mRest =  bone.parent.matrix_local.inverted() * bone.matrix_local
        else:
            self.mRest =  bone.matrix_local
            self.mParentId = None
            
        scale = self.mRest.to_scale()
        if (0.00001 < abs(scale.x - 1.0) or 0.00001 < abs(scale.y - 1.0) or 0.00001 < abs(scale.z - 1.0)):
             raise Exception("(Bone " + bone.name + "): Scale transformations are not allowed for skinned meshes" )
            
        
    def GetName(self):
        return self.mBone.name
    
    def AddKeyFrame(self, pose, frame, time):
       
        mat = pose.bones[self.GetName()].matrix.copy()
        
        scale = mat.to_scale()
        if (0.00001 < abs(scale.x - 1.0) or 0.00001 < abs(scale.y - 1.0) or 0.00001 < abs(scale.z - 1.0)):
             raise Exception("(Bone " + self.GetName() + "): Scale transformations are not allowed for skinned meshes" )
        
        self.mAnimKeyFrames[frame] = (time, mat)
    
    def GetKeyFrame(self, frame):
        return self.mAnimKeyFrames[frame][1]
        
    def ClearKeyFrames(self):
        self.mAnimKeyFrames.clear()
    
    def GetAbsoluteTransf(self):
        return self.mAbsoluteTransformation
    
    def InitParent(self, bones):
        if self.mBone.parent is not None:
            self.mParent = bones[self.mBone.parent.name]
        else:
            self.mParent = None
            
    def WriteBoneHierarchy(self, xmlDoc, bonesElement):
        boneElement = xmlDoc.createElement("bone")
        bonesElement.appendChild(boneElement)
        boneElement.setAttribute("index", str(self.mId))
        boneElement.setAttribute("name", self.mBone.name)
        if self.mParentId is not None:
            boneElement.setAttribute("parent", str(self.mParentId))
        
        WriteTransformation(xmlDoc, boneElement, self.mRest.to_translation(), self.mRest.to_quaternion())

    def WriteAnimChannel(self, xmlDoc, animElement):
        channelElement = xmlDoc.createElement("channel")
        channelElement.setAttribute("bone", self.mBone.name)
        scene = bpy.context.scene

        for frame, timePoseMat in sorted(self.mAnimKeyFrames.items()):
            frameTime = timePoseMat[0]
            poseMat = timePoseMat[1]
            keyframeElement = xmlDoc.createElement("keyframe")
            keyframeElement.setAttribute("time", "%.6f" % frameTime)
            channelElement.appendChild(keyframeElement)
            
            if self.mParent is not None:
                mat = self.mParent.GetKeyFrame(frame).inverted() * poseMat
            else:
                mat = poseMat
            
            
            WriteTransformation(xmlDoc, keyframeElement, mat.to_translation(), mat.to_quaternion())
            
            '''
            

            
            frameTime = (frame - startFrame)/float(fps)
            scene.frame_set(frame)
            pose = armature.pose
            
            poseBone = pose.bones[self.mBone.name]
            
            if poseBone is not None:
                keyframeElement = xmlDoc.createElement("keyframe")
                keyframeElement.setAttribute("time", "%.6f" % frameTime)
                channelElement.appendChild(keyframeElement)
                
                if self.mParentId is not None:
                    mat = poseBone.matrix * poseBone.parent.matrix.inverted()
                    #self.mAbsoluteTransformation * Matrix(poseBone.localMatrix) * (self.mParentAbsoluteTransformation * Matrix(poseBone.parent.localMatrix)).invert()
                else:
                    mat = poseBone.matrix

                scale = mat.to_scale()
                if (0.00001 < abs(scale.x - 1.0) or 0.00001 < abs(scale.y - 1.0) or 0.00001 < abs(scale.z - 1.0)):
                    raise Exception("(Bone: " + self.mBone.name + "Action: " + armature.animation_data.action.name + "): Scale transformations are not allowed for skinned meshes - check mesh and bones." )

                WriteTransformation(xmlDoc, keyframeElement, mat.to_translation(), mat.to_quaternion())
        '''
        if channelElement.hasChildNodes():
            animElement.appendChild(channelElement)



# package manages registering

def menu_func(self, context):
    import os
    default_path = os.path.splitext(bpy.data.filepath)[0] + ".alm"
    self.layout.operator("export_mesh.alm", text="ALM (.alm)").filepath = default_path

def register():
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_export.append(menu_func)

def unregister():
    bpy.utils.unregister_module(__name__)
    bpy.types.INFO_MT_file_export.remove(menu_func)
    
if __name__ == "__main__":
    register()
