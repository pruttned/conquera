using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;

namespace Conquera
{
    public class GameUnitDescLoader : BaseAssetLoader<GameUnitSttings>
    {
        protected override object CreateDesc(GameUnitSttings settings, ContentGroup contentGroup)
        {
            return new GameUnitDesc(settings, contentGroup);
        }

        protected override string GetName(GameUnitSttings settings)
        {
            return settings.Name;
        }
    }
}
