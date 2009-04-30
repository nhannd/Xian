#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
			CommandLine cmdLine = new CommandLine();
			try
			{
				cmdLine.Parse(args);

				Version persistentStoreVersion = LoadPersistentStoreVersion();
				Version assemblyVersion = LoadAssemblyVersion();

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

			PersistentStoreUpgradeScriptExtensionPoint ep = new PersistentStoreUpgradeScriptExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			bool found = false;
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
			Version currentVersion = startingVersion;

			PersistentStoreUpgradeScriptExtensionPoint ep = new PersistentStoreUpgradeScriptExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			while (!currentVersion.Equals(assemblyVersion))
			{
				bool found = false;
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
							Console.WriteLine("Unexpected Exception when executing upgrade script :");
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
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			return store.Version;
		}
		#endregion
	}
}
