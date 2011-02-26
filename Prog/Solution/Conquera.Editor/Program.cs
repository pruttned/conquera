using System;

namespace Conquera.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
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
}

