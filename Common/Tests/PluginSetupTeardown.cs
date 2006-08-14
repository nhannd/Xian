#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace ClearCanvas.Common.Tests
{
	public class PluginSetupTeardown
	{
		private static string _projectPath;
		private static string _testDir;
		private static string _pluginDir;
		
		private static string _clearCanvasCommonPlatformPath;
		private static string _clearCanvasViewerCorePath;
		private static string _clearCanvasViewerUIPath;
		
		private static string _configFile1Path;
		private static string _configFile2Path;
		private static string _configFile3Path;
		private static string _configFile4Path;

		public static void PluginSetup()
		{
			InitializePaths();

			// Make dummy plugin directory so PluginManager doesn't complain
			Directory.CreateDirectory(_pluginDir);
			Platform.InstallDir = _testDir;

			CopyConfigFiles();

			// Form list of plugin paths. Load plugins from where they're built
			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(_clearCanvasViewerCorePath);
			pluginFileList.Add(_clearCanvasViewerUIPath);

//TODO			Platform.PluginManager.LoadPlugins(pluginFileList);
		}

		public static void PluginTeardown()
		{
			Directory.Delete(_testDir, true);
		}

		private static void InitializePaths()
		{
			_testDir = @"c:\test";
			_pluginDir = _testDir + @"\plugins";
			_projectPath = @"C:\VSProjects";
			
			_configFile1Path = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\App.config";
			_configFile2Path = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\exceptionhandlingconfiguration.config";
			_configFile3Path = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingconfiguration.config";
			_configFile4Path = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingdistributorconfiguration.config";

			_clearCanvasCommonPlatformPath = _projectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			_clearCanvasViewerCorePath = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			_clearCanvasViewerUIPath = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		private static void CopyPluginFiles()
		{
			File.Copy(_clearCanvasViewerCorePath, _pluginDir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(_clearCanvasViewerUIPath, _pluginDir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		private static void CopyConfigFiles()
		{
			File.Copy(_configFile1Path, Directory.GetCurrentDirectory() + @"\ClearCanvas.Workstation.Model.dll.config", true);
			File.Copy(_configFile2Path, Directory.GetCurrentDirectory() + @"\exceptionhandlingconfiguration.config", true);
			File.Copy(_configFile3Path, Directory.GetCurrentDirectory() + @"\loggingconfiguration.config", true);
			File.Copy(_configFile4Path, Directory.GetCurrentDirectory() + @"\loggingdistributorconfiguration.config", true);
		}

	}
}

#endif