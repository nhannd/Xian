#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using System.Reflection;
using System.Text.RegularExpressions;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Upgrade;
using ClearCanvas.ImageServer.Enterprise.SqlServer2005;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.UpgradeScripts
{
	public class BaseUpgradeScript : IPersistentStoreUpgradeScript
	{
		private readonly Version _upgradeFromVersion;
		private readonly Version _upgradeToVersion;
		private readonly string _scriptName;
		private readonly bool _upgradeStoredProcs = false;
		public BaseUpgradeScript(Version upgradeFromVersion, Version upgradeToVersion, string scriptName)
		{
			if (upgradeToVersion == null)
			{
				_upgradeToVersion = Assembly.GetExecutingAssembly().GetName().Version;
				_upgradeStoredProcs = true;
			}
			else
				_upgradeToVersion = upgradeToVersion;
			_upgradeFromVersion = upgradeFromVersion;
			_scriptName = scriptName;
		}
		public string GetScript()
		{
			string sql;

			using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), _scriptName))
			{
				if (stream == null)
					throw new ApplicationException("Unable to load script resource (is the script an embedded resource?): " + _scriptName);

				StreamReader reader = new StreamReader(stream);
				sql = reader.ReadToEnd();
				stream.Close();
			}
			return sql;
		}

		public Version SourceVersion
		{
			get {return _upgradeFromVersion;}
		}

		public Version DestinationVersion
		{
			get { return _upgradeToVersion; }
		}

		public void Execute()
		{
			// Wrap the upgrade in a single commit.
			using (
				IUpdateContext updateContext =
					PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				UpdateContext context = updateContext as UpdateContext;
				if (context == null)
				{
					Console.WriteLine("Unexpected error opening connection to the database.");
					throw new ApplicationException("Error opening connection to the database.");
				}

				ExecuteSql(context, GetScript());

				DatabaseVersionUpdateColumns columns = new DatabaseVersionUpdateColumns();
				DatabaseVersionSelectCriteria criteria = new DatabaseVersionSelectCriteria();

				// Build & Revision are switched, due to how we do revisions in the DB table!!
				columns.Build = DestinationVersion.Revision.ToString();
				columns.Revision = DestinationVersion.Build.ToString();
				columns.Minor = DestinationVersion.Minor.ToString();
				columns.Major = DestinationVersion.Major.ToString();

				IDatabaseVersionEntityBroker broker = context.GetBroker<IDatabaseVersionEntityBroker>();
				broker.Update(criteria, columns);

				updateContext.Commit();
			}

			if (_upgradeStoredProcs)
			{
				RunSqlScriptApplication app = new RunSqlScriptApplication();
				app.RunApplication(new string[] {"-storedprocedures"});
			}
			return;
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
						try
						{
							cmd.CommandText = line;
							cmd.CommandType = CommandType.Text;

							cmd.ExecuteNonQuery();
						}
						catch (Exception e)
						{
							Console.WriteLine("Unexpected error with line in upgrade script: {0}", line);
							Console.WriteLine("Error: {0}", e.Message);
							throw;
						}
					}
				}
			}
		}
	}
}