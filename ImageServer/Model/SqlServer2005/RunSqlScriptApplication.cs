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
