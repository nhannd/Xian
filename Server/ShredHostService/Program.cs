#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.ServiceProcess;
using System;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHostService
{
    static class Program
    {
		private class CommandLine : Common.Utilities.CommandLine
		{
			public CommandLine(string[] args)
				: base(args)
			{}

			[CommandLineParameter("service", "s", "Instructs the application that it is to run as a service.", Required = false)]
			public bool RunAsService { get; set; }

			[CommandLineParameter("migrate", "m", "Migrates settings from a previous version of the application, given the previous config filename.", Required = false)]
			public string PreviousExeConfigurationFilename { get; set; }
		}

    	/// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
			var commandLine = new CommandLine(args);

			if (commandLine.RunAsService)
			{
				var ServicesToRun = new ServiceBase[] { new ShredHostService() };
				ServiceBase.Run(ServicesToRun);
			}
			else if (!String.IsNullOrEmpty(commandLine.PreviousExeConfigurationFilename))
			{
				foreach (var settingsGroup in SettingsGroupDescriptor.ListInstalledSettingsGroups(false))
					SettingsMigrator.MigrateSharedSettings(settingsGroup, commandLine.PreviousExeConfigurationFilename);

				ShredSettingsMigrator.MigrateAll(commandLine.PreviousExeConfigurationFilename);
			}
			else
			{
				ShredHostService.InternalStart();
				Console.WriteLine("Press <Enter> to terminate the ShredHost.");
				Console.WriteLine();
				Console.ReadLine();
				ShredHostService.InternalStop();
			}
        }
    }
}