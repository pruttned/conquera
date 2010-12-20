//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Text;
//using Ale.Graphics;
//using SimpleOrmFramework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework;
//using Ale.Input;
//using Ale.Tools;
//using Microsoft.Xna.Framework.Graphics;
//using Ale.Content;
//using System.Runtime.InteropServices;
//using Microsoft.Xna.Framework.Graphics.PackedVector;
//using System.Collections.ObjectModel;
//using Ale.Settings;

//namespace Ale.Scene
//{
//    public class TerrainScene : OctreeScene
//    {
//        private TerrainPalette mPalette;
//        private StaticGeometry mTerrainStaticGeomtery;
//        private byte[,] mTitles;
//        GraphicModel mUjo;
//        private Vector3 mLightDir = new Vector3(-0.3333333f, 0.1f, 1.4f);
//        private Plane mGroundPlane = new Plane(Vector3.UnitZ, -2);
//        private GraphicsDeviceManager mGraphicsDeviceManager; //aux


//       // Grass mGrass;



//        public TerrainScene(SceneManager sceneManager, BoundingBox sceneBounds, TerrainPalette palette, ContentGroup content, int size, GraphicsDeviceManager graphicsDeviceManager)
//            :base(sceneManager, content, sceneBounds)
//        {

//            sceneManager.KeyboardManager.KeyUp += new KeyboardManager.KeyEventHandler(KeyboardManager_KeyUp);

//            mTerrainStaticGeomtery = new StaticGeometry(SceneManager.GraphicsDevice, Octree, MainCamera, 20);
//            RegisterFrameListener(mTerrainStaticGeomtery);
//            mGraphicsDeviceManager = graphicsDeviceManager;

//            //aux init
//            mTitles = new byte[size, size];
//            for (int i = 0; i < size; ++i)
//            {
//                for (int j = 0; j < size; ++j)
//                {
//                    if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
//                    {
//                        mTitles[i, j] = 0;
//                    }
//                    else
//                    {
//                        mTitles[i, j] = 1;
//                    }
//                }
//            }


//            for (int i = 0; i < 100; ++i)
//            {
//                Point pos = new Point(AleMathUtils.Random.Next(0, size), AleMathUtils.Random.Next(0, size));
//                for (int j = 0; j < 300; ++j)
//                {
//                    switch (AleMathUtils.Random.Next(4))
//                    {
//                        case 0:
//                            pos.X++;
//                            if (size <= pos.X)
//                            {
//                                pos.X = 0;
//                            }
//                            break;
//                        case 1:
//                            pos.X--;
//                            if (0 > pos.X)
//                            {
//                                pos.X = size - 1;
//                            }
//                            break;
//                        case 2:
//                            pos.Y++;
//                            if (size <= pos.Y)
//                            {
//                                pos.Y = 0;
//                            }
//                            break;
//                        case 3:
//                            pos.Y--;
//                            if (0 > pos.Y)
//                            {
//                                pos.Y = size - 1;
//                            }
//                            break;
//                    }
//                    mTitles[pos.X, pos.Y] = 0;
//                }

//            }


//            //aux
//            Material surfaceMat = content.Load<Material>("SurfaceMat");
//            //surfaceMat.DefaultTechnique.Passes[0].SetParam("gLightViewProj", mShadowCamera.ViewProjectionTransformation);

//            Material treeMatLeaves = new Material(content.Load<MaterialEffect>("Simple"), DefaultRenderLayers.GroundStandingObjects);
//            treeMatLeaves.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("leaves"));
//            treeMatLeaves.Techniques["ShadowPass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("leaves"));

//            Material treeMatWood = new Material(content.Load<MaterialEffect>("Simple"), DefaultRenderLayers.GroundStandingObjects);
//            treeMatWood.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("wood"));
//            treeMatWood.Techniques["ShadowPass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("wood"));

//            Material treeMat2 = new Material(content.Load<MaterialEffect>("Simple"), DefaultRenderLayers.GroundStandingObjects);
//            treeMat2.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("treeText"));
//            treeMat2.Techniques["ShadowPass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("treeText"));


//            Material ujoMat = content.Load<Material>("UjoMaterial");

//            mUjo = new GraphicModel(content.Load<Mesh>("ujo"), ujoMat);
//            mUjo.WorldScale = new Vector3(0.3f);
//            mUjo.WorldPosition = new Vector3(0, 0, 2.9f);
//            mUjo.AnimationPlayer.Animation = "Run";
//            mUjo.AnimationPlayer.Play(true);
//            Octree.AddObject(mUjo);

//            Dictionary<string, Material> treeMaterials = new Dictionary<string, Material>
//            {
                
//                {"arbol.otono", treeMat2},
//                //{"KORA2", treeMatWood},
//                //{"KRONA1", treeMatLeaves},
//                //{"KRONA1T", treeMatLeaves}
//            };


//            Mesh treeMesh = content.Load<Mesh>("tree");
//            GraphicModel tree = new GraphicModel(treeMesh, treeMaterials);
//            for (int i = 0; i < size * size / 30; ++i)
//            {
//                do
//                {
//                    tree.WorldPosition = new Vector3(
//                        (float)AleMathUtils.Random.NextDouble() * size,
//                        (float)AleMathUtils.Random.NextDouble() * size,
//                        2);
//                    tree.WorldScale = new Vector3(0.2f);

//                    tree.WorldOrientation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), (float)AleMathUtils.Random.NextDouble() * 3.14f);

//                } while ((tree.WorldPosition.X >= size - 1 || tree.WorldPosition.Y >= size - 1) ||
//                    1 != mTitles[(int)tree.WorldPosition.X, (int)tree.WorldPosition.Y] ||
//                    1 != mTitles[(int)tree.WorldPosition.X + 1, (int)tree.WorldPosition.Y] ||
//                    1 != mTitles[(int)tree.WorldPosition.X, (int)tree.WorldPosition.Y + 1] ||
//                    1 != mTitles[(int)tree.WorldPosition.X + 1, (int)tree.WorldPosition.Y + 1]);
//                //Octree.AddObject(tree);
//                mTerrainStaticGeomtery.AddGraphicModel(tree);
//            }
//            for (int i = 0; i < size * size / 50; ++i)
//            {
//                GraphicModel ujo = new GraphicModel(content.Load<Mesh>("ujo"), ujoMat);
//                ujo.WorldScale = new Vector3(0.3f);
//                ujo.WorldPosition = new Vector3(
//                    (float)AleMathUtils.Random.NextDouble() * size,
//                    (float)AleMathUtils.Random.NextDouble() * size,
//                    2.9f);
//                ujo.AnimationPlayer.Animation = "Run";
//                ujo.AnimationPlayer.Play(true);

