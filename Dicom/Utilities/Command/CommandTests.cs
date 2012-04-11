#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587, 649

using System.IO;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Command.Tests
{
    internal class TestContext : ICommandProcessorContext
    {
        public TestContext()
        {
            BackupDirectory = Path.GetTempPath();
        }

        public void Dispose()
        {
            
        }

        public void PreExecute(ICommand command)
        {
            CommandsExecuted++;
        }

        public void Commit()
        {
            RollbackEncountered = true;
        }

        public void Rollback()
        {
            RollbackEncountered = true;
        }

        public int CommandsExecuted { get; set; }
        public bool RollbackEncountered { get; set; }
        public bool CommitEncountered { get; set; }
        public string TempDirectory
        {
            get { return Path.GetTempPath(); }
        }

        public string BackupDirectory
        {
            get;
            set;
        }
    }

    internal class TestCommandProcessor : CommandProcessor
    {
        public TestCommandProcessor() : base("Test",new TestContext())
        {}

        public TestContext TestContext { get { return ProcessorContext as TestContext; } }
    }

	[TestFixture]
	public class CommandTests : AbstractTest
	{
	    private DicomFile _dicomFile;
		
        public CommandTests()
        {
            _dicomFile = new DicomFile();
            SetupMR(_dicomFile.DataSet);
        }
        
		[Test]
		public void TestSaveAndDeleteCommand()
		{
		    string file;

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "Test.dcm");

                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 2);

                Assert.IsTrue(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {

                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
            }

            // Delete the file
            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new FileDeleteCommand(file, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsFalse(File.Exists(file));
            }

            // Resave and delete
            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new FileDeleteCommand(file, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 2);

                Assert.IsFalse(File.Exists(file));
            }

            // Resave and delete Rollback
            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new FileDeleteCommand(file, true));
                processor.AddCommand(new FileDeleteCommand(file, true)); // Should fail

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);
                Assert.IsTrue(File.Exists(file));
            }

            // Cleanup the file
		    File.Delete(file);
		}

        [Test]
        public void AggregateTest()
        {
            string file;

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "AggregateTest.dcm");

                var aggregateCommand = new AggregateCommand();
                processor.AddCommand(aggregateCommand);

                aggregateCommand.AddSubCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                aggregateCommand.AddSubCommand(new FileDeleteCommand(file, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                Assert.IsTrue(File.Exists(file));
            }
        }
	}
}

#endif