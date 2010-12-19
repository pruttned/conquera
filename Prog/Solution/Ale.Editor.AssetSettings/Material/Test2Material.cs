using Ale.Graphics;
using Ale.Content;

namespace Ale.Editor.AssetSettings
{
    public class Test2Material : AssetSettingsDefinitionBase<MaterialSettings>
    {
        public override MaterialSettings GetSettings(ContentGroup content)
        {
            return content.ParentContentManager.OrmManager.LoadObjects<MaterialSettings>(string.Format("Name='{0}'", "TreeMat"))[0];
        }
    }
}
//asdkjhsakjdhaksj   f
//refresni        