//                Octree.AddObject(ujo);

//            }


//            mPalette = palette;


//            MeshBuilder meshBuilder = new MeshBuilder(mGraphicsDeviceManager.GraphicsDevice);

//            int textureCellCnt = 6;
//            float textureCellSpacing = 0.02f;
//            float baseCellSize = 1.0f / (float)textureCellCnt;
//            textureCellSpacing = textureCellSpacing * baseCellSize;
//            float textureCellSize = baseCellSize - textureCellSpacing;

//            Point textureCellIndex = new Point(1,0);
//            {
//                meshBuilder.SetCurrentSubMesh(DefaultRenderLayers.GroundStandingObjects, "mat1");
//                SimpleVertex vert = new SimpleVertex(new Vector3(0, 0, 5), Vector3.UnitZ, Vector2.Zero);

//                 vert.Uv = new Vector2(
//                        textureCellSize*(vert.Position.X/2.0f+0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                        (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                        );

//                int vcI = meshBuilder.AddVertex(ref vert);
//                vert.Position = new Vector3(0, 1, 5);

//                 vert.Uv = new Vector2(
//                        textureCellSize*(vert.Position.X/2.0f+0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                        (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                        );
                
//                int vOldI = meshBuilder.AddVertex(ref vert);
//                int vFirstI = vOldI;
//                Vector3 baseVec = new Vector3(0, 1, 5);
//                for (int i = 1; i < 6; ++i)
//                {
//                    Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(i * 60));
//                    Vector3.Transform(ref baseVec, ref rotQuat, out vert.Position);

//                    //vert.Uv = new Vector2(vert.Position.X/2.0f+0.5f, vert.Position.Y/2.0f+0.5f);
//                    vert.Uv = new Vector2(
//                        textureCellSize*(vert.Position.X/2.0f+0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                        (textureCellSize * (1-(vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                    );
                    
//                    int vI = meshBuilder.AddVertex(ref vert);

//                    meshBuilder.AddFace(vcI, vI, vOldI);

//                    vOldI = vI;
//                }

//                meshBuilder.AddFace(vcI, vFirstI, vOldI);
//            }
//            Mesh buildedMesh = meshBuilder.BuildMesh();

//            textureCellIndex = new Point(2, 0);
//            {
//                meshBuilder.SetCurrentSubMesh(DefaultRenderLayers.GroundStandingObjects, "mat1");
//                SimpleVertex vert = new SimpleVertex(new Vector3(0, 0, 5), Vector3.UnitZ, Vector2.Zero);

//                vert.Uv = new Vector2(
//                       textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                       (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                       );

//                int vcI = meshBuilder.AddVertex(ref vert);
//                vert.Position = new Vector3(0, 1, 5);

//                vert.Uv = new Vector2(
//                       textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                       (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                       );

//                int vOldI = meshBuilder.AddVertex(ref vert);
//                int vFirstI = vOldI;
//                Vector3 baseVec = new Vector3(0, 1, 5);
//                for (int i = 1; i < 6; ++i)
//                {
//                    Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(i * 60));
//                    Vector3.Transform(ref baseVec, ref rotQuat, out vert.Position);

//                    //vert.Uv = new Vector2(vert.Position.X/2.0f+0.5f, vert.Position.Y/2.0f+0.5f);
//                    vert.Uv = new Vector2(
//                        textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
//                        (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
//                    );

//                    int vI = meshBuilder.AddVertex(ref vert);

//                    meshBuilder.AddFace(vcI, vI, vOldI);

//                    vOldI = vI;
//                }

//                meshBuilder.AddFace(vcI, vFirstI, vOldI);
//            }
//            Mesh buildedMesh2 = meshBuilder.BuildMesh();


//            Material grassMaterial = new Material(content.Load<MaterialEffect>("Simple"), DefaultRenderLayers.GroundStandingObjects);
//            grassMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("grass"));
//            grassMaterial.Techniques["ShadowPass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("grass"));
//            grassMaterial.Techniques["WaterReflectionPass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("grass"));

//            {
//                meshBuilder.SetCurrentSubMesh(DefaultRenderLayers.GroundStandingObjects, "mat1");

 
//                for (int i = 0; i < 40; ++i)
//                {


//                    Vector3 cp = Vector3.Zero;
//                    Vector3 pos = AleMathUtils.GetRandomVector3(ref cp, 1.3f);
//                    pos.Z = 0;

//                    float scale = 0.5f*(0.02f + (1.5f-Vector3.DistanceSquared(pos, Vector3.Zero)));
//                    SimpleVertex vert1 = new SimpleVertex(new Vector3(- 0.5f * scale, 0, 5), Vector3.UnitZ, new Vector2(1, 1));
//                    SimpleVertex vert2 = new SimpleVertex(new Vector3(-0.5f * scale, 0, 5+ scale), Vector3.UnitZ, new Vector2(1, 0));
//                    SimpleVertex vert3 = new SimpleVertex(new Vector3(0.5f * scale, 0, 5), Vector3.UnitZ, new Vector2(0, 1));
//                    SimpleVertex vert4 = new SimpleVertex(new Vector3(0.5f * scale, 0, 5 + scale), Vector3.UnitZ, new Vector2(0, 0));

//                    Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)AleMathUtils.Random.NextDouble() * 3.14f);
//                    Vector3.Transform(ref vert1.Position, ref rotQuat, out vert1.Position);
//                    Vector3.Transform(ref vert2.Position, ref rotQuat, out vert2.Position);
//                    Vector3.Transform(ref vert3.Position, ref rotQuat, out vert3.Position);
//                    Vector3.Transform(ref vert4.Position, ref rotQuat, out vert4.Position);


          

//                    vert1.Position += pos;
//                    vert2.Position += pos;
//                    vert3.Position += pos;
//                    vert4.Position += pos;

