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
Name: 'ALE model (.alm)'
Blender: 244
Group: 'Export'
Tooltip: 'Exports the model for using it with ALE'
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
from Blender.Mathutils import Matrix,Quaternion


#gRotMat = Mathutils.RotationMatrix(-90, 4 ,"x")


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


def Write(filename):
    xmlDoc = Document()
    activeScene= bpy.data.scenes.active
    
    modelElement = xmlDoc.createElement("model")
    xmlDoc.appendChild(modelElement)
    
    #write active object as a mesh
    WriteSelectedMesh(xmlDoc, modelElement)
    
    #write xmlDoc to file
    outFile = file(filename, "w")
    xmlDoc.writexml(outFile, addindent="    ", newl="\n")
    outFile.close()

def WriteSelectedMesh(xmlDoc, modelElement):
    activeObject = bpy.data.scenes.active.objects.active

    if activeObject is None:
        raise Exception("You must select an object that should be exported")
    
    if "Mesh" != activeObject.type:
        raise Exception("Selected object must be Mesh type")

    bBoneIds=None

    #ignore armature modifiers    
    armsOrigRestValues = [arm.restPosition for arm in bpy.data.armatures]
    for arm in bpy.data.armatures:
        arm.restPosition = True
        
        
    activeObject.makeDisplayList()
    Blender.Set('curframe', Blender.Get('curframe'))        
    Window.RedrawAll()
    origMesh = activeObject.getData(mesh=1)
    bMesh = Mesh.New()
    bMesh.getFromObject(activeObject)
    bMesh.materials = origMesh.materials
    activeObject.copy().link(bMesh)
        
    ##########
    #Armature hierarchy and bind pose
    ##########
    activeObjectParent = activeObject.getParent()
    hasArmature = False
    if(activeObjectParent and "Armature" == activeObjectParent.getType()): #Has armature assigned
        hasArmature = True
        armObj = activeObjectParent
        arm = armObj.getData()

        #Create bone ids
        bBoneIds = {}
        boneId = 0
        for boneName in arm.bones.keys():
            bBoneIds[boneName] = boneId
            boneId += 1
        #Bone hierarchy
        bonesElement = xmlDoc.createElement("bones")
        modelElement.appendChild(bonesElement)

        bones = [];
        armWorldMat = armObj.matrixWorld
        objWorldMat = activeObject.matrixWorld
        for bBone in arm.bones.values():
            bone = Bone(bBone, bBoneIds, armWorldMat, objWorldMat)
            bones.append(bone)
            if not bBone.parent:
                bonesElement.setAttribute("root", str(bBoneIds[bBone.name]))
            bone.WriteBoneHierarchy(xmlDoc, bonesElement)


    ##########
    #Connection points
    ##########
    connectionPointsElement = xmlDoc.createElement("connectionPoints")

    if hasArmature:
        rootObject = activeObjectParent #armature
        for obj in GetChildConnectionPoints(rootObject): #connection point attached to the armature
            parentBoneName = obj.getParentBoneName()
            if parentBoneName:
                bone = GetBone(bones, parentBoneName)
#                relTransf = obj.matrixWorld * bone.GetAbsoluteTransf().copy().invert()
                relTransf = obj.matrixLocal
                connectionPointElement = xmlDoc.createElement("connectionPoint")
                connectionPointsElement.appendChild(connectionPointElement)
                connectionPointElement.setAttribute("name", obj.name)
                connectionPointElement.setAttribute("parentBone", bone.GetName())
                WriteTransformation(xmlDoc, connectionPointElement, relTransf.translationPart(), relTransf.toQuat())

    rootObject = activeObject
    for obj in GetChildConnectionPoints(rootObject): #connection point attached to the mesh
        relTransf = obj.matrixLocal
        connectionPointElement = xmlDoc.createElement("connectionPoint")
        connectionPointsElement.appendChild(connectionPointElement)
        WriteTransformation(xmlDoc, connectionPointElement, relTransf.translationPart(), relTransf.toQuat())
        connectionPointElement.setAttribute("name", obj.name)
    
    if 0 != len(connectionPointsElement.childNodes):
        modelElement.appendChild(connectionPointsElement)

    ##########
    #Mesh
    ##########
    #create copy of the object and the mesh with all modificators applied
    meshElement = xmlDoc.createElement("mesh")
    modelElement.appendChild(meshElement)
    meshElement.setAttribute("name", activeObject.name)

    verticies = VertexCollection(bMesh, bBoneIds)
    for i in range(0, len(bMesh.materials)):
        WriteSubmesh(xmlDoc, meshElement, verticies, bMesh, i)
    verticies.WriteToXml(xmlDoc, meshElement)

    #restore arm orig armature restPosition
    for i, arm in enumerate(bpy.data.armatures):
        arm.restPosition = armsOrigRestValues[i]
    if armsOrigRestValues:
        for ob in bpy.data.objects:
        	if 'Armature' == ob.type:
        		ob.makeDisplayList()
        Blender.Set('curframe', Blender.Get('curframe'))
        Window.RedrawAll()

    ##########
    #Animations
    ##########
    if bBoneIds is not None: #Has armature assigned
        #anims
        fps = bpy.data.scenes.active.getRenderingContext().framesPerSec()
        animsElement = xmlDoc.createElement("anims")
        modelElement.appendChild(animsElement)
        for action in bpy.data.actions:
            animElement = xmlDoc.createElement("anim")
            animElement.setAttribute("name", action.name)
            animElement.setAttribute("duration", str((action.getFrameNumbers()[-1]  - action.getFrameNumbers()[0])/float(fps)))
                                     
            animsElement.appendChild(animElement)
            action.setActive(armObj)
            for bone in bones:
                bone.WriteAnimChannel(xmlDoc, animElement, armObj, fps, action.getFrameNumbers()[0], action.getFrameNumbers()[-1])



       
        
