#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace ClearCanvas.Common.Tests
{
	public class PluginSetupTeardown
	{
		private static string _ProjectPath;
		private static string _TestDir;
		private static string _PluginDir;
		
		private static string _ClearCanvasCommonPlatformPath;
		private static string _ClearCanvasViewerCorePath;
		private static string _ClearCanvasViewerUIPath;
		
		private static string _ConfigFile1Path;
		private static string _ConfigFile2Path;
		private static string _ConfigFile3Path;
		private static string _ConfigFile4Path;

		public static void PluginSetup()
		{
			InitializePaths();

			// Make dummy plugin directory so PluginManager doesn't complain
			Directory.CreateDirectory(_PluginDir);
			Platform.InstallDir = _TestDir;

			CopyConfigFiles();

			// Form list of plugin paths. Load plugins from where they're built
			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(_ClearCanvasViewerCorePath);
			pluginFileList.Add(_ClearCanvasViewerUIPath);

//TODO			Platform.PluginManager.LoadPlugins(pluginFileList);
		}

		public static void PluginTeardown()
		{
			Directory.Delete(_TestDir, true);
		}

		private static void InitializePaths()
		{
			_TestDir = @"c:\test";
			_PluginDir = _TestDir + @"\plugins";
			_ProjectPath = @"C:\VSProjects";
			
			_ConfigFile1Path = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\App.config";
			_ConfigFile2Path = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\exceptionhandlingconfiguration.config";
			_ConfigFile3Path = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingconfiguration.config";
			_ConfigFile4Path = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingdistributorconfiguration.config";

			_ClearCanvasCommonPlatformPath = _ProjectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			_ClearCanvasViewerCorePath = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			_ClearCanvasViewerUIPath = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		private static void CopyPluginFiles()
		{
			File.Copy(_ClearCanvasViewerCorePath, _PluginDir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(_ClearCanvasViewerUIPath, _PluginDir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		private static void CopyConfigFiles()
		{
			File.Copy(_ConfigFile1Path, Directory.GetCurrentDirectory() + @"\ClearCanvas.Workstation.Model.dll.config", true);
			File.Copy(_ConfigFile2Path, Directory.GetCurrentDirectory() + @"\exceptionhandlingconfiguration.config", true);
			File.Copy(_ConfigFile3Path, Directory.GetCurrentDirectory() + @"\loggingconfiguration.config", true);
			File.Copy(_ConfigFile4Path, Directory.GetCurrentDirectory() + @"\loggingdistributorconfiguration.config", true);
		}

	}
}

#endif