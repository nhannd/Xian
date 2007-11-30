#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
#pragma warning disable 1591, 219, 169

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
			//Platform.InstallDir = _testDir;

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
            _testDir = Directory.GetCurrentDirectory() + @"..\..\..\..\..\UnitTestFiles\ClearCanvas.Common.Tests.PluginSetupTeardown.InitializePaths";
			_pluginDir = _testDir + @"\plugins";
            _projectPath = Directory.GetCurrentDirectory() + @"\..\..\..\..";
			
			_configFile1Path = _projectPath + @"\Trunk\Desktop\Executable\App.config";
			//_configFile2Path = _projectPath + @"\Trunk\Desktop\Executable\exceptionhandlingconfiguration.config";
			_configFile3Path = _projectPath + @"\Trunk\Desktop\Executable\logging.config";
			//_configFile4Path = _projectPath + @"\Trunk\Desktop\Executable\loggingdistributorconfiguration.config";

            _clearCanvasCommonPlatformPath = _projectPath + @"\Trunk\Common\bin\Debug\ClearCanvas.Common.dll";
            _clearCanvasViewerCorePath = _projectPath + @"\Trunk\ImageViewer\bin\Debug\ClearCanvas.ImageViewer.dll";
            _clearCanvasViewerUIPath = _projectPath + @"\Trunk\ImageViewer\View\WinForms\bin\Debug\ClearCanvas.ImageViewer.View.WinForms.dll";
		}

		private static void CopyPluginFiles()
		{
            File.Copy(_clearCanvasViewerCorePath, _pluginDir + @"\ClearCanvas.Workstation.Model.dll", true);
            File.Copy(_clearCanvasViewerUIPath, _pluginDir + @"\ClearCanvas.Workstation.View.dll", true);
		}

		private static void CopyConfigFiles()
		{
            //HH - 15/08/06 - Not required since the build already copies config files to Desktop\Executable\bin\Debug\
            
            //File.Copy(_configFile1Path, Directory.GetCurrentDirectory() + @"\ClearCanvas.Workstation.Model.dll.config", true);
            //File.Copy(_configFile2Path, Directory.GetCurrentDirectory() + @"\exceptionhandlingconfiguration.config", true);
            //File.Copy(_configFile3Path, Directory.GetCurrentDirectory() + @"\loggingconfiguration.config", true);
            //File.Copy(_configFile4Path, Directory.GetCurrentDirectory() + @"\loggingdistributorconfiguration.config", true);
		}

	}
}

#endif