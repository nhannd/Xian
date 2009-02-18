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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.SqlServer2005;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Upgrade
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class ModelUpgradeApplication : IApplicationRoot
	{
		public void RunApplication(string[] args)
		{
			ModelUpgradeCommandLine cmdLine = new ModelUpgradeCommandLine();
			try
			{
				cmdLine.Parse(args);

				DatabaseVersion databaseVersion = LoadDatabaseVersion();
				DatabaseVersion assemblyVersion = LoadAssemblyVersion();

				Console.WriteLine("The current database version is {0} and assembly version is {1}", databaseVersion.GetVersionString(),
				                  assemblyVersion.GetVersionString());

				if (databaseVersion.Equals(assemblyVersion))
				{
					Console.WriteLine("Database version is already correct.  Exiting upgrade application.");
					return;
				}

				if (!UpgradeDatabase(databaseVersion, assemblyVersion))
					Environment.ExitCode = -1;
				else
					Environment.ExitCode = 0;

			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
				Environment.ExitCode = -1;
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception when upgrading database: {0}",e.Message);
				Environment.ExitCode = -1;	
			}
		}

		bool UpgradeDatabase(DatabaseVersion startingVersion, DatabaseVersion assemblyVersion)
		{
			DatabaseVersion currentVersion = startingVersion;
			
			UpgradeScriptExtensionPoint ep = new UpgradeScriptExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			while (!currentVersion.Equals(assemblyVersion))
			{
				bool found = false;
				foreach (IUpgradeScript script in extensions)
				{
					if (script.UpgradeFromVersion.Equals(currentVersion))
					{
						if (!UpgradeVersion(script, assemblyVersion))
						{
							return false;
						}

						if (script.UpgradeToVersion == null)
						{
							Console.WriteLine("The database has been upgraded from version {0} to version {1}", currentVersion.GetVersionString(),
							  assemblyVersion.GetVersionString());
							currentVersion = assemblyVersion;
						}
						else
						{
							Console.WriteLine("The database has been upgraded from version {0} to version {1}", currentVersion.GetVersionString(),
							  script.UpgradeToVersion.GetVersionString());
							currentVersion = script.UpgradeToVersion;
						}

						found = true;
						break;
					}
				}
				if (!found)
				{
					Console.WriteLine("Unable to find upgrade script for {0}", currentVersion.GetVersionString());
					return false;
				}
			}

			return true;
		}

		bool UpgradeVersion(IUpgradeScript script, DatabaseVersion assemblyVersion)
		{
			using (
				IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateContext context = updateContext as UpdateContext;
				if (context == null)
				{
					Console.WriteLine("Unexpected error opening connection to the database.");
					return false;
				}

				ExecuteSql(context, script);

				DatabaseVersionUpdateColumns columns = new DatabaseVersionUpdateColumns();
				DatabaseVersionSelectCriteria criteria = new DatabaseVersionSelectCriteria();

				if (script.UpgradeToVersion == null)
				{
					columns.Build = assemblyVersion.Build;
					columns.Revision = assemblyVersion.Revision;
					columns.Minor = assemblyVersion.Minor;
					columns.Major = assemblyVersion.Major;
				}
				else
				{
					columns.Build = script.UpgradeToVersion.Build;
					columns.Revision = script.UpgradeToVersion.Revision;
					columns.Minor = script.UpgradeToVersion.Minor;
					columns.Major = script.UpgradeToVersion.Major;
				}

				IDatabaseVersionEntityBroker broker = context.GetBroker<IDatabaseVersionEntityBroker>();
				broker.Update(criteria, columns);

				updateContext.Commit();
			}
			return true;
		}

		static DatabaseVersion LoadAssemblyVersion()
		{
			Version ver = Assembly.GetExecutingAssembly().GetName().Version;

			DatabaseVersion assemblyVersion = new DatabaseVersion(ver.Build.ToString(),
				ver.Major.ToString(),
				ver.Minor.ToString(),
				ver.Revision.ToString());
			return assemblyVersion;
		}

		static DatabaseVersion LoadDatabaseVersion()
		{
			using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IDatabaseVersionEntityBroker broker = read.GetBroker<IDatabaseVersionEntityBroker>();
				DatabaseVersionSelectCriteria criteria = new DatabaseVersionSelectCriteria();
				criteria.Major.SortDesc(0);
				criteria.Minor.SortDesc(1);
				criteria.Revision.SortDesc(2);
				criteria.Build.SortDesc(3);

				IList<DatabaseVersion> versions = broker.Find(criteria);
				if (versions.Count == 0)
					return null;

				return CollectionUtils.FirstElement(versions);
			}
		}

		public void ExecuteSql(UpdateContext context, IUpgradeScript script)
		{
			string sql = script.GetScript();

			Regex regex = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			string[] lines = regex.Split(sql);

			using (SqlCommand cmd = context.Connection.CreateCommand())
			{
				cmd.Connection = context.Connection;
				cmd.Transaction = context.Transaction;

				foreach (string line in lines)
				{
					if (line.Length > 0)
					{
						cmd.CommandText = line;
						cmd.CommandType = CommandType.Text;

						cmd.ExecuteNonQuery();
					}
				}
			}
		}
	}
}
