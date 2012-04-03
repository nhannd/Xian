using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    internal class DatabaseHelper
    {
        public static string GetDatabaseDirectory()
        {
            return Platform.ApplicationDataDirectory;
        }

        public static string GetDatabaseFilePath(string fileName)
        {
            return Path.Combine(Platform.ApplicationDataDirectory, fileName);
        }

        public static void CreateDatabase(string resourceName, string databaseFilePath)
        {
            //NOTE: Since we're using CE 4.0, the LINQ CreateDatabase function won't work because it creates a 3.5 database.

            // ensure the parent directory exists before trying to create database
            Directory.CreateDirectory(Path.GetDirectoryName(databaseFilePath));

            var resourceResolver = new ResourceResolver(typeof(DatabaseHelper).Assembly);
            using (Stream resourceStream = resourceResolver.OpenResource(resourceName))
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
    }
}
