//using Microsoft.Xna.Framework.Content.Pipeline;


//namespace Ale.Content
//{
//    /// <summary>
//    /// Content procesor that transforms AleMaterialContent to the AleCompiledMaterial
//    /// </summary>
//    [ContentProcessor(DisplayName = "Material - Ale")]
//    public class AleMaterialProcessor : ContentProcessor<AleMaterialContent, AleCompiledMaterial>
//    {
//        /// <summary>
//        /// See ContentProcessor
//        /// </summary>
//        /// <param name="input"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public override AleCompiledMaterial Process(AleMaterialContent input, ContentProcessorContext context)
//        {
//          //  System.Diagnostics.Debugger.Launch();
//            return new AleCompiledMaterial(input.XmlContent.SelectSingleNode(@"./material"));
//        }
//    }
//}