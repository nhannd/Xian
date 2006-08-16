#if	UNIT_TESTS

using System;
using System.IO;
using ClearCanvas.Common;
using NUnit.Framework;

namespace ClearCanvas.Common.Tests
{
	[TestFixture]
	public class FileProcessorTest
	{
		// The delegate
		private FileProcessor.ProcessFile _del;

		// Root test directory
		private string _testDir = Directory.GetCurrentDirectory() + @"..\..\..\..\..\UnitTestFiles\ClearCanvas.Common.Tests.FileProcessorTest";

		// The delgate function
		static void PrintPath(string path)
		{
			System.Console.WriteLine(path);
		}

		public FileProcessorTest()
		{
		}

		public string[] CreateFiles(string path, string extension, int numFiles)
		{
			string file;
			string[] fileList = new string[numFiles];
			FileStream stream;
			
			for (int i = 0; i < numFiles; i++)
			{
				file = String.Format("{0}\\File{1}{2}", path, i, extension);
				fileList[i] = file;
				stream = File.Create(file);
				// Close the file so we don't have a problem deleting the directory later
				stream.Close();
			}

			return fileList;
		}

		public string[] CreateDirectories(string path, int numDirs)
		{
			string dir;
			string[] dirList= new string[numDirs];
			
			for (int i = 0; i < numDirs; i++)
			{
				dir = String.Format("{0}\\Dir{1}", path, i);
				dirList[i] = dir;
				Directory.CreateDirectory(dir);
			}

			return dirList;
		}

		[TestFixtureSetUp]
		public void Init()
		{
			// Assign the delegate
			_del = new FileProcessor.ProcessFile(PrintPath);

			// Delete the old test directory, if it somehow didn't get deleted on teardown
			if (Directory.Exists(_testDir))
				Directory.Delete(_testDir, true);

			// Create the new test directory
			Directory.CreateDirectory(_testDir);
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			// Get rid of the test directory
			Directory.Delete(_testDir, true);
		}

		[Test]
		public void Process_EmptyDirectory()
		{
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void Process_DirectoryWithFilesOnly()
		{
			CreateFiles(_testDir, "", 10);
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void Process_DirectoryWithSubdirectoriesOnly()
		{
			CreateDirectories(_testDir, 3);
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void Process_DirectoryWithFileAndSubdirectories()
		{
			string[] dirList = CreateDirectories(_testDir, 3);
			CreateFiles(_testDir, "", 5);
			CreateFiles(dirList[0], "",  6);

			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void Process_FileOnly()
		{
			string[] fileList = CreateFiles(_testDir, "", 1);

			FileProcessor.Process(fileList[0], "", _del, true);
		}

		[Test]
		public void Process_Wildcards()
		{
			CreateFiles(_testDir, ".txt", 5);
			CreateFiles(_testDir, ".abc", 5);
			
			FileProcessor.Process(_testDir, "*.abc", _del, true);
		}
		
		[Test]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void Process_PathDoesNotExist()
		{
			FileProcessor.Process("c:\\NoSuchPath", "", _del, true);
		}

		[Test]
		public void Process_BadDelegate()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Process_PathEmpty()
		{
			FileProcessor.Process("", "", _del, true);
		}

		[Test]
		public void Process_InsufficientPermissions()
		{
			Assert.Fail("Test not written yet");
		}

		[Test]
		public void Process_PathTooLong()
		{
			Assert.Fail("Test not written yet");
		}
	}
}

#endif