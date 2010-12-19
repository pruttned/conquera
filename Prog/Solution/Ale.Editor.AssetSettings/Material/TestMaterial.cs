using Ale.Graphics;
using Ale.Content;

namespace Ale.Editor.AssetSettings
{
    public class TestMaterial : AssetSettingsDefinitionBase<MaterialSettings>
    {
        public override MaterialSettings GetSettings(ContentGroup content)
        {
            return content.ParentContentManager.OrmManager.LoadObjects<MaterialSettings>(string.Format("Name='{0}'", "DomMat"))[0];
        }
    }
}
//asdkjhsakjdhaksj   f
//refresni        