//                    int vcI1 = meshBuilder.AddVertex(ref vert1);
//                    int vcI2 = meshBuilder.AddVertex(ref vert2);
//                    int vcI3 = meshBuilder.AddVertex(ref vert3);
//                    int vcI4 = meshBuilder.AddVertex(ref vert4);
//                    meshBuilder.AddFace(vcI1, vcI2, vcI3);
//                    meshBuilder.AddFace(vcI2, vcI4, vcI3);
//                    meshBuilder.AddFace(vcI3, vcI2, vcI1);
//                    meshBuilder.AddFace(vcI3, vcI4, vcI2);
//                }
//            }
//            Mesh grassMesh = meshBuilder.BuildMesh();

//            //{
//            //    meshBuilder.SetCurrentSubMesh(DefaultRenderLayers.GroundStandingObjects, "mat2");
//            //    SimpleVertex vert = new SimpleVertex(new Vector3(0, 0, 4), Vector3.UnitZ, Vector2.Zero);
//            //    vert.Uv = new Vector2(vert.Position.X / 2.0f + 0.5f, vert.Position.Y / 2.0f + 0.5f);
//            //    int vcI = meshBuilder.AddVertex(ref vert);
//            //    vert.Position = new Vector3(1, 0, 4);
//            //    vert.Uv = new Vector2(vert.Position.X / 2.0f + 0.5f, vert.Position.Y / 2.0f + 0.5f);
//            //    int vOldI = meshBuilder.AddVertex(ref vert);
//            //    int vFirstI = vOldI;
//            //    Vector3 baseVec = new Vector3(1, 0, 4);
//            //    for (int i = 1; i < 6; ++i)
//            //    {
//            //        Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(i * 60));
//            //        Vector3.Transform(ref baseVec, ref rotQuat, out vert.Position);
//            //        vert.Uv = new Vector2(vert.Position.X / 2.0f + 0.5f, vert.Position.Y / 2.0f + 0.5f);
//            //        int vI = meshBuilder.AddVertex(ref vert);

//            //        meshBuilder.AddFace(vcI, vI, vOldI);

//            //        vOldI = vI;
//            //    }
//            //    meshBuilder.AddFace(vcI, vFirstI, vOldI);
//            //}


//            Material cellSurfaceMaterial = new Material(content.Load<MaterialEffect>("surfaceFx"), DefaultRenderLayers.GroundStandingObjects);
//            cellSurfaceMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("HexCellUv"));

//            for (int i = 0; i < 20; ++i)
//            {
//                for (int j = 0; j < 20; ++j)
//                {
//                    bool isDirt = (i == 5 && j == 5) || (i == 5 && j == 6) || (i == 5 && j == 7) || (i == 6 && j == 6) || (i == 7 && j == 6) || (i > 6 && j > 6);
//                    GraphicModel bm = new GraphicModel((isDirt ? buildedMesh2 : buildedMesh), new Dictionary<string, Material> 
//                    {
//                        {"mat1",cellSurfaceMaterial },
//                        {"mat2", content.Load<Material>("WallMat")}
//                    });
//                    //http://www.gamedev.net/reference/articles/article747.asp
//                    //PlotX=MapX*Width+(MapY AND 1)*(Width/2)
//                    //HeightOverLapping=)*0.75
//                    bm.WorldPosition = new Vector3(
//                        i * 1.73202f + (j & 1) * 0.86601f,
//                        j * 1.5f, 0);
//                    Octree.AddObject(bm);

//                    if (!isDirt && AleMathUtils.Random.NextDouble() < 0.5)
//                    {
//                        GraphicModel grass = new GraphicModel(grassMesh, grassMaterial);
//                        grass.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)AleMathUtils.Random.NextDouble() * 3.14f);
//                        grass.WorldPosition = new Vector3(
//                           i * 1.73202f + (j & 1) * 0.86601f,
//                           j * 1.5f, 0);
//                        Octree.AddObject(grass);
//                    }

//                    if (!isDirt && AleMathUtils.Random.NextDouble() < 0.2)
//                    {
//                        GraphicModel tree2 = new GraphicModel(treeMesh, treeMaterials);
//                        tree2.WorldScale = new Vector3(0.2f);
//                        tree2.WorldPosition = new Vector3(
//                           i * 1.73202f + (j & 1) * 0.86601f,
//                           j * 1.5f, 5);
//                        Octree.AddObject(tree2);
//                    }

//                }
//            }

           

//            //for (int i = 2; i < 15; ++i)
//            //{
//            //    for (int j = 2; j < 15; ++j)
//            //    {
//            //        mTitles[i, j] = 0;
//            //    }
//            //}

//            //foreach(Point point in (new Point[]{
//            //    new Point(6,6), 
//            //    new Point(6,8),
//            //    new Point(8,8),
//            //    new Point(10,8),
//            //    new Point(8,10),
//            //    new Point(10,10),
//            //    new Point(12,12),
//            //    new Point(10,12),
//            //    new Point(12,10),
//            //    new Point(12,14),
//            //    new Point(10,6),
//            //    new Point(10,4),
//            //    new Point(8,4),
//            //    new Point(6,4)
//            //}))
//            //{
//            //    mTitles[point.X, point.Y] = 1;
//            //    mTitles[point.X+1, point.Y] = 1;
//            //    mTitles[point.X, point.Y+1] = 1;
//            //    mTitles[point.X+1, point.Y+1] = 1;
//            //}




//            //for (int i = 0; i < 100; ++i)
//            //{
//            //    Point pos = new Point(AleMathUtils.Random.Next(0, size), AleMathUtils.Random.Next(0, size));
//            //    for (int j = 0; j < 300; ++j)
//            //    {
//            //        switch (AleMathUtils.Random.Next(4))
//            //        {
//            //            case 0:
//            //                pos.X++;
//            //                if (size <= pos.X)
//            //                {
//            //                    pos.X = 0;
//            //                }
//            //                break;
//            //            case 1:
//            //                pos.X--;
//            //                if (0 > pos.X)
//            //                {
//            //                    pos.X = size - 1;
//            //                }
//            //                break;
//            //            case 2:
//            //                pos.Y++;
//            //                if (size <= pos.Y)
//            //                {
//            //                    pos.Y = 0;
//            //                }
//            //                break;
//            //            case 3:
//            //                pos.Y--;
//            //                if (0 > pos.Y)
//            //                {
//            //                    pos.Y = size - 1;
//            //                }
//            //                break;
//            //        }
//            //        mTitles[pos.X, pos.Y] = 0;
//            //    }

