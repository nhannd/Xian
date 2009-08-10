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
			const string scriptFile =
				@"C:\Stewart\Xian\Trunk\Dicom\DataStore\AuxiliaryFiles\CreateTables.clearcanvas.dicom.datastore.ddl";
			const string databaseDirectory = @"C:\Stewart\Xian\Trunk\Dicom\DataStore\AuxiliaryFiles";
			const string databaseFileName = @"empty_viewer.sdf";
			string databaseFile = Path.Combine(databaseDirectory, databaseFileName);
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