using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.Editor
{
    public interface ICommand
    {
        void Execute(EditorApplication app);
    }




    public class NewMapInfo
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class NewMapCommand : ICommand
    {
        private NewMapInfo mNewMapInfo;


        public NewMapCommand(NewMapInfo newMapInfo)
        {
            mNewMapInfo = newMapInfo;
        }

        public void Execute(EditorApplication app)
        {
            app.GameScene = new HotseatGameScene(mNewMapInfo.Name, app.SceneManager, mNewMapInfo.Width, mNewMapInfo.Height,
                "Grass1Tile", app.Content.DefaultContentGroup);
        }
    }

}