//            //}

//            for (int i = 0; i < size - 1; ++i)
//            {
//                for (int j = 0; j < size - 1; ++j)
//                {
//                    GraphicModel wallModel = GetWall(i, j);
//                    if (null != wallModel)
//                    {
//                        wallModel.WorldPosition = new Vector3(i, j, 0);

//                        //if (null == batch)
//                        //{
//                        //    batch = mGeometryBatchManager.CreateGeometryBatch();
//                        //    batch.WorldPosition = new Vector3(iBatchIndex * mBatchSize, jBatchIndex * mBatchSize, 0);
//                        //    terrainGeometryBatches[iBatchIndex, jBatchIndex] = batch;
//                        //}

//                        mTerrainStaticGeomtery.AddGraphicModel(wallModel);

//                        //batch.AddGraphicModel(wallModel);
//                    }
//                }
//            }

//            //foreach (GeometryBatch batch in terrainGeometryBatches)
//            //{
//            //    if (null != batch)
//            //    {
//            //        mOctree.AddObject(batch);
//            //    }
//            //}

//            //aux

//            //for (int x = 0; x < 20; ++x)
//            //{
//            //    for (int y = 0; y < 20; ++y)
//            //    {
//            //        GeometryBatch geometryBatch = mGeometryBatchManager.CreateGeometryBatch();
//            //        for (int i = 0; i < 20; ++i)
//            //        {
//            //            for (int j = 0; j < 20; ++j)
//            //            {
//            //                mPalette.TerrainModel1011.WorldPosition = new Vector3(i, j, 0);
//            //                geometryBatch.AddGraphicModel(mPalette.TerrainModel1011);
//            //            }
//            //        }
//            //        geometryBatch.WorldPosition = new Vector3(x * 21, y * 21, 0);
//            //        mOctree.AddObject(geometryBatch);
//            //    }
//            //}


//            // Material grassMaterial = new Material(content.Load<MaterialEffect>("grassFx"), DefaultRenderLayers.GroundStandingObjects);
//            //grassMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("grass"));
//            //mGrass = new Grass(grassMaterial, SceneManager.GraphicsDevice);
//        }

//        void KeyboardManager_KeyUp(Microsoft.Xna.Framework.Input.Keys key, KeyboardManager keyboardManager)
//        {
//            if (key == Microsoft.Xna.Framework.Input.Keys.S)
//            {
//                ScenePass shadowPass = GetScenePass("ShadowPass");
//                shadowPass.IsEnabled = !shadowPass.IsEnabled;
//                if (!shadowPass.IsEnabled)
//                {
//                    shadowPass.RenderTarget.Clear(Color.White);
//                }

//            }
//            if (key == Microsoft.Xna.Framework.Input.Keys.X)
//            {
//                VideoSettings videoSettings = AppSettingsManager.GetSettings<VideoSettings>();
//                videoSettings.Fullscreen = false;
//                videoSettings.ScreenWidth = 800;
//                videoSettings.ScreenHeight = 600;
//                AppSettingsManager.CommitSettings(videoSettings);
//            }
//        }

//        //protected override Camera CreateMainCamera()
//        //{
//        //    return new OrthoCamera(Vector3.Zero, 1, new Vector2(-0.8f, 0), 20000, 3, 1.55f, -1.57f);
//        //}

//        protected override void OnDispose()
//        {
//            SceneManager.KeyboardManager.KeyUp -= KeyboardManager_KeyUp;

//            mTerrainStaticGeomtery.Dispose();
//        }
//        //public virtual void Draw(Renderer renderer, AleGameTime gameTime, RenderTargetManager renderTargetManager)
//        //{
//        //    mGraphicsDevice.Clear(Color.CornflowerBlue);

//        //    renderer.Begin(mCamera, renderTargetManager, null);

//        //    mOctree.ForEachObject(delegate(IOctreeObject obj)
//        //    {
//        //        Renderable renderable = obj as Renderable;
//        //        if (null != renderable)
//        //        {
//        //            renderable.EnqueRenderableUnits(renderer, gameTime);
//        //        }
//        //    }, mCamera);

//        //    mGrass.EnqueRenderableUnits(renderer, gameTime);

//        //    renderer.End(gameTime);
//        //}

//        public override void Update(AleGameTime gameTime)
//        {
//            Camera camera = (Camera)MainCamera;

//            Vector2 curPos =  SceneManager.MouseManager.CursorPosition;
//            Vector3 mouseMovement = SceneManager.MouseManager.CursorPositionDelta;
//            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Left))
//            {
//                //Vector3 v1 = SceneManager.GraphicsDevice.Viewport.Unproject(new Vector3(curPos, 0),
//                //    MainCamera.ProjectionTransformation, MainCamera.ViewTransformation, Matrix.Identity);
//                //Vector3 v2 = SceneManager.GraphicsDevice.Viewport.Unproject(new Vector3(curPos, 1),
//                //    MainCamera.ProjectionTransformation, MainCamera.ViewTransformation, Matrix.Identity);

//                //Ray ray = new Ray(v1, Vector3.Normalize(v2 - v1));

//                Plane plane = new Plane(Vector3.UnitZ, -2);

//                Ray ray;
//                MainCamera.CameraToViewport(curPos, SceneManager.GraphicsDevice.Viewport, out ray); 

//                float? intersection = ray.Intersects(plane);
//                if (null != intersection)
//                {
//                    Vector3 intPoint  = ray.Position + intersection.Value * ray.Direction;

//                    //mTitles[(int)intPoint.X, (int)intPoint.Y] = 0;

//                    mUjo.IsVisible = true;
//                    mUjo.WorldPosition = intPoint += new Vector3(0, 0, 0.9f);
//                }
//                else
//                {
//                    mUjo.IsVisible = false;
//                }
//            }
//            else
//            {
//                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right)) // zoom
//                {
//                    if(SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
//                    {
//                        camera.RotationArroundTarget -= new Vector2(mouseMovement.Y / 200.0f, mouseMovement.X / 200.0f);
//                    }
//                    else
//                    {
//                        camera.DistanceToTarget += mouseMovement.Y / 1.0f;
//                    }

