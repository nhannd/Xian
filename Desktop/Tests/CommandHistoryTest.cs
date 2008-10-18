#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

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
	}

	public class TestCommand1 : UndoableCommand
	{
		public TestCommand1()
		{
			base.Name = "TestCommand1";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand1";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand1";
		}
	}

	public class TestCommand2 : UndoableCommand
	{
		public TestCommand2()
		{
			base.Name = "TestCommand2";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand2";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand2";
		}
	}

	public class TestCommand3 : UndoableCommand
	{
		public TestCommand3()
		{
			base.Name = "TestCommand3";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand3";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand3";
		}
	}

	public class TestCommand4 : UndoableCommand
	{
		public TestCommand4()
		{
			base.Name = "TestCommand4";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand4";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand4";
		}
	}

	public class TestCommand5 : UndoableCommand
	{
		public TestCommand5()
		{
			base.Name = "TestCommand5";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand5";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand5";
		}
	}

	public class TestCommand6 : UndoableCommand
	{
		public TestCommand6()
		{
			base.Name = "TestCommand6";
		}

		public override void Execute()
		{
			CommandHistoryTest._message = "Executed TestCommand6";
		}

		public override void Unexecute()
		{
			CommandHistoryTest._message = "Unexecuted TestCommand6";
		}
	}
}

#endif