#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;

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
            _testDir = Directory.GetCurrentDirectory() + @"..\..\..\..\UnitTestFiles\ClearCanvas.Common.Tests.PluginManagerTest.PluginManagerTest";
			_rootPluginDir = _testDir + @"\plugins";
			_plugin1Dir = _rootPluginDir + @"\plugin1";
			_plugin2Dir = _rootPluginDir + @"\plugin2";
			_plugin3Dir = _rootPluginDir + @"\plugin3";
            _projectPath = Directory.GetCurrentDirectory() + @"\..\..\..\..\..\..";
            _clearCanvasViewerPath = _projectPath + @"\Trunk\Desktop\Executable\bin\Debug\ClearCanvas.Desktop.Executable.exe";
			_clearCanvasCommonPlatformPath = _projectPath + @"\Trunk\Common\bin\Debug\ClearCanvas.Common.dll";
            _clearCanvasViewerCorePath = _projectPath + @"\Trunk\ImageViewer\bin\Debug\ClearCanvas.ImageViewer.dll";
            _clearCanvasViewerUIPath = _projectPath + @"\Trunk\ImageViewer\View\WinForms\bin\Debug\ClearCanvas.ImageViewer.View.WinForms.dll";
		}

		public void CopyPluginFiles()
		{
            File.Copy(_clearCanvasViewerCorePath, _plugin1Dir + @"\ClearCanvas.ImageViewer.dll", true);
            File.Copy(_clearCanvasViewerUIPath, _plugin2Dir + @"\ClearCanvas.ImageViewer.View.WinForms.dll", true);
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

//TODO			pm.EnsurePluginsLoaded(pluginFileList);

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