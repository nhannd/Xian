using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public static class SqlCeDatabaseHelper<TContext>
	{
		public static void CreateDatabase(string fileName)
		{
			//NOTE: Since we're using CE 4.0, the LINQ CreateDatabase function won't work because it creates a 3.5 database.
			var databaseFilePath = GetDatabaseFilePath(fileName);

			// ensure the parent directory exists before trying to create database
			Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath));

			var resourceResolver = new ResourceResolver(typeof(TContext).Assembly);
			using (Stream resourceStream = resourceResolver.OpenResource(fileName))
			{
				using (var fileStream = new FileStream(databaseFilePath, FileMode.Create))
				{
					var buffer = new byte[1024];
					int bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
					// write the required bytes
					while (bytesRead > 0)
					{
						fileStream.Write(buffer, 0, bytesRead);
						bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
					}

					fileStream.Close();
				}

				resourceStream.Close();
			}
		}

		public static IDbConnection CreateConnection(string fileName)
		{
			string filePath = GetDatabaseFilePath(fileName);
			var connectString = string.Format("Data Source = {0}; Default Lock Timeout = 10000", filePath);

			if (!File.Exists(filePath))
				CreateDatabase(fileName);

			// now we can create a long-lived connection
			var connection = new SqlCeConnection(connectString);
			connection.Open();
			return connection;
		}

		private static string GetDatabaseFilePath(string fileName)
		{
			return Path.Combine(Platform.ApplicationDataDirectory, fileName);
		}

	}
}
