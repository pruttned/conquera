using System;
using Ale.Scene;
using Ale.Content;

namespace Ale.Editor
{
    public interface IRenderInfo
    {
        BaseScene Scene { get; }
        void Update(object assetSettings, SceneManager sceneManager, ContentGroup content);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class RenderInfoAttribute : Attribute
    {
        public string SourceDirectory { get; private set; }

        public RenderInfoAttribute(string sourceDirectory)
        {
            SourceDirectory = sourceDirectory;
        }
    }
}