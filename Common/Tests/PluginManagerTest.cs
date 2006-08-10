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
		private string _TestDir;
		private string _RootPluginDir;
		private string _Plugin1Dir;
		private string _Plugin2Dir;
		private string _Plugin3Dir;
		private string _ProjectPath;
		private string _ClearCanvasViewerPath;
		private string _ClearCanvasCommonPlatformPath;
		private string _ClearCanvasViewerCorePath;
		private string _ClearCanvasViewerUIPath;

		public PluginManagerTest()
		{
			_TestDir = @"c:\test";
			_RootPluginDir = _TestDir + @"\plugins";
			_Plugin1Dir = _RootPluginDir + @"\plugin1";
			_Plugin2Dir = _RootPluginDir + @"\plugin2";
			_Plugin3Dir = _RootPluginDir + @"\plugin3";
			_ProjectPath = @"C:\VSProjects";
			_ClearCanvasViewerPath = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation\bin\Debug\ClearCanvas.Workstation.exe";
			_ClearCanvasCommonPlatformPath = _ProjectPath + @"\ClearCanvas\Trunk\Common\ClearCanvas.Common\bin\Debug\ClearCanvas.Common.dll";
			_ClearCanvasViewerCorePath = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.Model\bin\Debug\ClearCanvas.Workstation.Model.dll";
			_ClearCanvasViewerUIPath = _ProjectPath + @"\ClearCanvas\Trunk\Viewer\ClearCanvas.Workstation.View\bin\Debug\ClearCanvas.Workstation.View.dll";
		}

		public void CopyPluginFiles()
		{
			File.Copy(_ClearCanvasViewerCorePath, _Plugin1Dir + @"\ClearCanvas.Workstation.Model.dll", true);
			File.Copy(_ClearCanvasViewerUIPath, _Plugin2Dir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		[TestFixtureSetUp]
		public void Init()
		{
			// Create plugin directories
			Directory.CreateDirectory(_Plugin1Dir);
			Directory.CreateDirectory(_Plugin2Dir);
			Directory.CreateDirectory(_Plugin3Dir);
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
			// Get rid of the test directory
			Directory.Delete(_TestDir, true);
		}

		[Test]
		public void LoadPlugins()
		{
			CopyPluginFiles();
			PluginManager pm = new PluginManager(_RootPluginDir);

			StringCollection pluginFileList = new StringCollection();
			pluginFileList.Add(_ClearCanvasViewerCorePath);
			pluginFileList.Add(_ClearCanvasViewerUIPath);

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