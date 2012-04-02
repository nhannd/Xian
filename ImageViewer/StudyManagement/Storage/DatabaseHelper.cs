using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests;

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

        public static void CreateDatabase(string resourceName, string filePath)
        {
            //NOTE: Since we're using CE 4.0, the LINQ CreateDatabase function won't work because it creates a 3.5 database.

            // ensure the parent directory exists before trying to create database
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var resourceResolver = new ResourceResolver(typeof(StudyRootQueryTests).Assembly);
            using (Stream sourceStream = resourceResolver.OpenResource(resourceName))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var buffer = new byte[1024];
                    while (true)
                    {
                        int read = sourceStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            break;

                        fileStream.Write(buffer, 0, read);
                        if (read < buffer.Length)
                            break;
                    }

                    fileStream.Close();
                }
            }

        }

    }
}
