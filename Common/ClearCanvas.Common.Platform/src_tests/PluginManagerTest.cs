#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Tests
{
	[TestFixture]
	public class PluginManagerTest
	{
		private string m_TestDir;
		private string m_RootPluginDir;
		private string m_Plugin1Dir;
		private string m_Plugin2Dir;
		private string m_Plugin3Dir;
		private string m_ProjectPath;
		private string m_ClearCanvasViewerPath;
		private string m_ClearCanvasCommonPlatformPath;
		private string m_ClearCanvasViewerCorePath;
		private string m_ClearCanvasViewerUIPath;

		public PluginManagerTest()
		{
			m_TestDir = @"c:\test";
			m_RootPluginDir = m_TestDir + @"\plugins";
			m_Plugin1Dir = m_RootPluginDir + @"\plugin1";
			m_Plugin2Dir = m_RootPluginDir + @"\plugin2";
			m_Plugin3Dir = m_RootPluginDir + @"\plugin3";
			m_ProjectPath = @"C:\VSProjects";
			m_ClearCanvasViewerPath = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\bin\Debug\ClearCanvas.Workstation.exe";
			m_ClearCanvasCommonPlatformPath = m_ProjectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			m_ClearCanvasViewerCorePath = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			m_ClearCanvasViewerUIPath = m_ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		public void CopyPluginFiles()
		{
			File.Copy(m_ClearCanvasViewerCorePath, m_Plugin1Dir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(m_ClearCanvasViewerUIPath, m_Plugin2Dir + @"\ClearCanvas.Workstation.View.dll", true);
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

			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(m_ClearCanvasViewerCorePath);
			pluginFileList.Add(m_ClearCanvasViewerUIPath);

			pm.LoadPlugins(pluginFileList);

			Assert.IsTrue(pm.NumberOfPlugins == 2);
		}

		[Test]
		[ExpectedException(typeof(PluginException))]
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
		[ExpectedException(typeof(PluginException))]
		public void LoadPlugins_NoPluginsInDirectory()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(PluginException))]
		public void LoadPlugins_MoreThanOneModelPlugin()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(PluginException))]
		public void LoadPlugins_DuplicatePluginName()
		{
			Assert.Fail("Test not written yet");
		}
	}
}

#endif