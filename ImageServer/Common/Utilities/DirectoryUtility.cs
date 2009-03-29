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

        /// <summary>
        /// Moves a study from one location to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Move(string source, string destination)
        {
            Copy(source, destination);
            DeleteIfExists(source);
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            InternalCopy(diSource, diTarget);
        }

        private static void InternalCopy(DirectoryInfo source, DirectoryInfo target)
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
                InternalCopy(diSourceSubDir, nextTargetSubDir);

            }
        }

        public static void DeleteIfExists(string dir)
        {
            DeleteIfExists(dir, false);
        }

        public static bool DeleteIfEmpty(string path)
        {
            if (Directory.Exists(path))
            {
                if (Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0)
                {
                    Directory.Delete(path, true);
                    return true;
                }
                else
                {
                    // not empty
                    return false;
                }
            }

            return true;// not exist = empty 
        }

        public static void DeleteEmptySubDirectories(string path, bool recursive)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    DeleteEmptySubDirectories(subDir.FullName, recursive);
                    DeleteIfEmpty(subDir.FullName);
                }
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
