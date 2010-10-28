#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data.SqlServerCe;
using ClearCanvas.Common;
using System.IO;

namespace ClearCanvas.Dicom.DataStore.SetupApplication
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class Application : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			// example command line args: 
			// ClearCanvas.Dicom.DataStore.SetupApplication.Application "<TrunkPath>\Dicom\DataStore\AuxiliaryFiles\empty_viewer.sdf" "<TrunkPath>\Dicom\DataStore\AuxiliaryFiles\CreateTables.clearcanvas.dicom.datastore.ddl"

			string databaseFile = args[0];
			string scriptFile = args[1];

			File.Delete(databaseFile);

			string connectionString = String.Format("Data Source=\"{0}\"", databaseFile);

			SqlCeEngine engine = new SqlCeEngine(connectionString);
			engine.CreateDatabase();
			engine.Dispose();

			StreamReader reader = new StreamReader(scriptFile);
			string scriptText = reader.ReadToEnd();
			reader.Close();

			SqlCeConnection connection = new SqlCeConnection(connectionString);
			connection.Open();

			SqlCeTransaction transaction = connection.BeginTransaction();
			SqlCeCommand command = new SqlCeCommand();
			command.Connection = connection;
			command.CommandText = scriptText;
			command.ExecuteNonQuery();

			transaction.Commit();
			connection.Close();

		}

		#endregion
	}
}