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

using System;
using SimpleOrmFramework;
using System.IO;
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Ale;
using Ale.Scene;

namespace Conquera
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (1 == 0)
            {
                GenerateContent();
            }
            else
            {
                using (Application app = new Application())
                {
                    //try
                    //{
                    app.Run();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Tracer.WriteError(ex.ToString());
                    //    //MessageBox.Show(ex.ToString());
                    //    throw;
                    //}
                }
            }
        }

        static void GenerateContent()
        {
            using (OrmManager ormManager = new OrmManager(string.Format("Data Source={0};Version=3;New=False;Compress=False", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Conquera.mod"))))
            {
                using (SofTransaction transaction = ormManager.BeginTransaction())
                {
                    //long particleMaterialId;
                    //{
                    //    MaterialSettings particleMaterial = new MaterialSettings("ParticleMaterial", "particle", DefaultRenderLayers.GroundStandingObjects);
                    //    particleMaterial.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "smoke"));
                    //    particleMaterial.Params.Add(new Vector2ArrayMaterialParamSettings("gUvs", new Vector2[]
                    //    {
                    //        new Vector2(0,0),
                    //        new Vector2(0.5f, 0),
                    //        new Vector2(0, 0.5f),
                    //        new Vector2(0.5f, 0.5f),
                    //    }));
                    //    particleMaterialId = ormManager.SaveObject(particleMaterial);
                    //}

                    //{
                    //    MaterialSettings particleMaterial2 = new MaterialSettings("ParticleMaterial2", "particle2", DefaultRenderLayers.GroundStandingObjects);
                    //    particleMaterial2.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "smoke"));
                    //    ormManager.SaveObject(particleMaterial2);
                    //}

                    //{
                    //    MaterialSettings surfaceMat = new MaterialSettings("SurfaceMat", "SurfaceFx", DefaultRenderLayers.Ground);
                    //    surfaceMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "HexCellUv"));
                    //    ormManager.SaveObject(surfaceMat);
                    //}

                    //{
                    //    MaterialSettings ujoMaterial = new MaterialSettings("UjoMaterial", "GameUnitFx", DefaultRenderLayers.GroundStandingObjects);
                    //    ormManager.SaveObject(ujoMaterial);
                    //}

                    //{
                    //    MaterialSettings wallMat = new MaterialSettings("WallMat", "wallFx", DefaultRenderLayers.GroundWall);
                    //    wallMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "WallSurface"));
                    //    wallMat.Params.Add(new Texture2DMaterialParamSettings("gWallLightMap", "WallLightMap"));
                    //    ormManager.SaveObject(wallMat);
                    //}

                    //{
                    //    MaterialSettings waterPlaneMat = new MaterialSettings("waterPlaneMat", "WaterPlaneFx", DefaultRenderLayers.Water);
                    //    waterPlaneMat.Params.Add(new Texture2DMaterialParamSettings("gNoiseMap", "Wave"));
                    //    waterPlaneMat.Params.Add(new Texture2DMaterialParamSettings("gBwNoiseMap", "BwNoise"));
                    //    waterPlaneMat.Params.Add(new TextureCubeMaterialParamSettings("gEnvMap", "EnvMap"));
                    //    ormManager.SaveObject(waterPlaneMat);
                    //}


                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
                    //    PointParticleEmitterSettings particleEmitterSettings = new PointParticleEmitterSettings();
                    //    particleSystemSettings.Name = "ParticleSystem1";
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);
                    //    //particleSystemSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 0), new Vector2(0.4f, 0), new Vector2(0.4f, 1000), new Vector2(0.6f, 1000), new Vector2(0.6f, 0), new Vector2(1, 0)); 
                    //    //particleSystemSettings.MaxParticleCnt = 100;
                    //    particleEmitterSettings.MaxFastForwardTime = particleEmitterSettings.FastForwardTimeOnLoad = 3;
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.7f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.1f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.3f, 0), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.6f, 1), new Vector2(1, 0) });
                    //    //            particleSystemSettings.ParticleSettings.ParticleSpeed = new TimeFunction(new Vector2(0, 0.05f), new Vector2(0.5f, 0.0f), new Vector2(1, 0.05f));
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(5, 2);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(5, 0);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 8);

                    //    GravityParticleAffectorSettings gravityParticleAffectorSettings = new GravityParticleAffectorSettings();
                    //    gravityParticleAffectorSettings.GravityDirection = new Vector3(0, 0.6f, 0);
                    //    particleEmitterSettings.ParticleAffectors.Add(gravityParticleAffectorSettings);
                    //    particleEmitterSettings.ColorVariation = new Vector4(2, 2, 2, 2);
                    //    particleEmitterSettings.Material = particleMaterialId;
                    //    particleEmitterSettings.Name = "Emitter1";


                    //    SphereParticleEmitterSettings particleEmitterSettings2 = new SphereParticleEmitterSettings();
                    //    particleEmitterSettings2.Name = "Emitter2";
                    //    particleEmitterSettings2.Radius = 5;
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings2);
                    //    //particleSystemSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 0), new Vector2(0.4f, 0), new Vector2(0.4f, 1000), new Vector2(0.6f, 1000), new Vector2(0.6f, 0), new Vector2(1, 0)); 
                    //    //particleSystemSettings.MaxParticleCnt = 100;
                    //    particleEmitterSettings2.MaxFastForwardTime = particleEmitterSettings2.FastForwardTimeOnLoad = 3;
                    //    particleEmitterSettings2.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.7f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings2.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.1f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings2.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.3f, 0), new Vector2(1, 0) });
                    //    particleEmitterSettings2.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.6f, 1), new Vector2(1, 0) });
                    //    //            particleSystemSettings.ParticleSettings.ParticleSpeed = new TimeFunction(new Vector2(0, 0.05f), new Vector2(0.5f, 0.0f), new Vector2(1, 0.05f));
                    //    particleEmitterSettings2.ParticleSettings.ParticleSpeed = new TimeFunction(5, 2);
                    //    particleEmitterSettings2.ParticleSettings.ParticleSize = new TimeFunction(5, 0);
                    //    particleEmitterSettings2.ParticleSettings.ParticleRotation = new TimeFunction(0, 8);
                    //    particleEmitterSettings2.ParticleAffectors.Add(gravityParticleAffectorSettings);

                    //    particleEmitterSettings2.RelativePosition = new Vector3(5, 0, 0);

                    //    particleEmitterSettings2.Material = particleMaterialId;

                    //    ormManager.SaveObject(particleSystemSettings);

                    //}

                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
                    //    PointParticleEmitterSettings particleEmitterSettings = new PointParticleEmitterSettings();
                    //    particleSystemSettings.Name = "ParticleSystem2";
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);
                    //    //particleSystemSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 0), new Vector2(0.4f, 0), new Vector2(0.4f, 1000), new Vector2(0.6f, 1000), new Vector2(0.6f, 0), new Vector2(1, 0)); 
                    //    //particleSystemSettings.MaxParticleCnt = 100;
                    //    particleEmitterSettings.MaxFastForwardTime = particleEmitterSettings.FastForwardTimeOnLoad = 3;
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.7f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.1f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.3f, 0), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.6f, 1), new Vector2(1, 0) });
                    //    //            particleSystemSettings.ParticleSettings.ParticleSpeed = new TimeFunction(new Vector2(0, 0.05f), new Vector2(0.5f, 0.0f), new Vector2(1, 0.05f));
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.5f, 0.01f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.5f, 0);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);

                    //    particleEmitterSettings.ColorVariation = new Vector4(2, 2, 2, 2);
                    //    //particleEmitterSettings.Material = "ParticleMaterial";
                    //    particleEmitterSettings.Material = particleMaterialId;
                    //    particleEmitterSettings.Name = "Emitter1";

                    //    ormManager.SaveObject(particleSystemSettings);

                    //}

                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();

                    //    SphereParticleEmitterSettings particleEmitterSettings = new SphereParticleEmitterSettings();
                    //    particleSystemSettings.Name = "CellCaptureParticleSystem";
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);

                    //    particleEmitterSettings.Radius = 0.3f;
                    //    particleEmitterSettings.RelativePosition = new Vector3(0, 0, -0.2f);
                    //    particleEmitterSettings.ParticleLifeDuration = 3f;
                    //    particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.7f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(0.6f, 0.1f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.3f, 0), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.8f, 1), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.5f, 0.08f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.6f, 0);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
                    //    particleEmitterSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 1000), new Vector2(0.2f, 1000), new Vector2(0.3f, 1000));
                    //    particleEmitterSettings.CycleTime = 0.5f;

                    //    particleEmitterSettings.Material = ormManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
                    //    particleEmitterSettings.Name = "Emitter1";

                    //    ormManager.SaveObject(particleSystemSettings);
                    //}

                    //{
                    //    //long[] ids = ormManager.FindIds(typeof(GraphicModelSettings), "Name='ujoGm'");
                    //    //if (null != ids && 0 < ids.Length)
                    //    //{
                    //    //    ormManager.DeleteObject(ids[0]);
                    //    //}

                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "ujo";
                    //    settings.Name = "ujoGm";

                    //    settings.BoundsMultiplicator = 2.0f;
                    //    settings.Scale = 0.2f;
                    //    settings.Position = new Vector3(0, 0, 0.6f);


                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("Material", ormManager.FindObjects(typeof(MaterialSettings), "Name='UjoMaterial'")[0]));
                    //    //settings.ConnectionPointAssigments.Add(new ConnectionPointAssigmentSettings("ConnectionPoint",
                    //    //    32, 1));
                    //    ormManager.SaveObject(settings);
                    //}

                    //{
                    //    OctreeSceneObjectSettings obj = new OctreeSceneObjectSettings();
                    //    obj.Name = "PrvyObj";
                    //    obj.GraphicModel = 35;
                    //    ormManager.SaveObject(obj);

                    //}

                    //{
                    //    //ormManager.DeleteObject(39);
                    //    MaterialSettings waterWaveMat = new MaterialSettings("waterWaveMat", "WaterWaveFx", DefaultRenderLayers.Water + 10);
                    //    waterWaveMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "waveSingle"));
                    //    ormManager.SaveObject(waterWaveMat);
                    //}


                    //{
                    //    HexTerrainTileAtlasSettings obj = new HexTerrainTileAtlasSettings();
                    //    obj.Name = "HexTerrainTileAtlas1";
                    //    obj.Material = ormManager.FindObject(typeof(MaterialSettings), "Name = 'SurfaceMat'");
                    //    obj.Size = 6;
                    //    obj.TextureCellSpacing = 0.02f;
                    //    ormManager.SaveObject(obj);

                    //}



                    //{
                    //    MaterialSettings treeMat = new MaterialSettings("TreeMat", "Simple", DefaultRenderLayers.GroundStandingObjects);
                    //    treeMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "treeText"));
                    //    ormManager.SaveObject(treeMat);
                    //}
                    //{
                    //    MaterialSettings grassMat = new MaterialSettings("WallGrassMat", "SimpleNoShadow", DefaultRenderLayers.GroundStandingObjects);
                    //    grassMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "grass"));
                    //    ormManager.SaveObject(grassMat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "tree";
                    //    settings.Name = "TreeGm";
                    //    settings.Scale = 1.0f;
                    //    settings.BoundsMultiplicator = 1.0f;
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("arbol.otono", ormManager.FindObjects(typeof(MaterialSettings), "Name='TreeMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "HexCellWall";
                    //    settings.Name = "Wall1Gm";
                    //    settings.BoundsMultiplicator = 1.0f;
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("Wall", ormManager.FindObjects(typeof(MaterialSettings), "Name='WallMat'")[0]));
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("Grass", ormManager.FindObjects(typeof(MaterialSettings), "Name='WallGrassMat'")[0]));

                    //    ormManager.SaveObject(settings);
                    //}

                    //{
                    //    MaterialSettings mat = new MaterialSettings("CastleMat", "Simple", DefaultRenderLayers.GroundStandingObjects);
                    //    mat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "CastleText"));
                    //    ormManager.SaveObject(mat);
                    //}

                    //{
                    //    MaterialSettings mat = new MaterialSettings("CastleActiveMat", "CastleActiveFx", DefaultRenderLayers.GroundStandingObjects);
                    //    mat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "CastleText"));
                    //    mat.Params.Add(new Texture2DMaterialParamSettings("gLightMap", "CastleLightText"));
                    //    ormManager.SaveObject(mat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Castle";
                    //    settings.Name = "CastleGm";
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='CastleMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Castle";
                    //    settings.Name = "CastleActiveGm";
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='CastleActiveMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}

                    ////{
                    ////    HexTerrainTileSettings obj = new HexTerrainTileSettings();
                    ////    obj.Name = "Grass2Tile";
                    ////    obj.IsPassable = false;
                    ////    obj.TileIndex = new Point(1, 0);
                    ////    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    ////    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    ////    obj.GraphicModels.Add(ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'TreeGm'"));
                    ////    ormManager.SaveObject(obj);
                    ////}

                    //{
                    //    MaterialSettings domMat = new MaterialSettings("DomMat", "Simple", DefaultRenderLayers.GroundStandingObjects);
                    //    domMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "domUv"));
                    //    ormManager.SaveObject(domMat);

                    //}

                    //{
                    //    //GraphicModelSettings settings = ormManager.LoadObject<GraphicModelSettings>(57);
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Dom";
                    //    settings.Name = "DomGm";
                    //    settings.BoundsMultiplicator = 1.0f;
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='DomMat'")[0]));
                    //    settings.ConnectionPointAssigments.Add(new ConnectionPointAssigmentSettings("Chimney",
                    //        32, 1));

                    //    ormManager.SaveObject(settings);
                    //}

                    ////{
                    ////    HexTerrainTileSettings obj = new HexTerrainTileSettings();
                    ////    obj.Name = "TestTile";
                    ////    obj.IsPassable = true;
                    ////    obj.TileIndex = new Point(1, 0);
                    ////    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    ////    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    ////    obj.GraphicModels.Add(ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'DomGm'"));
                    ////    obj.GraphicModels.Add(ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'ujoGm'"));
                    ////    ormManager.SaveObject(obj);
                    ////}


                    //{
                    //    MaterialSettings domMat = new MaterialSettings("HillMat", "Simple", DefaultRenderLayers.GroundStandingObjects);
                    //    domMat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "HillText"));
                    //    ormManager.SaveObject(domMat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Hill";
                    //    settings.Name = "HillGm";
                    //    settings.BoundsMultiplicator = 1.0f;
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='HillMat'")[0]));

                    //    ormManager.SaveObject(settings);
                    //}


                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();

                    //    SphereParticleEmitterSettings particleEmitterSettings = new SphereParticleEmitterSettings();
                    //    particleSystemSettings.Name = "BloodParticleSystem";
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);

                    //    particleEmitterSettings.Radius = 0.3f;
                    //    particleEmitterSettings.RelativePosition = new Vector3(0, 0, -0.2f);
                    //    particleEmitterSettings.ParticleLifeDuration = 3f;
                    //    particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(1.0f, 1) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.1f), new Vector2(0.2f, 0.1f), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.1f), new Vector2(0.3f, 0), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.8f, 1), new Vector2(1, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.5f, 0.08f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.6f, 0);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
                    //    particleEmitterSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 1000), new Vector2(0.2f, 1000), new Vector2(0.3f, 1000));
                    //    particleEmitterSettings.CycleTime = 0.5f;

                    //    particleEmitterSettings.Material = ormManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
                    //    particleEmitterSettings.Name = "Emitter1";

                    //    ormManager.SaveObject(particleSystemSettings);
                    //}


                    //{
                    //    GameUnitSettings obj = new GameUnitSettings();
                    //    obj.DisplayName = "Knight";
                    //    obj.Name = "GameUnit1";
                    //    obj.GraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'ujoGm'");
                    //    obj.AttackAnimation = "Attack";
                    //    obj.IdleAnimation = "Run";
                    //    obj.MoveAnimation = "Run";
                    //    obj.MaxHp = 20;
                    //    obj.Cost = 100;
                    //    obj.Attack = 5;
                    //    obj.Defense = 3;
                    //    obj.BloodParticleSystem = ormManager.FindObject(typeof(ParticleSystemSettings), "Name = 'BloodParticleSystem'"); ;
                    //    obj.DamageAnimationTime = 1.0f;
                    //    ormManager.SaveObject(obj);
                    //}

                    //{
                    //    MaterialSettings mat = new MaterialSettings("Cursor3dMat", "Cursor3dFx", DefaultRenderLayers.Cursor3D);
                    //    mat.Params.Add(new Vector3MaterialParamSettings("gColor", Vector3.One));
                    //    ormManager.SaveObject(mat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Cursor3d";
                    //    settings.Name = "Cursor3dGm";
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='Cursor3dMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}

                    //{
                    //    MaterialSettings mat = new MaterialSettings("Cursor3dCellSelMat", "Cursor3dCellSelFx", DefaultRenderLayers.Cursor3dCellSel);
                    //    mat.Params.Add(new Vector3MaterialParamSettings("gColor", Vector3.One));
                    //    ormManager.SaveObject(mat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Cursor3dCellSel";
                    //    settings.Name = "Cursor3dCellSelGm";
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='Cursor3dCellSelMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}



                    //{
                    //    MaterialSettings mat = new MaterialSettings("MountainsMat", "SimpleNoShadow", DefaultRenderLayers.GroundStandingObjects);
                    //    mat.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "MountainsText"));
                    //    ormManager.SaveObject(mat);
                    //}

                    //{
                    //    GraphicModelSettings settings = new GraphicModelSettings();
                    //    settings.Mesh = "Mountains";
                    //    settings.Name = "MountainsGm";
                    //    settings.MaterialAssignments.Add(new MaterialAssignmentsettings("m1", ormManager.FindObjects(typeof(MaterialSettings), "Name='MountainsMat'")[0]));
                    //    ormManager.SaveObject(settings);
                    //}




                    //{
                    //    ormManager.SaveObject(new StringResource("GrassTileDescription", "Adds 2 hp each turn"));
                    //    ormManager.SaveObject(new StringResource("DirtTileDescription", "Does nothing"));
                    //    ormManager.SaveObject(new StringResource("MountainsTileDescription", "Unpassable mountains"));
                    //    ormManager.SaveObject(new StringResource("CastleTileDescription", "Used to build new units"));
                    //    ormManager.SaveObject(new StringResource("GoldMineLv1TileDescription", "Adds 100 gold each turn"));
                    //}

                    //{
                    //    LandTileSettings obj = new LandTileSettings();
                    //    obj.Name = "Grass1Tile";
                    //    obj.DisplayName = "Grass Land";
                    //    obj.Description = ormManager.LoadObject<StringResource>("Name = 'GrassTileDescription'");
                    //    obj.HpIncrement = 2;
                    //    obj.UnitPosition = Vector3.Zero;
                    //    obj.TileIndex = new Point(1, 0);
                    //    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    //    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    //    obj.IsPassable = true;
                    //    obj.Icon = "TileIconLand";
                    //    ormManager.SaveObject(obj);
                    //}

                    //{
                    //    LandTileSettings obj = new LandTileSettings();
                    //    obj.Name = "Dirt1Tile";
                    //    obj.DisplayName = "Dirt Land";
                    //    obj.Description = ormManager.LoadObject<StringResource>("Name = 'DirtTileDescription'");
                    //    obj.HpIncrement = 0;
                    //    obj.UnitPosition = Vector3.Zero;
                    //    obj.TileIndex = new Point(2, 0);
                    //    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    //    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    //    obj.IsPassable = true;
                    //    obj.Icon = "TileIconLand";
                    //    ormManager.SaveObject(obj);
                    //}
                    //{
                    //    LandTileSettings obj = new LandTileSettings();
                    //    obj.Name = "MountainsTile";
                    //    obj.DisplayName = "Mountains";
                    //    obj.Description = ormManager.LoadObject<StringResource>("Name = 'MountainsTileDescription'");
                    //    obj.HpIncrement = 0;
                    //    obj.UnitPosition = Vector3.Zero;
                    //    obj.TileIndex = new Point(1, 0);
                    //    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    //    obj.GraphicModels.Add(ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'MountainsGm'"));
                    //    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    //    obj.IsPassable = false;
                    //    obj.Icon = "TileIconLand";
                    //    ormManager.SaveObject(obj);
                    //}
                    //{
                    //    CastleTileSettings obj = new CastleTileSettings();
                    //    obj.Name = "CastleTile";
                    //    obj.DisplayName = "Castle";
                    //    obj.Description = ormManager.LoadObject<StringResource>("Name = 'CastleTileDescription'");
                    //    obj.HpIncrement = int.MaxValue;
                    //    obj.UnitPosition = new Vector3(-0.4f, -0.4f, 0);
                    //    obj.TileIndex = new Point(4, 0);
                    //    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    //    obj.ActiveGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'CastleActiveGm'");
                    //    obj.InactiveGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'CastleGm'");
                    //    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    //    obj.IsPassable = true;
                    //    obj.Icon = "TileIconCastle";
                    //    ormManager.SaveObject(obj);
                    //}
                    //{
                    //    GoldMineTileSettings obj = new GoldMineTileSettings();
                    //    obj.Name = "GoldMineLv1Tile";
                    //    obj.DisplayName = "Small Gold Mine";
                    //    obj.Description = ormManager.LoadObject<StringResource>("Name = 'GoldMineLv1TileDescription'");
                    //    obj.GoldIncrement = 100;
                    //    obj.HpIncrement = int.MaxValue;
                    //    obj.UnitPosition = new Vector3(0.2f, 0.2f, 0);
                    //    obj.TileIndex = new Point(3, 0);
                    //    obj.WallGraphicModel = ormManager.FindObject(typeof(GraphicModelSettings), "Name = 'Wall1Gm'");
                    //    obj.HexTerrainTileAtlas = ormManager.FindObject(typeof(HexTerrainTileAtlasSettings), "Name = 'HexTerrainTileAtlas1'");
                    //    obj.IsPassable = true;
                    //    obj.Icon = "TileIconDimensionGate";
                    //    ormManager.SaveObject(obj);
                    //}

                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();

                    //    SphereParticleEmitterSettings particleEmitterSettings = new SphereParticleEmitterSettings();
                    //    particleSystemSettings.Name = "UnitDeathParticleSystem";
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);

                    //    particleEmitterSettings.Radius = 0.5f;
                    //    particleEmitterSettings.RelativePosition = new Vector3(0, 0, -0.2f);
                    //    particleEmitterSettings.ParticleLifeDuration = 3f;
                    //    particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(1.0f, 0.3f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(1.0f, 0.3f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.9f), new Vector2(1.0f, 0.3f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.8f, 1), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.8f, 0.08f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(1.6f, 0.3f);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
                    //    particleEmitterSettings.EmissionRateFunction = new TimeFunction(new Vector2(0, 1000), new Vector2(0.2f, 1000), new Vector2(0.3f, 1000));
                    //    particleEmitterSettings.CycleTime = 0.5f;


                    //    particleEmitterSettings.Material = ormManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
                    //    particleEmitterSettings.Name = "Emitter1";

                    //    ormManager.SaveObject(particleSystemSettings);
                    //}



                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
                    //    particleSystemSettings.Name = "FireBallPsys";

                    //    PointParticleEmitterSettings particleEmitterSettings = new PointParticleEmitterSettings();
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);

                    //    particleEmitterSettings.MaxParticleCnt = 200;
                    //    particleEmitterSettings.ParticleLifeDuration = 1f;
                    //    particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
                    //    particleEmitterSettings.ParticleDirectionVariation = MathHelper.ToRadians(360.0f);
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.7f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.9f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.3f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.5f, 0.1f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.3f, 0.9f);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
                    //    particleEmitterSettings.EmissionRateFunction = new TimeFunction(40);
                    //    particleEmitterSettings.CycleTime = 0.5f;



                    //    particleEmitterSettings.Material = ormManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
                    //    particleEmitterSettings.Name = "Emitter1";



                    //    ormManager.SaveObject(particleSystemSettings);

                    //}

                    //{
                    //    ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
                    //    particleSystemSettings.Name = "FireExplosionPsys";

                    //    PointParticleEmitterSettings particleEmitterSettings = new PointParticleEmitterSettings();
                    //    particleSystemSettings.Emitters.Add(particleEmitterSettings);

                    //    particleEmitterSettings.FastForwardTimeOnLoad = 0;
                    //    particleEmitterSettings.MaxParticleCnt = 25;
                    //    particleEmitterSettings.ParticleLifeDuration = 1f;
                    //    particleEmitterSettings.ParticleLifeDurationVariation = 1f;
                    //    particleEmitterSettings.ParticleDirectionVariation = MathHelper.ToRadians(90.0f);
                    //    particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.7f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.9f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.3f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
                    //    particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(1.0f, 0) });
                    //    particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.8f, 0.1f);
                    //    particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.3f, 0.9f);
                    //    particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
                    //    particleEmitterSettings.EmissionRateFunction = new TimeFunction(1000);
                    //    particleEmitterSettings.CycleTime = 0.5f;

                    //    particleEmitterSettings.Material = ormManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
                    //    particleEmitterSettings.Name = "Emitter1";

                    //    ormManager.SaveObject(particleSystemSettings);
                    //}

                    transaction.Commit();
                }
            }
        }
    }
}

