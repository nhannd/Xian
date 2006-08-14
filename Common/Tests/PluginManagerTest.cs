#if	UNIT_TESTS

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Tests
{
	[TestFixture]
	public class PluginManagerTest
	{
		private string _testDir;
		private string _rootPluginDir;
		private string _plugin1Dir;
		private string _plugin2Dir;
		private string _plugin3Dir;
		private string _projectPath;
		private string _clearCanvasViewerPath;
		private string _clearCanvasCommonPlatformPath;
		private string _clearCanvasViewerCorePath;
		private string _clearCanvasViewerUIPath;

		public PluginManagerTest()
		{
			_testDir = @"c:\test";
			_rootPluginDir = _testDir + @"\plugins";
			_plugin1Dir = _rootPluginDir + @"\plugin1";
			_plugin2Dir = _rootPluginDir + @"\plugin2";
			_plugin3Dir = _rootPluginDir + @"\plugin3";
			_projectPath = @"C:\VSProjects";
			_clearCanvasViewerPath = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\bin\Debug\ClearCanvas.Workstation.exe";
			_clearCanvasCommonPlatformPath = _projectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			_clearCanvasViewerCorePath = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			_clearCanvasViewerUIPath = _projectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		public void CopyPluginFiles()
		{
			File.Copy(_clearCanvasViewerCorePath, _plugin1Dir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(_clearCanvasViewerUIPath, _plugin2Dir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		[TestFixtureSetUp]
		public void Init()
		{
			// Create plugin directories
			Directory.CreateDirectory(_plugin1Dir);
			Directory.CreateDirectory(_plugin2Dir);
			Directory.CreateDirectory(_plugin3Dir);
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			// Get rid of the test directory
			Directory.Delete(_testDir, true);
		}

		[Test]
		public void LoadPlugins()
		{
			CopyPluginFiles();
			PluginManager pm = new PluginManager(_rootPluginDir);

			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(_clearCanvasViewerCorePath);
			pluginFileList.Add(_clearCanvasViewerUIPath);

//TODO			pm.LoadPlugins(pluginFileList);

			Assert.IsTrue(pm.Plugins.Length == 2);
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