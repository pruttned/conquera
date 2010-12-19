using System;
using System.Collections.Generic;
using System.Text;
using Ale.Input;
using Ale;

namespace Conquera
{
    public interface IGameSceneState
    {
        void OnStart();
        void OnEnd();
        void OnClickOnCell(HexCell cellUnderCur, MouseButton button);
        void Update(AleGameTime gameTime);
    }

}
