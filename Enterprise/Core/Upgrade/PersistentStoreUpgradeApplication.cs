#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Upgrade
{
	/// <summary>
	/// Class for performing upgrades of a PersistentStore
	/// </summary>
	/// <remarks>
	/// The class searches for plugins that implement the <see cref="IPersistentStoreUpgradeScript"/>
	/// interface to perform upgrades.
	/// </remarks>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class PersistentStoreUpgradeApplication : IApplicationRoot
	{
		#region Public Methods
		public void RunApplication(string[] args)
		{
			var cmdLine = new CommandLine();
			try
			{
				cmdLine.Parse(args);

				var persistentStoreVersion = LoadPersistentStoreVersion();
				var assemblyVersion = LoadAssemblyVersion();

				if (cmdLine.Switches.ContainsKey("check") && cmdLine.Switches["check"])
				{
					CheckPersistentStoreStatus(persistentStoreVersion, assemblyVersion);
					return;
				}

				Console.WriteLine("The current database version is {0} and assembly version is {1}",
								  persistentStoreVersion.ToString(4),
								  assemblyVersion.ToString(4));

				if (persistentStoreVersion.Equals(assemblyVersion))
				{
					Console.WriteLine("Database version is up-to-date.");
					Environment.ExitCode = 0;
					return;
				}

				if (!UpdatePersistentStore(persistentStoreVersion, assemblyVersion))
					Environment.ExitCode = -1;
				else
				{
					Environment.ExitCode = 0;
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
				Environment.ExitCode = -1;
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception when upgrading database: {0}", e.Message);
				Environment.ExitCode = -1;
			}
		}
		#endregion

		#region Private Static Methods
		private static void CheckPersistentStoreStatus(Version databaseVersion, Version assemblyVersion)
		{
			if (databaseVersion.Equals(assemblyVersion))
			{
				Console.WriteLine("The database is up-to-date.");
				Environment.ExitCode = 0;
				return;
			}

			var ep = new PersistentStoreUpgradeScriptExtensionPoint();
			var extensions = ep.CreateExtensions();

			var found = false;
			foreach (IPersistentStoreUpgradeScript script in extensions)
			{
				if (script.SourceVersion.Equals(databaseVersion))
				{
					found = true;
					break;
				}
			}

			if (found)
			{
				Console.WriteLine("The database must be upgraded from {0} to {1}", databaseVersion.ToString(4),
								  assemblyVersion.ToString(4));
				Environment.ExitCode = 1;
			}
			else
			{
				Console.WriteLine("An unsupported version ({0}) is installed which cannot be upgraded from", databaseVersion.ToString(4));
				Environment.ExitCode = -1;
			}
		}

		private static bool UpdatePersistentStore(Version startingVersion, Version assemblyVersion)
		{
			var currentVersion = startingVersion;

			var ep = new PersistentStoreUpgradeScriptExtensionPoint();
			var extensions = ep.CreateExtensions();

			while (!currentVersion.Equals(assemblyVersion))
			{
				var found = false;
				foreach (IPersistentStoreUpgradeScript script in extensions)
				{
					if (script.SourceVersion.Equals(currentVersion))
					{
						try
						{
							script.Execute();
						}
						catch (Exception e)
						{
							Console.WriteLine("Unexpected Exception when executing upgrade script : {0}", e.Message);
							Console.WriteLine("Call stack : {0}", e.StackTrace);
							return false;
						}
						Console.WriteLine("The database has been upgraded from version {0} to version {1}", currentVersion.ToString(4),
						                  script.DestinationVersion.ToString(4));
						currentVersion = script.DestinationVersion;

						found = true;
						break;
					}
				}

				if (!found)
				{
					Console.WriteLine("Unable to find upgrade script for {0}", currentVersion.ToString(4));
					return false;
				}
			}

			return true;
		}

		private static Version LoadAssemblyVersion()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}

		private static Version LoadPersistentStoreVersion()
		{
			var store = PersistentStoreRegistry.GetDefaultStore();
			return store.Version;
		}
		#endregion
	}
}
