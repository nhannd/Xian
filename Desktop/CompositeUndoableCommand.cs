#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> whose only purpose is to process other <see cref="UndoableCommand"/>s in
	/// a repeatable manner, such that the entire set of commands can be undone/redone.
	/// </summary>
	/// <remarks>
	/// The <see cref="CompositeUndoableCommand"/> doesn't place any explicit restrictions as to whether
	/// or not a <see cref="UndoableCommand"/> has already been executed or unexecuted, but rather it
	/// leaves the details up to the consumer.  Typically, before adding a <see cref="CompositeUndoableCommand"/>
	/// to the <see cref="CommandHistory"/>, it (or it's contained commands) must be Executed first.
	/// </remarks>
	public class CompositeUndoableCommand : UndoableCommand
	{
		private readonly List<UndoableCommand> _commands;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CompositeUndoableCommand()
		{
			_commands = new List<UndoableCommand>();
		}

		/// <summary>
		/// Gets the number of commands in this <see cref="CompositeUndoableCommand"/>.
		/// </summary>
		public int Count
		{
			get { return _commands.Count; }	
		}

		/// <summary>
		/// Adds/Enqueues an <see cref="UndoableCommand"/>.
		/// </summary>
		public void Enqueue(UndoableCommand command)
		{
			_commands.Add(command);
		}

		/// <summary>
		/// <see cref="Execute"/>s each command, from the beginning to the end.
		/// </summary>
		public override void Execute()
		{
			foreach (UndoableCommand command in _commands)
				command.Execute();
		}

		/// <summary>
		/// <see cref="Unexecute"/>s each command, from the end to the beginning.
		/// </summary>
		public override void Unexecute()
		{
			for (int i = _commands.Count - 1; i >= 0; --i)
				_commands[i].Unexecute();
		}
	}
}
