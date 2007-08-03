using System;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
	public class FileProcessor
	{
		// Public methods
		public delegate void ProcessFile(string filePath);
		public delegate void ProcessFileCancellable(string filePath, out bool cancel);

		public static void Process(string path, string searchPattern, FileProcessor.ProcessFile proc, bool recursive)
		{
			Process(path, searchPattern,
				delegate(string filePath, out bool cancel)
				{
					cancel = false;
					proc(filePath);
				},
				recursive);
		}

		public static void Process(string path, string searchPattern, FileProcessor.ProcessFileCancellable proc, bool recursive)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForNullReference(proc, "proc");

			bool cancel;

			// If the path is a directory, process its contents
			if (Directory.Exists(path))
			{
				ProcessDirectory(path, searchPattern, proc, recursive, out cancel);
			}
			// If the path is a file, just process the file
			else if (File.Exists(path))
			{
				proc(path, out cancel);
			}
			else
			{
				throw new DirectoryNotFoundException(String.Format(SR.ExceptionPathDoesNotExist, path));
			}
		}

		// Private methods
		private static void ProcessDirectory(string path, string searchPattern, FileProcessor.ProcessFileCancellable proc, bool recursive, out bool cancel)
		{
			cancel = false;

			// Process files in this directory
			string[] fileList;
			GetFiles(path, searchPattern, out fileList);

			if (fileList != null)
			{
				foreach (string file in fileList)
				{
					proc(file, out cancel);
					if (cancel)
						return;
				}
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
				Platform.Log(LogLevel.Warn, e);
				throw e;
			}
		}

		private static void GetDirectories(string path, string searchPattern, FileProcessor.ProcessFileCancellable proc, bool recursive, out string[] dirList)
		{
			dirList = null;

			try
			{
				dirList = Directory.GetDirectories(path);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
				throw e;
			}

			if (recursive)
			{
				bool cancel;

				foreach (string dir in dirList)
				{
					ProcessDirectory(dir, searchPattern, proc, recursive, out cancel);
					if (cancel)
						break;
				}
			}
		}
	}
}
