#if	UNIT_TESTS

using System;
//using MbUnit.Core.Framework;
//using MbUnit.Framework;
using NUnit.Framework;


namespace ClearCanvas.Common.Application.Tests.CommandHistoryTest
{
	[TestFixture]
	public class CommandHistoryTest
	{
		public static string m_Message;
		//public string m_CommandAddedEventMessage;
		//public string m_ReachedCommandHistoryBeginningEventMessage;
		//public string m_ReachedCommandHistoryEndEventMessage;

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
			Assert.AreEqual("Unexecuted TestCommand4", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand1", m_Message);

			// Try to undo two extra times, even though we should be at the beginning
			m_Message = String.Empty;
			history.Undo();
			Assert.AreEqual(String.Empty, m_Message);
			history.Undo();
			Assert.AreEqual(String.Empty, m_Message);

			// Redo all commands we've added
			history.Redo();
			Assert.AreEqual("Executed TestCommand1", m_Message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand2", m_Message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand3", m_Message);
			history.Redo();
			Assert.AreEqual("Executed TestCommand4", m_Message);

			// Try to redo two extra times, even though we should be at the end
			m_Message = String.Empty;
			history.Redo();
			Assert.AreEqual(String.Empty, m_Message);
			history.Redo();
			Assert.AreEqual(String.Empty, m_Message);

			// Add another command
			TestCommand5 cmd5 = new TestCommand5();
			history.AddCommand(cmd5);

			Assert.AreEqual(5, history.NumCommands);

			// Undo a couple times
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand5", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand4", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", m_Message);

			// Now add a new command
			TestCommand6 cmd6 = new TestCommand6();
			history.AddCommand(cmd6);

			Assert.AreEqual(3, history.NumCommands);
			
			// Try to redo again
			m_Message = String.Empty;
			history.Redo();
			Assert.AreEqual(String.Empty, m_Message);

			// Undo again
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand6", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", m_Message);
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
			m_Message = String.Empty;
			history.Undo();
			Assert.AreEqual(String.Empty, m_Message);
			history.Redo();
			Assert.AreEqual(String.Empty, m_Message);
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
			Assert.AreEqual("Unexecuted TestCommand4", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand3", m_Message);
			history.Undo();
			Assert.AreEqual("Unexecuted TestCommand2", m_Message);

			// We've undone all the commands; test the boundary
			// by adding a new one
			TestCommand5 cmd5 = new TestCommand5();
			history.AddCommand(cmd5);
			Assert.AreEqual(1, history.NumCommands);
		}

		[Test]
		public void SubscribeToEvents()
		{
			//m_CommandAddedEventMessage = String.Empty;
			//m_ReachedCommandHistoryBeginningEventMessage = String.Empty;
			//m_ReachedCommandHistoryEndEventMessage = String.Empty;

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
			//Assert.AreEqual("TestCommand1", m_CommandAddedEventMessage);
			history.AddCommand(cmd2);
			//Assert.AreEqual("TestCommand2", m_CommandAddedEventMessage);
			history.AddCommand(cmd3);
			//Assert.AreEqual("TestCommand3", m_CommandAddedEventMessage);

			// Undo them all
			history.Undo();
			//Assert.AreEqual(String.Empty, m_ReachedCommandHistoryBeginningEventMessage);
			history.Undo();
			//Assert.AreEqual(String.Empty, m_ReachedCommandHistoryBeginningEventMessage);
			history.Undo();
			//Assert.AreEqual("ReachedBeginning", m_ReachedCommandHistoryBeginningEventMessage);

			// Redo them all
			history.Redo();
			//Assert.AreEqual(String.Empty, m_ReachedCommandHistoryEndEventMessage);
			history.Redo();
			//Assert.AreEqual(String.Empty, m_ReachedCommandHistoryEndEventMessage);
			history.Redo();
			//Assert.AreEqual("ReachedEnd", m_ReachedCommandHistoryEndEventMessage);

		}

		/*
		private void OnCommandAdded(object sender, EventArgs e)
		{
			CommandAddedEventArgs args = (CommandAddedEventArgs) e;
			//m_CommandAddedEventMessage = args.Command.Name;
		}*/

/*		private void OnReachedCommandHistoryBeginning(object sender, EventArgs e)
		{
			m_ReachedCommandHistoryBeginningEventMessage = "ReachedBeginning";
		}

		private void OnReachedCommandHistoryEnd(object sender, EventArgs e)
		{
			m_ReachedCommandHistoryEndEventMessage = "ReachedEnd";
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
			CommandHistoryTest.m_Message = "Executed TestCommand1";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand1";
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
			CommandHistoryTest.m_Message = "Executed TestCommand2";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand2";
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
			CommandHistoryTest.m_Message = "Executed TestCommand3";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand3";
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
			CommandHistoryTest.m_Message = "Executed TestCommand4";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand4";
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
			CommandHistoryTest.m_Message = "Executed TestCommand5";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand5";
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
			CommandHistoryTest.m_Message = "Executed TestCommand6";
		}

		public override void Unexecute()
		{
			base.Unexecute();
			CommandHistoryTest.m_Message = "Unexecuted TestCommand6";
		}
	}
}

#endif