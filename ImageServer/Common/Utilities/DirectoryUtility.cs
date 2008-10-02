using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class DirectoryUtility
    {
        public static float CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(folder))
                            folderSize += CalculateFolderSize(dir);
                    }
                    catch (NotSupportedException)
                    {

                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            return folderSize;
        }

        public static void Move(string sourceDirectory, string targetDirectory)
        {
            Copy(sourceDirectory, targetDirectory);
            DeleteIfExists(sourceDirectory);
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);

            }
        }

        public static void DeleteIfExists(string dir)
        {
            DeleteIfExists(dir, false);
        }

        public static void DeleteIfEmpty(string path)
        {
            if (Directory.Exists(path))
            {
                if (Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0)
                    Directory.Delete(path, true);
            }
        }

        public static void DeleteIfExists(string dir, bool deleteParentIfEmpty)
        {
            DirectoryInfo parent = Directory.GetParent(dir);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            if (deleteParentIfEmpty)
            {
                // delete the parent too
                DeleteIfEmpty(parent.FullName);
            }
            
            
        }


    }
    
}
