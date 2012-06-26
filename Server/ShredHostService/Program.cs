#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
				var groups = SettingsGroupDescriptor.ListInstalledLocalSettingsGroups();
				groups.Add(new SettingsGroupDescriptor(typeof (ShredSettingsMigrator).Assembly.GetType("ClearCanvas.Server.ShredHost.ShredHostServiceSettings")));
				foreach (var group in groups)
					SettingsMigrator.MigrateSharedSettings(group, commandLine.PreviousExeConfigurationFilename);

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