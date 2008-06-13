using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class DirectoryUtility
    {

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

        public static void DeleteIfExists(string dir, bool deleteParent)
        {
            DirectoryInfo parent = Directory.GetParent(dir);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            if (deleteParent)
            {
                // delete the parent too
                if (parent.GetFiles().Length == 0 && parent.GetDirectories().Length == 0)
                    parent.Delete(true);
            }
            
            
        }


    }
    
}
