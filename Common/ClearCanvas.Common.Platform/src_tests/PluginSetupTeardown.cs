#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace ClearCanvas.Common.Tests
{
	/// <summary>
	/// Summary description for PluginSetupTeardown.
	/// </summary>
	public class PluginSetupTeardown
	{
		private static string m_ProjectPath;
		private static string m_TestDir;
		private static string m_PluginDir;
		
		private static string m_ClearCanvasCommonPlatformPath;
		private static string m_ClearCanvasViewerCorePath;
		private static string m_ClearCanvasViewerUIPath;
		
		private static string m_ConfigFile1Path;
		private static string m_ConfigFile2Path;
		private static string m_ConfigFile3Path;
		private static string m_ConfigFile4Path;

		public static void PluginSetup()
		{
			InitializePaths();

			// Make dummy plugin directory so PluginManager doesn't complain
			Directory.CreateDirectory(m_PluginDir);
			Platform.InstallDir = m_TestDir;

			CopyConfigFiles();

			// Form list of plugin paths. Load plugins from where they're built
			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(m_ClearCanvasViewerCorePath);
			pluginFileList.Add(m_ClearCanvasViewerUIPath);

			Platform.PluginManager.LoadPlugins(pluginFileList);
		}

		public static void PluginTeardown()
		{
			Directory.Delete(m_TestDir, true);
		}

		private static void InitializePaths()
		{
			m_TestDir = @"c:\test";
			m_PluginDir = m_TestDir + @"\plugins";
			m_ProjectPath = @"C:\VSProjects";
			
			m_ConfigFile1Path = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\App.config";
			m_ConfigFile2Path = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\exceptionhandlingconfiguration.config";
			m_ConfigFile3Path = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingconfiguration.config";
			m_ConfigFile4Path = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\loggingdistributorconfiguration.config";

			m_ClearCanvasCommonPlatformPath = m_ProjectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			m_ClearCanvasViewerCorePath = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			m_ClearCanvasViewerUIPath = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		private static void CopyPluginFiles()
		{
			File.Copy(m_ClearCanvasViewerCorePath, m_PluginDir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(m_ClearCanvasViewerUIPath, m_PluginDir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		private static void CopyConfigFiles()
		{
			File.Copy(m_ConfigFile1Path, Directory.GetCurrentDirectory() + @"\ClearCanvas.Workstation.Model.dll.config", true);
			File.Copy(m_ConfigFile2Path, Directory.GetCurrentDirectory() + @"\exceptionhandlingconfiguration.config", true);
			File.Copy(m_ConfigFile3Path, Directory.GetCurrentDirectory() + @"\loggingconfiguration.config", true);
			File.Copy(m_ConfigFile4Path, Directory.GetCurrentDirectory() + @"\loggingdistributorconfiguration.config", true);
		}

	}
}

#endif