//                    //  MouseManager.ClipRealCursor = true;
//                }
//                else
//                {
//                    if (SceneManager.MouseManager.IsButtonDown(MouseButton.Middle))
//                    {//movement
//                        Vector2 dirVec = new Vector2(camera.TargetWorldPosition.X - MainCamera.WorldPosition.X,
//                            camera.TargetWorldPosition.Y - MainCamera.WorldPosition.Y);
//                        dirVec.Normalize();

//                        Vector2 perpDir;
//                        AleMathUtils.GetPerpVector(ref dirVec, out perpDir);
//                        perpDir *= mouseMovement.X / 10.0f;
//                        dirVec *= mouseMovement.Y / 10.0f;

//                        camera.TargetWorldPosition += new Vector3(perpDir.X + dirVec.X, perpDir.Y + dirVec.Y, 0);
//                    }
//                }
//            }
//        }


//        protected override List<ScenePass> CreateScenePasses(GraphicsDevice graphicsDevice, RenderTargetManager renderTargetManager, ContentGroup content)
//        {
//            Camera mainCamera = new Camera(Vector3.Zero, 100, new Vector2(-0.8f, 0), 20000, 3, 1.55f, -1.57f);
//            mainCamera.DistanceToTarget = 10;

//            //return null;
//            List<ScenePass> scenePasses = new List<ScenePass>();
//            scenePasses.Add(new ShadowScenePass(mainCamera, this, mLightDir, mGroundPlane, renderTargetManager, content));
//            scenePasses.Add(new WaterReflectionPass(mainCamera, this, renderTargetManager, content));
//            scenePasses.Add(new ScenePass("Default", this,  mainCamera, null, Color.Black));

//            scenePasses[0].RenderTarget.Clear(Color.White);

//            //scenePasses[0].IsEnabled = false;

//            return scenePasses;
//        }

//        protected GraphicModel GetWall(int i, int j)
//        {
//            //0100
//            if (IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0100.WorldOrientation = Quaternion.Identity;
//                return mPalette.TerrainModel0100;
//            }
//            if (!IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0100.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
//                return mPalette.TerrainModel0100;
//            }
//            if (!IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0100.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi);
//                return mPalette.TerrainModel0100;
//            }
//            if (!IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0100.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2 * 3);
//                return mPalette.TerrainModel0100;
//            }

//            //0011
//            if (!IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0011.WorldOrientation = Quaternion.Identity;
//                return mPalette.TerrainModel0011;
//            }
//            if (IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
//                return mPalette.TerrainModel0011;
//            }
//            if (IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi);
//                return mPalette.TerrainModel0011;
//            }
//            if (!IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2 * 3);
//                return mPalette.TerrainModel0011;
//            }

//            //0101
//            if (IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0101.WorldOrientation = Quaternion.Identity;
//                return mPalette.TerrainModel0101;
//            }
//            if (!IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel0101.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
//                return mPalette.TerrainModel0101;
//            }

//            //1011
//            if (!IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel1011.WorldOrientation = Quaternion.Identity;
//                return mPalette.TerrainModel1011;
//            }
//            if (IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && !IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel1011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
//                return mPalette.TerrainModel1011;
//            }
//            if (IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && !IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel1011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.Pi);
//                return mPalette.TerrainModel1011;
//            }
//            if (IsTitlePassable(i, j) && !IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                mPalette.TerrainModel1011.WorldOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2 * 3);
//                return mPalette.TerrainModel1011;
//            }

//            //1111
//            if (IsTitlePassable(i, j) && IsTitlePassable(i, j + 1) && IsTitlePassable(i + 1, j + 1) && IsTitlePassable(i + 1, j))
//            {
//                return mPalette.TerrainModel1111;
//            }

//            //0000
//            return mPalette.TerrainModel0000;
//        }

//        private bool IsTitlePassable(int i, int j)
//        {
//            return 0 != mTitles[i, j];
//        }
//    }


//    [DataObject(MaxCachedCnt = 5)]
//    public class TerrainPaletteSettings
//    {
//        [DataProperty]
//        public string TerrainMesh0000 { get; set; }
//        [DataProperty]
//        public string TerrainMesh0011 { get; set; }
//        [DataProperty]
//        public string TerrainMesh0100 { get; set; }
//        [DataProperty]
//        public string TerrainMesh0101 { get; set; }
//        [DataProperty]
//        public string TerrainMesh1011 { get; set; }
//        [DataProperty]
//        public string TerrainMesh1111 { get; set; }

//        [DataProperty]
//        public string WallMaterial { get; set; }
//        [DataProperty]
//        public string SurfaceMaterial { get; set; }
//        [DataProperty]
//        public string WaterMaterial { get; set; }

//    }

//    public class TerrainPalette
//    {
//        private GraphicModel mTerrainModel0000;
//        private GraphicModel mTerrainModel0011;
//        private GraphicModel mTerrainModel0100;
//        private GraphicModel mTerrainModel0101;
//        private GraphicModel mTerrainModel1011;
//        private GraphicModel mTerrainModel1111;

//        public GraphicModel TerrainModel0000
//        {
//            get { return mTerrainModel0000; }
//        }
//        public GraphicModel TerrainModel0011
//        {
//            get { return mTerrainModel0011; }
//        }
//        public GraphicModel TerrainModel0100
//        {
//            get { return mTerrainModel0100; }
//        }
//        public GraphicModel TerrainModel0101
//        {
//            get { return mTerrainModel0101; }
//        }
//        public GraphicModel TerrainModel1011
//        {
//            get { return mTerrainModel1011; }
//        }
//        public GraphicModel TerrainModel1111
//        {
//            get { return mTerrainModel1111; }
//        }

//        public TerrainPalette(TerrainPaletteSettings settings, ContentManager content)
//        {
//            Dictionary<string, Material> materials = new Dictionary<string, Material>(StringComparer.InvariantCultureIgnoreCase);
//            materials["Wall"] = content.Load<Material>(settings.WallMaterial);
//            materials["Surface"] = content.Load<Material>(settings.SurfaceMaterial);
//            materials["Water"] = content.Load<Material>(settings.WaterMaterial);


