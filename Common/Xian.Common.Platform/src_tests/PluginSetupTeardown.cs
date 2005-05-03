#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;

namespace Xian.Common.Platform.Tests
{
	/// <summary>
	/// Summary description for PluginSetupTeardown.
	/// </summary>
	public class PluginSetupTeardown
	{
		private static string m_ProjectPath;
		private static string m_TestDir;
		private static string m_PluginDir;
		
		private static string m_XianCommonPlatformPath;
		private static string m_XianViewerCorePath;
		private static string m_XianViewerUIPath;
		
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
			ArrayList pluginFileList = new ArrayList();
			pluginFileList.Add(m_XianViewerCorePath);
			pluginFileList.Add(m_XianViewerUIPath);

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
			
			m_ConfigFile1Path = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer\App.config";
			m_ConfigFile2Path = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer\exceptionhandlingconfiguration.config";
			m_ConfigFile3Path = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer\loggingconfiguration.config";
			m_ConfigFile4Path = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer\loggingdistributorconfiguration.config";

			m_XianCommonPlatformPath = m_ProjectPath + @"\Xian\Trunk\Common\Xian.Common.Platform\bin\Debug\Xian.Common.Platform.dll";
			m_XianViewerCorePath = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer.Core\bin\Debug\Xian.Viewer.Core.dll";
			m_XianViewerUIPath = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer.UI\bin\Debug\Xian.Viewer.UI.dll";
		}

		private static void CopyPluginFiles()
		{
			File.Copy(m_XianViewerCorePath, m_PluginDir + @"\Xian.Viewer.Core.dll", true);
			File.Copy(m_XianViewerUIPath, m_PluginDir + @"\Xian.Viewer.UI.dll", true);
		}

		private static void CopyConfigFiles()
		{
			File.Copy(m_ConfigFile1Path, Directory.GetCurrentDirectory() + @"\Xian.Viewer.Core.dll.config", true);
			File.Copy(m_ConfigFile2Path, Directory.GetCurrentDirectory() + @"\exceptionhandlingconfiguration.config", true);
			File.Copy(m_ConfigFile3Path, Directory.GetCurrentDirectory() + @"\loggingconfiguration.config", true);
			File.Copy(m_ConfigFile4Path, Directory.GetCurrentDirectory() + @"\loggingdistributorconfiguration.config", true);
		}

	}
}

#endif