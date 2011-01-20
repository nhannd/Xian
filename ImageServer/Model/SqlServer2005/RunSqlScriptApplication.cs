#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.SqlServer2005;

namespace ClearCanvas.ImageServer.Model.SqlServer2005
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class RunSqlScriptApplication : IApplicationRoot
	{
		#region Public Methods
		public void RunApplication(string[] args)
		{
			CommandLine cmdLine = new CommandLine();
			try
			{
				cmdLine.Parse(args);

				if (cmdLine.Switches.ContainsKey("storedprocedures") && cmdLine.Switches["storedprocedures"])
				{
					Console.WriteLine("Upgrading the stored procedures.");
					if (!RunEmbeddedScript("ClearCanvas.ImageServer.Model.SqlServer2005.Scripts.ImageServerStoredProcedures.sql"))
						Environment.ExitCode = -1;
					else
						Environment.ExitCode = 0;
				}

				if (cmdLine.Switches.ContainsKey("defaultdata") && cmdLine.Switches["defaultdata"])
				{
					Console.WriteLine("Upgrading the stored procedures.");
					if (!RunEmbeddedScript("ClearCanvas.ImageServer.Model.SqlServer2005.Scripts.ImageServerDefaultData.sql"))
						Environment.ExitCode = -1;
					else
						Environment.ExitCode = 0;
				}

				foreach (string script in cmdLine.Positional)
				{
					if (!RunScript(script))
					{
						Console.WriteLine("Upgrading to execute script: {0}", script);
						Environment.ExitCode = -1;
						return;
					}
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
				Console.WriteLine("Unexpected exception when executing script: {0}", e.Message);
				Environment.ExitCode = -1;
			}
		}
		#endregion

		static bool RunScript(string scriptName)
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

				string sql;

				using (Stream stream = File.OpenRead(scriptName))
				{
					if (stream == null)
					{
						Console.WriteLine("Unable to load script: {0}", scriptName);
						return false;
					}
					StreamReader reader = new StreamReader(stream);
					sql = reader.ReadToEnd();
					stream.Close();
				}

				ExecuteSql(context, sql);

				updateContext.Commit();
			}
			return true;
		}

		bool RunEmbeddedScript(string embeddedScriptName)
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


				string sql;

				using (Stream stream = GetType().Assembly.GetManifestResourceStream(embeddedScriptName))
				{
					if (stream == null)
					{
						Console.WriteLine("Unable to load embedded script: {0}", embeddedScriptName);
						return false;
					}
					StreamReader reader = new StreamReader(stream);
					sql = reader.ReadToEnd();
					stream.Close();
				}

				ExecuteSql(context, sql);

				updateContext.Commit();
			}
			return true;
		}

		private static void ExecuteSql(UpdateContext context, string rawSqlText)
		{
			Regex regex = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			string[] lines = regex.Split(rawSqlText);

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