//            mTerrainModel0000 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh0000), materials);
//            mTerrainModel0011 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh0011), materials);
//            mTerrainModel0100 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh0100), materials);
//            mTerrainModel0101 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh0101), materials);
//            mTerrainModel1011 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh1011), materials);
//            mTerrainModel1111 = new GraphicModel(content.Load<Mesh>(settings.TerrainMesh1111), materials);
//        }
//    }












//    /// <summary>
//    /// Custom vertex structure for drawing point sprite particles.
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential)]
//    public struct GrassVertex
//    {
//        public Vector3 Position;

//        /// <summary>
//        /// Uv for bilboard computing
//        /// </summary>
//        public HalfVector2 NormalizedUv;


//        // Describe the layout of this vertex structure.
//        public static readonly VertexElement[] VertexElements =
//        {
//            new VertexElement(0, 0, VertexElementFormat.Vector3,
//                                    VertexElementMethod.Default,
//                                    VertexElementUsage.Position, 0),


//            new VertexElement(0, 12, VertexElementFormat.HalfVector2,
//                                     VertexElementMethod.Default,
//                                     VertexElementUsage.TextureCoordinate, 0),
//        };


//        public const int SizeInBytes = 16;
//    }

//    class Grass : Renderable, IRenderableUnit, IFrameListener
//    {
//        Material mMaterial;
//        private List<Vector2> mGrassBlades = new List<Vector2>(1000);
//        DynamicQuadGeometryManager<GrassVertex> mGrassDynamicQuadGeometryManager;
//        TransientQuadGeometry<GrassVertex> mTransientQuadGeometry = new TransientQuadGeometry<GrassVertex>();
//        long LastRenderedFrameNum = -1; //todo
//        static GrassVertex[] GrassVerticies = null;


//        Renderable IRenderableUnit.ParentRenderable
//        {
//            get { return this; }
//        }

//        Material IRenderableUnit.Material
//        {
//            get { return mMaterial; }
//        }

//        public Grass(Material material, GraphicsDevice graphicsDevice)
//            : base(new BoundingSphere(Vector3.Zero, 500), false)
//        {
//            mGrassDynamicQuadGeometryManager = new DynamicQuadGeometryManager<GrassVertex>(graphicsDevice, new VertexDeclaration(graphicsDevice, GrassVertex.VertexElements));
//            for (int i = 0; i < mGrassBlades.Capacity; ++i)
//            {
//                mGrassBlades.Add(new Vector2((float)AleMathUtils.Random.NextDouble() * 20, (float)AleMathUtils.Random.NextDouble() * 20));
//            }
//            mMaterial = material;
//        }


//        protected override void OnEnqueRenderableUnits(Renderer renderer, AleGameTime gameTime)
//        {
//            renderer.EnqueueRenderable(this);

//            if (null == GrassVerticies || (mGrassBlades.Count * 4 > GrassVerticies.Length))
//            {
//                GrassVerticies = new GrassVertex[mGrassBlades.Count * 4];

//                for (int i = 0; i < GrassVerticies.Length; )
//                {
//                    GrassVerticies[i++].NormalizedUv = new HalfVector2(0, 0);
//                    GrassVerticies[i++].NormalizedUv = new HalfVector2(1, 0);
//                    GrassVerticies[i++].NormalizedUv = new HalfVector2(0, 1);
//                    GrassVerticies[i++].NormalizedUv = new HalfVector2(1, 1);
//                }
//            }

//            int vIndex = 0;
//            for (int i = 0; i < mGrassBlades.Count; ++i)
//            {
//                Vector2 pos = mGrassBlades[i];
//                GrassVerticies[vIndex++].Position = new Vector3(pos, 3);
//                GrassVerticies[vIndex++].Position = new Vector3(pos, 3);
//                GrassVerticies[vIndex++].Position = new Vector3(pos, 3);
//                GrassVerticies[vIndex++].Position = new Vector3(pos, 3);
//            }


//            mGrassDynamicQuadGeometryManager.AllocGeometry(GrassVerticies, 0, mGrassBlades.Count * 4, ref mTransientQuadGeometry);
//        }

//        #region IRenderableUnit Members


//        void IRenderableUnit.Render(AleGameTime gameTime)
//        {
//            mTransientQuadGeometry.Draw();
//        }

//        void IRenderableUnit.UpdateMaterialEffectParameters()
//        {
//        }

//        #endregion



//        #region IFrameListener Members

//        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
//        {
//            ((IFrameListener)mGrassDynamicQuadGeometryManager).BeforeUpdate(gameTime);
//        }

//        void IFrameListener.AfterUpdate(AleGameTime gameTime)
//        {
//            ((IFrameListener)mGrassDynamicQuadGeometryManager).AfterUpdate(gameTime);
//        }

//        void IFrameListener.BeforeRender(AleGameTime gameTime)
//        {
//            ((IFrameListener)mGrassDynamicQuadGeometryManager).BeforeRender(gameTime);
//        }

//        void IFrameListener.AfterRender(AleGameTime gameTime)
//        {
//            ((IFrameListener)mGrassDynamicQuadGeometryManager).AfterRender(gameTime);
//        }

//        #endregion

//    }


//    class ShadowScenePass : ScenePass
//    {
//        public ShadowScenePass(ICamera mainCamera, BaseScene scene, Vector3 lightDir, Plane groundPlane, RenderTargetManager renderTargetManager, ContentGroup content)
//            : base("ShadowPass", scene, new ShadowOrthoCamera((Camera)mainCamera, lightDir, groundPlane),
//                CreateRenderTarget(renderTargetManager), Color.White)
//        {
//        }

//        static private AleRenderTarget CreateRenderTarget(RenderTargetManager renderTargetManager)
//        {
//            PresentationParameters pp = renderTargetManager.GraphicsDevice.PresentationParameters;
//            return renderTargetManager.CreateRenderTarget("ShadowMap", 1024,1024, 1, pp.BackBufferFormat, DepthFormat.Depth16);
//        }
//    }


//    class ShadowOrthoCamera : ICamera, IDisposable
//    {
//        public event CameraTransformationChangedHandler ViewTransformationChanged;
//        public event CameraTransformationChangedHandler ProjectionTransformationChanged;
//        public event CameraTransformationChangedHandler ViewProjectionTransformationChanged;

//        private Camera mMainCamera;
//        bool mIsDisposed = false;
//        private Vector3 mLightDir = new Vector3(-0.3333333f, 0.1f, 1.4f);
//        private Vector3 mWorldPosition = Vector3.Zero;
//        private Plane mGroundPlane;

