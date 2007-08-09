using System;
using ClearCanvas.Common;
#if !MONO
using ClearCanvas.Controls.WinForms;
#endif

namespace ClearCanvas.Desktop.Executable
{
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
#if !MONO
			SplashScreenManager.DisplaySplashScreen();
#endif
			Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(OnPluginProgress);

			// check for command line arguments
            if (args.Length > 0)
            {
                // for the sake of simplicity, this is a naive implementation (probably needs to change in future)
                // if there is > 0 arguments, assume the first argument is a class name
                // and bundle the subsequent arguments into a secondary array which is 
                // forwarded to the application root class
                string[] args1 = new string[args.Length - 1];
                Array.Copy(args, 1, args1, 0, args1.Length);

                Platform.StartApp(new ClassNameExtensionFilter(args[0]), args1);
            }
            else
            {
                Platform.StartApp();
            }
		}

		private static void OnPluginProgress(object sender, PluginLoadedEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
#if !MONO
			SplashScreenManager.SetStatus(e.Message);
#endif
        }
	}
}
