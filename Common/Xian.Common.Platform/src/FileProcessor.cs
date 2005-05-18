///////////////////////////////////////////////////////////
//
//  FileProcessor.cs
//
//  Created on:      09-Feb-2005 12:00:00 PM
//
//  Original author: Norman Young
//
///////////////////////////////////////////////////////////

using System;
using System.IO;

namespace Xian.Common.Platform
{
	/// <summary>
	/// Summary description for RecursiveFileProcessor.
	/// </summary>
	public class FileProcessor
	{
		// Public methods
		public delegate void ProcessFile(string filePath);

		public static void Process(string path, string searchPattern, FileProcessor.ProcessFile proc, bool recursive)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForNullReference(proc, "proc");

			// If the path is a directory, process its contents
			if (Directory.Exists(path))
			{
				ProcessDirectory(path, searchPattern, proc, recursive);
			}
				// If the path is a file, just process the file
			else if (File.Exists(path))
			{
				proc(path);
			}
			else
			{
				throw new DirectoryNotFoundException(SR.ExceptionPathDoesNotExist(path));
			}
		}

		// Private methods
		private static void ProcessDirectory(string path, string searchPattern, FileProcessor.ProcessFile proc, bool recursive)
		{
			// Process files in this directory
			string[] fileList;
			GetFiles(path, searchPattern, out fileList);

			if (fileList != null)
			{
				foreach (string file in fileList)
					proc(file);
			}

			// If recursive, then descend into lower directories and process those as well
			string[] dirList;
			GetDirectories(path, searchPattern, proc, recursive, out dirList);
		}

		private static void GetFiles(string path, string searchPattern, out string[] fileList)
		{
			fileList = null;

			try
			{
				if (searchPattern == null || searchPattern == String.Empty)
					fileList = Directory.GetFiles(path);
				else
					fileList = Directory.GetFiles(path, searchPattern);
			}
			catch (Exception e)
			{
				Platform.HandleException(e, "LogExceptionPolicy");
			}
		}

		private static void GetDirectories(string path, string searchPattern, FileProcessor.ProcessFile proc, bool recursive, out string[] dirList)
		{
			dirList = null;

			try
			{
				dirList = Directory.GetDirectories(path);
			}
			catch (Exception e)
			{
				Platform.HandleException(e, "LogExceptionPolicy");
			}

			if (recursive)
			{
				foreach (string dir in dirList)
					ProcessDirectory(dir, searchPattern, proc, recursive);
			}
		}
	}
}
