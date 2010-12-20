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

bl_addon_info = {
    "name": "Ale Mesh",
    "blender": (2, 5, 5),
    "location": "File > Import/Export > ALM",
    "description": "Exports Ale mesh",
    "warning": "",
    "category": "Import/Export"}

import bpy
import sys
from collections import OrderedDict
from bpy.props import *
from xml.dom.minidom import Document
import math

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
    
        if activeObject is None:
            raise Exception("You must select an object that should be exported")
        
        if "MESH" != activeObject.type:
            raise Exception("Selected object must be Mesh type")

        bBoneIds=None
        
        #ignore armature modifiers    
        armsOrigRestValues = [arm.restPosition for arm in bpy.data.armatures]
        for arm in bpy.data.armatures:
            arm.pose_position = 'REST'
            
        bMesh = activeObject.create_mesh(bpy.context.scene, True, "PREVIEW")
        
        
        
        ##########
        #Connection points
        ##########
        connectionPointsElement = xmlDoc.createElement("connectionPoints")
    
        '''
        if hasArmature:
            rootObject = activeObjectParent #armature
            for obj in GetChildConnectionPoints(rootObject): #connection point attached to the armature
                parentBoneName = obj.getParentBoneName()
                if parentBoneName:
                    bone = GetBone(bones, parentBoneName)
                    #relTransf = obj.matrixWorld * bone.GetAbsoluteTransf().copy().invert()
                    relTransf = obj.matrixLocal
                    connectionPointElement = xmlDoc.createElement("connectionPoint")
                    connectionPointsElement.appendChild(connectionPointElement)
                    connectionPointElement.setAttribute("name", obj.name)
                    connectionPointElement.setAttribute("parentBone", bone.GetName())
                    WriteTransformation(xmlDoc, connectionPointElement, relTransf.translationPart(), relTransf.toQuat())
        '''   
        rootObject = activeObject
        for obj in self.GetChildConnectionPoints(rootObject): #connection point attached to the mesh
            relTransf = obj.matrix_local
            connectionPointElement = xmlDoc.createElement("connectionPoint")
            connectionPointsElement.appendChild(connectionPointElement)
            self.WriteTransformation(xmlDoc, connectionPointElement, relTransf.translation_part(), relTransf.to_quat())
            connectionPointElement.setAttribute("name", obj.name[3:])
        
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
            self.WriteSubmesh(xmlDoc, meshElement, verticies, bMesh, i)
        verticies.WriteToXml(xmlDoc, meshElement)

    def WriteTransformation(self, xmlDoc, parentElement, translation, orientation):
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


    def WriteSubmesh(self, xmlDoc, meshElement, verticies, mesh, materialIndex):
        submeshElement = xmlDoc.createElement("submesh")
        meshElement.appendChild(submeshElement)
    
        submeshElement.setAttribute("material", mesh.materials[materialIndex].name)
        
        #write faces that belongs to the this submesh (by material)
        for bFace in mesh.faces:
            if bFace.material_index == materialIndex:
                self.WriteFaceToXml(xmlDoc, submeshElement, verticies, bFace)

    def WriteTriangleFaceToXml(self, xmlDoc, submeshElement, v0Index, v1Index, v2Index):
        faceElement = xmlDoc.createElement("face")
        submeshElement.appendChild(faceElement)
        faceElement.setAttribute("v0", str(v0Index))
        faceElement.setAttribute("v1", str(v1Index))
        faceElement.setAttribute("v2", str(v2Index))

    def WriteFaceToXml(self, xmlDoc, submeshElement, verticies, bFace):
        if 4 < len(bFace.vertices):
            raise Exception("Only faces with three or four vertices are supported")
    
        bVertices = verticies.mBMesh.vertices
    
        vIndexes = []
        for i in range(0, len(bFace.vertices)):
            vIndexes.append(verticies.GetVertexIndex(bFace, i))
        
        if 3 == len(bFace.vertices): #triangle
            self.WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
        else: # quad => split it to triangles
            if (bVertices[bFace.vertices[2]].co - bVertices[bFace.vertices[0]].co).length < (bVertices[bFace.vertices[3]].co - bVertices[bFace.vertices[1]].co).length: #0-2
                self.WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[2], vIndexes[1], vIndexes[0])
                self.WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[0])
            else: #1-3
                self.WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[1], vIndexes[0])
                self.WriteTriangleFaceToXml(xmlDoc, submeshElement, vIndexes[3], vIndexes[2], vIndexes[1])
            
            
    def GetObjChildrens(self, obj):
        return filter(lambda o : o.parent == obj, bpy.context.scene.objects)
             
    def IsConnectionPointObject(self, obj):
        return "cp_" == obj.name[0:3]
    
    def GetChildConnectionPoints(self, obj):
        return filter(lambda o : self.IsConnectionPointObject(o), self.GetObjChildrens(obj))
    
    def GetBone(self, boneList, boneName):
        for b in boneList:
            if b.GetName() == boneName:
                return b
        return None   

    def invoke(self, context, event):
        wm = context.window_manager
        wm.add_fileselect(self)
        return {'RUNNING_MODAL'}






class VertexCollection:
    __slots__ = ("mVerticies", "mNextVertIndex", "mBMesh", "mBBoneIds")

    def __init__(self, bMesh, bBoneIds):
        self.mVerticies = OrderedDict()
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

        for vertex in self.mVerticies.keys():
            vertex.WriteToXml(xmlDoc, verticesElement)
           

class Vertex:
    __slots__ = ("mPosition", "mNormal", "mUv", "mBIndex", "mBoneWeights")
    
    #if mBoneIds is None then no skinning data is exported
    def __init__(self, bMesh, bBoneIds, bFace, vertexIndexInFace):

        vert = bMesh
        self.mPosition = bMesh.vertices[bFace.vertices[vertexIndexInFace]].co  #* gRotMat
        self.mNormal = bMesh.vertices[bFace.vertices[vertexIndexInFace]].normal #* gRotMat
        #self.mUv = bFace.index uv[vertexIndexInFace]
        self.mUv = bMesh.uv_textures[0].data[bFace.index].uv[vertexIndexInFace]
        self.mBIndex = bMesh.vertices[bFace.vertices[vertexIndexInFace]].index

        '''self.mBoneWeights = {}
        if bBoneIds is not None: #skinning data
            influences = bMesh.getVertexInfluences(self.mBIndex)
            if 0 == len(influences):
                raise Exception("Vertex '%d' has no bone assigned" % self.mBIndex)
            if 4 < len(influences):
                raise Exception("Vertex '%d' has more then 4 bones assigned" % self.mBIndex)
            for bBoneName, weight in bMesh.getVertexInfluences(self.mBIndex):
                self.mBoneWeights[bBoneIds[bBoneName]] = weight'''
        
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
        '''
        #weights
        for boneId, weight in self.mBoneWeights.iteritems():
            boneWeightElement = xmlDoc.createElement("boneWeight")
            vertexElement.appendChild(boneWeightElement)
            boneWeightElement.setAttribute("bone", str(boneId))
            boneWeightElement.setAttribute("weight", "%.6f" % weight)'''




# package manages registering

def menu_export(self, context):
    import os
    default_path = os.path.splitext(bpy.data.filepath)[0] + ".alm"
    self.layout.operator("export_mesh.alm", text="ALM (.alm)").filepath = default_path


def register():
    bpy.types.INFO_MT_file_export.append(menu_export)

def unregister():
    bpy.types.INFO_MT_file_export.remove(menu_export)

if __name__ == "__main__":
    register()