def WriteSubmesh(xmlDoc, meshElement, verticies, mesh, materialIndex):
    submeshElement = xmlDoc.createElement("submesh")
    meshElement.appendChild(submeshElement)

    submeshElement.setAttribute("material", mesh.materials[materialIndex].name)
    
    #write faces that belongs to the this submesh (by material)
    for bFace in mesh.faces:
        if bFace.mat == materialIndex:
            WriteFaceToXml(xmlDoc, submeshElement, verticies, bFace)


def WriteTriangleFaceToXml(xmlDoc, submeshElement, v0Index, v1Index, v2Index):
    faceElement = xmlDoc.createElement("face")
    submeshElement.appendChild(faceElement)
    faceElement.setAttribute("v0", str(v0Index))
    faceElement.setAttribute("v1", str(v1Index))
    faceElement.setAttribute("v2", str(v2Index))

#Loads vertices of the face to the verticies and writes the face to the xml
def WriteFaceToXml(xmlDoc, submeshElement, verticies, bFace):
    if 4 < len(bFace.verts):
        raise Exception("Only faces with three or four vertices are supported")

    vIndexes = []
    for i in range(0, len(bFace.verts)):
        vIndexes.append(verticies.GetVertexIndex(bFace, i))
    
    if 3 == len(bFace.verts): #triangle
        WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
    else: # quad => split it to triangles
        if (bFace.verts[2].co - bFace.verts[0].co).length < (bFace.verts[3].co - bFace.verts[1].co).length: #0-2
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[0])
        else: #1-3
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[1], vIndexes[0])
            WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[1])

def GetObjChildrens(obj):
    return filter(lambda o : o.getParent() == obj, bpy.data.scenes.active.objects)
         
def IsConnectionPointObject(obj):
    return 0 != len(filter(lambda p: p.getName() == "IsConnectionPoint", obj.getAllProperties()))

def GetChildConnectionPoints(obj):
    return filter(lambda o : IsConnectionPointObject(o), GetObjChildrens(obj))

def GetBone(boneList, boneName):
    for b in boneList:
        if b.GetName() == boneName:
            return b
    return None   

class VertexCollection:
    __slots__ = ("mVerticies", "mNextVertIndex", "mBMesh", "mBBoneIds")

    def __init__(self, bMesh, bBoneIds):
        self.mVerticies = {}
        self.mNextVertIndex = 0
        self.mBMesh = bMesh
        self.mBBoneIds = bBoneIds
        
    def GetVertexIndex(self, bFace, vertexIndexInFace):
        newVertex = Vertex(self.mBMesh, self.mBBoneIds, bFace, vertexIndexInFace)
        outIndex = self.mVerticies.get(newVertex, -1)
        if -1 == outIndex: #new vertex
            self.mVerticies[newVertex] = self.mNextVertIndex
            outIndex = self.mNextVertIndex
            self.mNextVertIndex+=1
        return  outIndex
    
    def WriteToXml(self, xmlDoc, parentXmlElement):
        verticesElement = xmlDoc.createElement("vertices")
        parentXmlElement.appendChild(verticesElement)

        for vertex in self.Sort(self.mVerticies):
            vertex[0].WriteToXml(xmlDoc, verticesElement)
            
    #based on http://writeonly.wordpress.com/2008/08/30/sorting-dictionaries-by-value-in-python-improved/
    def Sort(self, d):  
        return sorted(d.iteritems(), key=itemgetter(1), reverse=False)

