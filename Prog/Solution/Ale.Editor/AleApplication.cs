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

using Ale.Scene;
using System;
using System.IO;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using Ale.Gui;

namespace Ale.Editor
{
    public class AleApplication : BaseApplication
    {
        private Dictionary<FileSystemWatcher, IRenderInfo> mRenderInfos = new Dictionary<FileSystemWatcher, IRenderInfo>();
        private CSharpCodeProvider mCodeDomProvider;
        private CompilerParameters mCompilerParameters;
        private string mChangedFileName;
        private IRenderInfo mRenderInfo;
        private bool mSourceFilesChanged = false;

        protected override string GuiPaletteName
        {
            get { return "PaletteDef"; }
        }

        public AleApplication(AleRenderControl renderControl)
            : base(renderControl)
        {
        }

        public AleApplication()
            : base()
        {
            base.GraphicsDeviceManager.IsFullScreen = false;

            //Initializing editor infos and watchers for source files.
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IRenderInfo).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    object[] attributes = type.GetCustomAttributes(typeof(RenderInfoAttribute), false);

                    if (attributes.Length > 0)
                    {
                        string sourceDirectory = ((RenderInfoAttribute)attributes[0]).SourceDirectory;
                        IRenderInfo renderInfo = (IRenderInfo)Activator.CreateInstance(type);

                        FileSystemWatcher watcher = new FileSystemWatcher(Path.Combine(Program.AssetSettingsSourceRootDirectory, sourceDirectory));
                        watcher.IncludeSubdirectories = true;
                        watcher.NotifyFilter = NotifyFilters.LastWrite;
                        watcher.Filter = "*.cs";
                        watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                        watcher.EnableRaisingEvents = true;
                        mRenderInfos.Add(watcher, renderInfo);
                    }
                }
            }

            //Initializing code dom provider.
            mCodeDomProvider = new CSharpCodeProvider();

            mCompilerParameters = new CompilerParameters();
            mCompilerParameters.GenerateExecutable = false;
            mCompilerParameters.GenerateInMemory = true;
            mCompilerParameters.IncludeDebugInformation = true;

            mCompilerParameters.ReferencedAssemblies.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ale.Editor.exe"));
            mCompilerParameters.ReferencedAssemblies.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SimpleOrmFramework.dll"));
            foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                mCompilerParameters.ReferencedAssemblies.Add(Assembly.Load(name).Location);
            }

            ApplicationFocusChanged += new BaseApplication.ApplicationFocusChangedHandler(mApplication_ApplicationFocusChanged);
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            return new DefaultScene(sceneManager, Content.DefaultContentGroup);
        }


        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                mChangedFileName = Path.GetFileNameWithoutExtension(e.Name);
                mRenderInfo = mRenderInfos[(FileSystemWatcher)sender];
                mSourceFilesChanged = true;
            }
        }

        private void mApplication_ApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus && mSourceFilesChanged)
            {
                try
                {
                    Assembly assembly = CompileAssetSettings();
                    Type settingsDefinitionType = assembly.GetType("Ale.Editor.AssetSettings." + mChangedFileName, true);
                    IAssetSettingsDefinition settingsDefinition = (IAssetSettingsDefinition)Activator.CreateInstance(settingsDefinitionType);
                    object assetSettings = settingsDefinition.GetSettingsAsObject(Content.DefaultContentGroup);

                    mRenderInfo.Update(assetSettings, SceneManager, Content.DefaultContentGroup);
                    SceneManager.ActivateScene(mRenderInfo.Scene);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                mSourceFilesChanged = false;
            }
        }

        private Assembly CompileAssetSettings()
        {
            string[] sourceFiles = Directory.GetFiles(Program.AssetSettingsSourceRootDirectory, "*.cs", SearchOption.AllDirectories);
            CompilerResults results = mCodeDomProvider.CompileAssemblyFromFile(mCompilerParameters, sourceFiles);

            if (results.Errors.HasErrors)
            {
                StringBuilder strb = new StringBuilder("Compilation errors:\n");
                foreach (var error in results.Errors)
                {
                    strb.AppendLine(error.ToString());
                }
                throw new Exception(strb.ToString());
            }

            return results.CompiledAssembly;
        }

        protected override void Draw(AleGameTime gameTime)
        {
            base.Draw(gameTime);            
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SceneManager.MouseManager.ClipRealCursor = false;
        }
    }
}
