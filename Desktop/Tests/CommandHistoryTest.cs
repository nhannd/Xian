#if	UNIT_TESTS

using System;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;


namespace ClearCanvas.Desktop.Tests.CommandHistoryTest
{
	[TestFixture]
	public class CommandHistoryTest
	{
		public static string _message;
		//public string _commandAddedEventMessage;
		//public string _reachedCommandHistoryBeginningEventMessage;
		//public string _reachedCommandHistoryEndEventMessage;

		public CommandHistoryTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void AddUndoRedo()
		{
			int maxSize = 10;
			CommandHistory history = new CommandHistory(maxSize);

			// Add 4 commands
			TestCommand1 cmd1 = new TestCommand1();
			TestCommand2 cmd2 = new TestCommand2();
			TestCommand3 cmd3 = new TestCommand3();
			TestCommand4 cmd4 = new TestCommand4();

			history.AddCommand(cmd1);
			history.AddCommand(cmd2);
			history.AddCommand(cmd3);
			history.AddCommand(cmd4);

			Assert.AreEqual(4, history.NumCommands);

			// Undo all commands we've added 
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand4", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand1", _message);

			// Try to undo two extra times, even though we should be at the beginning
			_message = String.Empty;
			history.Undo();
			Assert.AreEqual(String.Empty, _message);
			history.Undo();
			Assert.AreEqual(String.Empty, _message);

			// Redo all commands we've added
			history.Redo();
			Assert.AreEqual("Executed TestCommand1", _message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand2", _message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand3", _message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand4", _message);

			// Try to redo two extra times, even though we should be at the end
			_message = String.Empty;
			history.Redo();
			Assert.AreEqual(String.Empty, _message);
			history.Redo();
			Assert.AreEqual(String.Empty, _message);

			// Add another command
			TestCommand5 cmd5 = new TestCommand5();
			history.AddCommand(cmd5);

			Assert.AreEqual(5, history.NumCommands);

			// Undo a couple times
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand5", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand4", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", _message);

			// Now add a new command
			TestCommand6 cmd6 = new TestCommand6();
			history.AddCommand(cmd6);

			Assert.AreEqual(3, history.NumCommands);
			
			// Try to redo again
			_message = String.Empty;
			history.Redo();
			Assert.AreEqual(String.Empty, _message);

			// Undo again
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand6", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", _message);
		}

		[Test]
		public void EmptyHistory()
		{
			int maxSize = 0;
			CommandHistory history = new CommandHistory(maxSize);

			// Add 2 commands
			TestCommand1 cmd1 = new TestCommand1();
			TestCommand2 cmd2 = new TestCommand2();

			history.AddCommand(cmd1);
			Assert.AreEqual(0, history.NumCommands);
			history.AddCommand(cmd2);
			Assert.AreEqual(0, history.NumCommands);

			// Try to undo/redo on an empty history
			_message = String.Empty;
			history.Undo();
			Assert.AreEqual(String.Empty, _message);
			history.Redo();
			Assert.AreEqual(String.Empty, _message);
		}

		[Test]
		public void ForceFIFO()
		{
			int maxSize = 3;
			CommandHistory history = new CommandHistory(maxSize);

			// Add 3 commands
			TestCommand1 cmd1 = new TestCommand1();
			TestCommand2 cmd2 = new TestCommand2();
			TestCommand3 cmd3 = new TestCommand3();

			history.AddCommand(cmd1);
			history.AddCommand(cmd2);
			history.AddCommand(cmd3);
			Assert.AreEqual(3, history.NumCommands);

			// Force history to drop oldest item (FIFO)
			TestCommand4 cmd4 = new TestCommand4();
			history.AddCommand(cmd4);
			Assert.AreEqual(3, history.NumCommands);

			// Undo all commands
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand4", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", _message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", _message);

			// We've undone all the commands; test the boundary
			// by adding a new one
			TestCommand5 cmd5 = new TestCommand5();
			history.AddCommand(cmd5);
			Assert.AreEqual(1, history.NumCommands);
		}

