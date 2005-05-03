#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;
using Xian.Common.Platform;

namespace Xian.Common.Platform.Tests
{
	/// <summary>
	/// Summary description for PluginManagerTest.
	/// </summary>
	[TestFixture]
	public class PluginManagerTest
	{
		private string m_TestDir;
		private string m_RootPluginDir;
		private string m_Plugin1Dir;
		private string m_Plugin2Dir;
		private string m_Plugin3Dir;
		private string m_ProjectPath;
		private string m_XianViewerPath;
		private string m_XianCommonPlatformPath;
		private string m_XianViewerCorePath;
		private string m_XianViewerUIPath;

		public PluginManagerTest()
		{
			m_TestDir = @"c:\test";
			m_RootPluginDir = m_TestDir + @"\plugins";
			m_Plugin1Dir = m_RootPluginDir + @"\plugin1";
			m_Plugin2Dir = m_RootPluginDir + @"\plugin2";
			m_Plugin3Dir = m_RootPluginDir + @"\plugin3";
			m_ProjectPath = @"C:\VSProjects";
			m_XianViewerPath = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer\bin\Debug\Xian.Viewer.exe";
			m_XianCommonPlatformPath = m_ProjectPath + @"\Xian\Trunk\Common\Xian.Common.Platform\bin\Debug\Xian.Common.Platform.dll";
			m_XianViewerCorePath = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer.Core\bin\Debug\Xian.Viewer.Core.dll";
			m_XianViewerUIPath = m_ProjectPath + @"\Xian\Trunk\Viewer\Xian.Viewer.UI\bin\Debug\Xian.Viewer.UI.dll";
		}

		public void CopyPluginFiles()
		{
			File.Copy(m_XianViewerCorePath, m_Plugin1Dir + @"\Xian.Viewer.Core.dll", true);
			File.Copy(m_XianViewerUIPath, m_Plugin2Dir + @"\Xian.Viewer.UI.dll", true);
		}

		[TestFixtureSetUp]
		public void Init()
		{
			// Create plugin directories
			Directory.CreateDirectory(m_Plugin1Dir);
			Directory.CreateDirectory(m_Plugin2Dir);
			Directory.CreateDirectory(m_Plugin3Dir);
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			// Get rid of the test directory
			Directory.Delete(m_TestDir, true);
		}

		[Test]
		public void LoadPlugins()
		{
			CopyPluginFiles();
			PluginManager pm = new PluginManager(m_RootPluginDir);

			ArrayList pluginFileList = new ArrayList();
			pluginFileList.Add(m_XianViewerCorePath);
			pluginFileList.Add(m_XianViewerUIPath);

			pm.LoadPlugins(pluginFileList);

			Assert.IsTrue(pm.Count == 2);
		}

		[Test]
		[ExpectedException(typeof(PluginWarningException))]
		public void LoadPlugins_DuplicatePluginsInDirectory()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		public void LoadPlugins_NonAssemblyDLLsInDirectory()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(PluginErrorException))]
		public void LoadPlugins_NoPluginsInDirectory()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(PluginWarningException))]
		public void LoadPlugins_MoreThanOneModelPlugin()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(PluginWarningException))]
		public void LoadPlugins_DuplicatePluginName()
		{
			Assert.Fail("Test not written yet");
		}
	}
}

#endif