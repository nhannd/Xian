using System;
using ClearCanvas.Common;
#if !MONO
using ClearCanvas.Controls.WinForms;
#endif

namespace ClearCanvas.Workstation.Executable
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Viewer
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
#if !MONO
			SplashScreen.ShowSplashScreen();
#endif
			Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(OnPluginProgress);
			Platform.StartApp();
		}

		private static void OnPluginProgress(object sender, PluginLoadedEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
#if !MONO
			SplashScreen.SetStatus(e.Message);
#endif
		}
	}
}
