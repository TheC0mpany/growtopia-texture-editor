using System;
using System.IO;
using System.Windows.Forms;

namespace growtopia_texture_editor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create cache directory if it doesn't exist
            if (!Directory.Exists("cache"))
                Directory.CreateDirectory("cache");

            Globals.start = new Start();
            Globals.main = new Form1();

            Application.Run(Globals.start);
        }
    }
}
