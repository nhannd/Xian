using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.Dicom.DataStore
{
    public class FileRemover
    {
        public static void DeleteFilesInStudy(IStudy studyToRemove)
        {
            DeleteSopInstanceFiles(studyToRemove.GetSopInstances());
        }

        public static void DeleteFilesInSeries(ISeries seriesToRemove)
        {
            DeleteSopInstanceFiles(seriesToRemove.GetSopInstances());
        }

        public static void DeleteFileForSopInstance(ISopInstance sopIntanceToDelete)
        {
            if (sopIntanceToDelete.GetLocationUri().IsFile == false)
                return;

            string fileName = sopIntanceToDelete.GetLocationUri().LocalDiskPath;
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private static void DeleteSopInstanceFiles(IEnumerable<ISopInstance> sopInstancesToDelete)
        {
            List<string> directoriesToDelete = new List<string>();
            foreach (ISopInstance sop in sopInstancesToDelete)
            {
                DeleteFileForSopInstance(sop);

                string directoryName = System.IO.Path.GetDirectoryName(sop.GetLocationUri().LocalDiskPath);
                if (directoriesToDelete.Contains(directoryName) == false)
                    directoriesToDelete.Add(directoryName);
            }

            // Recursively delete directories that may be empty
            DeleteEmptyDirectories(directoriesToDelete);
        }

        private static void DeleteEmptyDirectories(List<string> directoriesToDelete)
        {
            if (directoriesToDelete.Count == 0)
                return;

            // Subdirectories will always be longer than parent directories
            // sort in descending order based on directory length
            directoriesToDelete.Sort();
            directoriesToDelete.Reverse();

            List<string> parentDirectoriesToDelete = new List<string>();
            foreach (string directoryName in directoriesToDelete)
            {
                if (Directory.Exists(directoryName))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                    if (directoryInfo.GetFiles("*", SearchOption.AllDirectories).Length <= 0)
                    {
                        Directory.Delete(directoryName, true);

                        parentDirectoriesToDelete.Add(directoryInfo.Parent.FullName);
                    }
                }
            }

            DeleteEmptyDirectories(parentDirectoriesToDelete);
        }
    }
}