//        /// <summary>
//        /// Current view transformation matrix
//        /// </summary>
//        private Matrix mViewTransformation;

//        /// <summary>
//        /// Current projection transformation matrix
//        /// </summary>
//        private Matrix mProjectionTransformation;

//        /// <summary>
//        /// Current view-projection transformation matrix
//        /// </summary>
//        private Matrix mViewProjectionTransformation;

//        /// <summary>
//        /// Inverted mViewProjectionTransformation
//        /// </summary>
//        private Matrix mViewProjectionTransformationInv;
        
//        private BoundingFrustum mWorldFrustum= new BoundingFrustum(Matrix.Identity);
//        private ReadOnlyCollection<Vector3> mFrustumCorners;


//        #region Property

//        /// <summary>
//        /// Gets the current view transformation matrix
//        /// </summary>
//        public Matrix ViewTransformation
//        {
//            get { return mViewTransformation; }
//        }

//        /// <summary>
//        /// Gets the current projection transformation matrix
//        /// </summary>
//        public Matrix ProjectionTransformation
//        {
//            get { return mProjectionTransformation; }
//        }

//        /// <summary>
//        /// Gets the current view*projection transformation matrix
//        /// </summary>
//        public Matrix ViewProjectionTransformation
//        {
//            get { return mViewProjectionTransformation; }
//        }

//        /// <summary>
//        /// Inverted ViewProjectionTransformation
//        /// </summary>
//        public Matrix ViewProjectionTransformationInv
//        {
//            get { return mViewProjectionTransformationInv; }
//        }

//        /// <summary>
//        /// Gets the corners of the bounding Frustum
//        /// </summary>
//        public ReadOnlyCollection<Vector3> FrustumCorners
//        {
//            get { return mFrustumCorners; }
//        }

//        /// <summary>
//        /// Camera's up vector
//        /// </summary>
//        public Vector3 CameraUp
//        {
//            get { return Vector3.UnitZ; }
//            set { throw new NotImplementedException(); }
//        }

//        /// <summary>
//        /// Gets the actual position of the camera
//        /// </summary>
//        public Vector3 WorldPosition
//        {
//            get { return  mWorldPosition; }
//        }

//        /// <summary>
//        /// Gets the bottom plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumBottom
//        {
//            get { return mWorldFrustum.Bottom; }
//        }

//        /// <summary>
//        /// Gets the far plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumFar
//        {
//            get { return mWorldFrustum.Far; }
//        }

//        /// <summary>
//        /// Gets the left plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumLeft
//        {
//            get { return mWorldFrustum.Left; }
//        }

//        /// <summary>
//        /// Gets the near plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumNear
//        {
//            get { return mWorldFrustum.Near; }
//        }

//        /// <summary>
//        /// Gets the right plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumRight
//        {
//            get { return mWorldFrustum.Right; }
//        }

//        /// <summary>
//        /// Gets the top plane of the BoundingFrustum.
//        /// </summary>
//        public Plane FrustumTop
//        {
//            get { return mWorldFrustum.Top; }
//        }

//        #endregion Property

//        public ShadowOrthoCamera(Camera mainCamera, Vector3 lightDir, Plane groundPlane)
//        {
//            if (null == mainCamera) throw new ArgumentNullException("mainCamera");

//            mLightDir = lightDir;
//            mLightDir.Normalize();

//            mGroundPlane = groundPlane;
            
//            mMainCamera = mainCamera;
           
//            mMainCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mMainCamera_ViewProjectionTransformationChanged);

//            UpdateViewProjectionTransformation();
//        }

//        void mMainCamera_ViewProjectionTransformationChanged(ICamera camera)
//        {
//            UpdateViewProjectionTransformation();
//        }

//        public virtual void Dispose()
//        {
//            if (!mIsDisposed)
//            {
//                mMainCamera.ViewProjectionTransformationChanged -= mMainCamera_ViewProjectionTransformationChanged;

//                mIsDisposed = true;
//                GC.SuppressFinalize(this);
//            }
//        }

//        public void CameraToViewport(Vector2 point, Viewport viewport, out Ray ray)
//        {
//            throw new NotImplementedException();
//        }

//        public bool IsInSight(ref BoundingBox boundingBox)
//        {
//            return mMainCamera.IsInSight(ref boundingBox);
//        }

//        public bool IsInSight(ref BoundingSphere boundingSphere)
//        {
//            return mMainCamera.IsInSight(ref boundingSphere);
//        }

//        public bool IsInSight(BoundingBox boundingBox)
//        {
//            return mMainCamera.IsInSight(boundingBox);
//        }

//        public bool IsInSight(BoundingSphere boundingSphere)
//        {
//            return mMainCamera.IsInSight(boundingSphere);
//        }

//        public ContainmentType IsInSightEx(ref BoundingBox boundingBox)
//        {
//            return mMainCamera.IsInSightEx(ref boundingBox);
//        }

//        public ContainmentType IsInSightEx(ref BoundingSphere boundingSphere)
//        {
//            return mMainCamera.IsInSightEx(ref boundingSphere);
//        }

//        public ContainmentType IsInSightEx(BoundingBox boundingBox)
//        {
//            return mMainCamera.IsInSightEx(boundingBox);
//        }

//        public ContainmentType IsInSightEx(BoundingSphere boundingSphere)
//        {
//            return mMainCamera.IsInSightEx(boundingSphere);
//        }

//        private void UpdateViewProjectionTransformation()
//        {
//            if (null != mMainCamera)
//            {
//                //based on http://creators.xna.com/en-US/sample/shadowmapping1

//                Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
//                                                           -mLightDir,
//                                                           CameraUp);
//                // Get the corners of the frustum
//                //                List<Vector3> frustumCorners = new List<Vector3>(mMainCamera.FrustumCorners);
//                IList<Vector3> frustumCorners = ComputeMainCameraClippedFrustumPoints();

//                // Transform the positions of the corners into the direction of the light
//                for (int i = 0; i < frustumCorners.Count; i++)
//                {
//                    frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
//                }

//                // Find the smallest box around the points
//                BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

//                Vector3 boxSize = lightBox.Max - lightBox.Min;
//                Vector3 halfBoxSize = boxSize * 0.5f;