class Vertex:
    __slots__ = ("mPosition", "mNormal", "mUv", "mBIndex", "mBoneWeights")
    
    #if mBoneIds is None then no skinning data is exported
    def __init__(self, bMesh, bBoneIds, bFace, vertexIndexInFace):

        self.mPosition = bFace.verts[vertexIndexInFace].co  #* gRotMat
        self.mNormal = bFace.verts[vertexIndexInFace].no #* gRotMat
        self.mUv = bFace.uv[vertexIndexInFace]
        self.mBIndex = bFace.verts[vertexIndexInFace].index

        self.mBoneWeights = {}
        if bBoneIds is not None: #skinning data
            influences = bMesh.getVertexInfluences(self.mBIndex)
            if 0 == len(influences):
                raise Exception("Vertex '%d' has no bone assigned" % self.mBIndex)
            if 4 < len(influences):
                raise Exception("Vertex '%d' has more then 4 bones assigned" % self.mBIndex)
            for bBoneName, weight in bMesh.getVertexInfluences(self.mBIndex):
                self.mBoneWeights[bBoneIds[bBoneName]] = weight
        
    def __eq__(self, other):
        return ((self.mBIndex == other.mBIndex) and math.fabs(self.mUv.x - other.mUv.x) < 0.000001 and math.fabs(self.mUv.y - other.mUv.y) < 0.000001)
        
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
        uvElement.setAttribute("x", "%.6f" % self.mUv.x)
        uvElement.setAttribute("y", "%.6f" % (1 - self.mUv.y))
        #weights
        for boneId, weight in self.mBoneWeights.iteritems():
            boneWeightElement = xmlDoc.createElement("boneWeight")
            vertexElement.appendChild(boneWeightElement)
            boneWeightElement.setAttribute("bone", str(boneId))
            boneWeightElement.setAttribute("weight", "%.6f" % weight)
            
class Bone:
    __slots__ = ("mBone", "mTransformation", "mId", "mParentId", "mParentAbsoluteTransformation", "mAbsoluteTransformation")
    
    #if mBoneIds is None then no skinning data is exported
    def __init__(self, bone, bBoneIds, armWorldMat, meshWorldMat):
        
        self.mBone = bone
        
        self.mAbsoluteTransformation = Matrix(bone.matrix["ARMATURESPACE"]) * armWorldMat * meshWorldMat.copy().invert()#* rotMat


        self.mId = bBoneIds[bone.name]
        if bone.parent is not None:
            if not isinstance(self.mBone.parent, Armature.Bone):
                raise Exception("Parent of the bone '%s' is not Armature.Bone")
            self.mParentId = bBoneIds[bone.parent.name]

            self.mParentAbsoluteTransformation = Matrix(bone.parent.matrix["ARMATURESPACE"]) *armWorldMat * meshWorldMat.copy().invert()
            self.mTransformation = self.mAbsoluteTransformation * self.mParentAbsoluteTransformation.copy().invert()

        else:
            self.mTransformation = Matrix(bone.matrix["ARMATURESPACE"]) * armWorldMat * meshWorldMat.copy().invert()
            self.mParentId = None
            self.mParentAbsoluteTransformation = None
            
        scale = self.mTransformation.scalePart()
        if (0.00001 < abs(scale.x - 1.0) or 0.00001 < abs(scale.y - 1.0) or 0.00001 < abs(scale.z - 1.0)):
             raise Exception("(Bone " + bone.name + "): Scale transformations are not allowed for skinned meshes - check mesh and bones." )

    def GetName(self):
        return self.mBone.name
    
    def GetAbsoluteTransf(self):
        return self.mAbsoluteTransformation
    
    def WriteBoneHierarchy(self, xmlDoc, bonesElement):
        boneElement = xmlDoc.createElement("bone")
        bonesElement.appendChild(boneElement)
        boneElement.setAttribute("index", str(self.mId))
        boneElement.setAttribute("name", self.mBone.name)
        if self.mParentId is not None:
            boneElement.setAttribute("parent", str(self.mParentId))
    
        dir(self.mTransformation)
        WriteTransformation(xmlDoc, boneElement, self.mTransformation.translationPart(), self.mTransformation.toQuat())

    def WriteAnimChannel(self, xmlDoc, animElement, armature, fps, startFrame, endFrame):

        channelElement = xmlDoc.createElement("channel")
        channelElement.setAttribute("bone", self.mBone.name)
        

        for frame in range(startFrame, endFrame + 1):
            frameTime = (frame - startFrame)/float(fps)
            armature.evaluatePose(frame)
            pose = armature.getPose()
            
            poseBone = pose.bones[self.mBone.name]
            
            if poseBone is not None:
                keyframeElement = xmlDoc.createElement("keyframe")
                keyframeElement.setAttribute("time", "%.6f" % frameTime)
                channelElement.appendChild(keyframeElement)
                
                if self.mParentId is not None:
                    mat = poseBone.poseMatrix * poseBone.parent.poseMatrix.copy().invert()
                    #self.mAbsoluteTransformation * Matrix(poseBone.localMatrix) * (self.mParentAbsoluteTransformation * Matrix(poseBone.parent.localMatrix)).invert()
                else:
                    mat = poseBone.poseMatrix

                scale = mat.scalePart()
                if (0.00001 < abs(scale.x - 1.0) or 0.00001 < abs(scale.y - 1.0) or 0.00001 < abs(scale.z - 1.0)):
                    raise Exception("(Bone: " + self.mBone.name + "Action: " + armature.getAction().name + "): Scale transformations are not allowed for skinned meshes - check mesh and bones." )

                WriteTransformation(xmlDoc, keyframeElement, mat.translationPart(), mat.toQuat())

        if channelElement.hasChildNodes():
            animElement.appendChild(channelElement)
        
#-----START-----

Blender.Window.FileSelector(Write, 'Export ALE model', Blender.sys.makename(ext='.alm'))