		[Test]
		public void SubscribeToEvents()
		{
			//_commandAddedEventMessage = String.Empty;
			//_reachedCommandHistoryBeginningEventMessage = String.Empty;
			//_reachedCommandHistoryEndEventMessage = String.Empty;

			int maxSize = 3;
			CommandHistory history = new CommandHistory(maxSize);
			//history.CommandAdded += new EventHandler(OnCommandAdded);
			//history.ReachedCommandHistoryBeginning += new EventHandler(OnReachedCommandHistoryBeginning);
			//history.ReachedCommandHistoryEnd += new EventHandler(OnReachedCommandHistoryEnd);

			// Add 3 commands
			TestCommand1 cmd1 = new TestCommand1();
			TestCommand2 cmd2 = new TestCommand2();
			TestCommand3 cmd3 = new TestCommand3();

			history.AddCommand(cmd1);
			//Assert.AreEqual("TestCommand1", _commandAddedEventMessage);
			history.AddCommand(cmd2);
			//Assert.AreEqual("TestCommand2", _commandAddedEventMessage);
			history.AddCommand(cmd3);
			//Assert.AreEqual("TestCommand3", _commandAddedEventMessage);

			// Undo them all
			history.Undo();
			//Assert.AreEqual(String.Empty, _reachedCommandHistoryBeginningEventMessage);
			history.Undo();
			//Assert.AreEqual(String.Empty, _reachedCommandHistoryBeginningEventMessage);
			history.Undo();
			//Assert.AreEqual("ReachedBeginning", _reachedCommandHistoryBeginningEventMessage);

			// Redo them all
			history.Redo();
			//Assert.AreEqual(String.Empty, _reachedCommandHistoryEndEventMessage);
			history.Redo();
			//Assert.AreEqual(String.Empty, _reachedCommandHistoryEndEventMessage);
			history.Redo();
			//Assert.AreEqual("ReachedEnd", _reachedCommandHistoryEndEventMessage);

		}

		/*
		private void OnCommandAdded(object sender, EventArgs e)
		{
			CommandAddedEventArgs args = (CommandAddedEventArgs) e;
			//_commandAddedEventMessage = args.Command.Name;
		}*/

/*		private void OnReachedCommandHistoryBeginning(object sender, EventArgs e)
		{
			_reachedCommandHistoryBeginningEventMessage = "ReachedBeginning";
		}

		private void OnReachedCommandHistoryEnd(object sender, EventArgs e)
		{
			_reachedCommandHistoryEndEventMessage = "ReachedEnd";
		}
*/	}

	public class TestCommand1 : UndoableCommand
	{
		public TestCommand1() : base(null)
		{
			base.Name = "TestCommand1";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand1";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand1";
		}
	}

	public class TestCommand2 : UndoableCommand
	{
		public TestCommand2() : base(null)
		{
			base.Name = "TestCommand2";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand2";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand2";
		}
	}

	public class TestCommand3 : UndoableCommand
	{
		public TestCommand3() : base(null)
		{
			base.Name = "TestCommand3";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand3";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand3";
		}
	}

	public class TestCommand4 : UndoableCommand
	{
		public TestCommand4() : base(null)
		{
			base.Name = "TestCommand4";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand4";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand4";
		}
	}

	public class TestCommand5 : UndoableCommand
	{
		public TestCommand5() : base(null)
		{
			base.Name = "TestCommand5";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand5";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand5";
		}
	}

	public class TestCommand6 : UndoableCommand
	{
		public TestCommand6() : base(null)
		{
			base.Name = "TestCommand6";
		}

		public override void Execute()
		{
			base.Execute();
			CommandHistoryTest._message = "Executed TestCommand6";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest._message = "Unexecuted TestCommand6";
		}
	}
}

#endif