//                // The position of the light should be in the center of the back pannel of the box. 
//                mWorldPosition = lightBox.Min + halfBoxSize;
//                mWorldPosition.Z = lightBox.Min.Z;

//                // We need the position back in world coordinates so we transform  the light position by the inverse of the lights rotation
//                mWorldPosition = Vector3.Transform(mWorldPosition,
//                                                  Matrix.Invert(lightRotation));

//                // Create the view matrix for the light
//                mViewTransformation = Matrix.CreateLookAt(mWorldPosition, mWorldPosition - mLightDir, CameraUp);
//                mProjectionTransformation = Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z);

//                Matrix.Multiply(ref mViewTransformation, ref mProjectionTransformation, out mViewProjectionTransformation);
//                mWorldFrustum.Matrix = mViewProjectionTransformation;
//                mFrustumCorners = new ReadOnlyCollection<Vector3>(mWorldFrustum.GetCorners());

//                Matrix.Invert(ref mViewProjectionTransformation, out mViewProjectionTransformationInv);
//            }
//            else
//            {
//                mViewTransformation = Matrix.Identity;
//            }
            
//            if (null != ViewTransformationChanged)
//            {
//                ViewTransformationChanged.Invoke(this);
//            }
//            if (null != ProjectionTransformationChanged)
//            {
//                ProjectionTransformationChanged.Invoke(this);
//            }
//            if (null != ViewProjectionTransformationChanged)
//            {
//                ViewProjectionTransformationChanged.Invoke(this);
//            }
//        }


//        IList<Vector3> ComputeMainCameraClippedFrustumPoints()
//        {
//            //Far clipping plane will be a ground plane
            
//            Vector3[] corners = new Vector3[8];

//            Plane frustumNear = mMainCamera.FrustumNear;
//            Plane frustumFar = mGroundPlane; //ground plane
//            Plane frustumLeft = mMainCamera.FrustumLeft;
//            Plane frustumRight = mMainCamera.FrustumRight;
//            Plane frustumTop = mMainCamera.FrustumTop;
//            Plane frustumBottom = mMainCamera.FrustumBottom;

//            Ray intRay;

//            GetPlanePlaneIntersection(ref frustumNear, ref frustumLeft, out intRay);
//            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[0]);
//            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[3]);

//            GetPlanePlaneIntersection(ref frustumRight, ref frustumNear, out intRay);
//            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[1]);
//            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[2]);

//            GetPlanePlaneIntersection(ref frustumLeft, ref frustumFar, out intRay);
//            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[4]);
//            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[7]);

//            GetPlanePlaneIntersection(ref frustumFar, ref frustumRight, out intRay);
//            GetPlaneRayIntersection(ref frustumTop, ref intRay, out corners[5]);
//            GetPlaneRayIntersection(ref frustumBottom, ref intRay, out corners[6]);

//            return corners;
//        }

//        private void GetPlanePlaneIntersection(ref Plane p1, ref Plane p2, out Ray ray)
//        {
//            Vector3.Cross(ref p1.Normal, ref p2.Normal, out ray.Direction);
//            ray.Position = Vector3.Cross(((-p1.D * p2.Normal) + (p2.D * p1.Normal)), ray.Direction) / ray.Direction.LengthSquared();
//        }

//        private void GetPlaneRayIntersection(ref Plane plane, ref Ray ray, out Vector3 point)
//        {
//            point = ray.Position + (ray.Direction * ((-plane.D - Vector3.Dot(plane.Normal, ray.Position)) / Vector3.Dot(plane.Normal, ray.Direction)));
//        }
//    }










//     class WaterReflectionPass : ScenePass
//    {
//        ICamera mMainCamera;

//        public WaterReflectionPass(ICamera mainCamera, BaseScene scene, RenderTargetManager renderTargetManager, ContentGroup content)
//            : base("WaterReflectionPass", scene, CreateReflectionCamera(mainCamera),
//                CreateRenderTarget(renderTargetManager), Color.White)
//        {
//            mMainCamera = mainCamera;
//        }

//        private static ICamera CreateReflectionCamera(ICamera mainCamera)
//        {
//            return new WaterReflectionCamera((Camera)mainCamera);

//        }

//        static private AleRenderTarget CreateRenderTarget(RenderTargetManager renderTargetManager)
//        {
//            PresentationParameters pp = renderTargetManager.GraphicsDevice.PresentationParameters;
//            return renderTargetManager.CreateRenderTarget("ReflectionMap", 1024,1024, 1, pp.BackBufferFormat, DepthFormat.Depth16);
//        }
//    }



//    class WaterReflectionCamera : Camera, IDisposable
//    {
//        Camera mMainCamera;
//        bool mIsDisposed = false;

//        public WaterReflectionCamera(Camera mainCamera)
//            :base(mainCamera.TargetWorldPosition, mainCamera.DistanceToTarget, Vector2.Zero, mainCamera.MaxDistanceToTarget, 
//            mainCamera.MinDistanceToTarget, -mainCamera.MinRotX, -mainCamera.MaxRotX)
//        {
//            if (null == mainCamera) throw new ArgumentNullException("mainCamera");

//            CameraUp = -mainCamera.CameraUp;

//            mMainCamera = mainCamera;

//            mMainCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mMainCamera_ViewProjectionTransformationChanged);

//            Update();
//        }

//        void mMainCamera_ViewProjectionTransformationChanged(ICamera camera)
//        {
//            Update();
//        }

//        public virtual void Dispose()
//        {
//            if (!mIsDisposed)
//            {
//                mMainCamera.ViewProjectionTransformationChanged -= mMainCamera_ViewProjectionTransformationChanged;

//                mIsDisposed = true;
//                GC.SuppressFinalize(this);
//            }
//        }

//        private void Update()
//        {
//            if (null != mMainCamera)
//            {
//                Vector3 targetPos = mMainCamera.TargetWorldPosition;
//                targetPos.Z *= -1;
//                TargetWorldPosition = targetPos;
//                Vector2 rotationArroundTarget = mMainCamera.RotationArroundTarget;
//                rotationArroundTarget.X = -rotationArroundTarget.X;
//                RotationArroundTarget = rotationArroundTarget;
//                DistanceToTarget = mMainCamera.DistanceToTarget;
//            }
//        }


//    }
//}
