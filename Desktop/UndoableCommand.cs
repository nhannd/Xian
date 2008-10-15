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

using ClearCanvas.Common;
using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A command that facilitates undo/redo using the <b>Memento</b> design pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is common for the framework to check the equality of the 
	/// <see cref="BeginState"/> and <see cref="EndState"/> mementos in order to decide whether or not an
	/// <see cref="UndoableCommand"/> should be added to a <see cref="CommandHistory"/>
	/// object.  Therefore, it is good practice to override and implement the
	/// <see cref="object.Equals(object)"/> method on memento classes.
	/// </para>
	/// </remarks>
	/// <seealso cref="CommandHistory"/>
	/// <seealso cref="IMemorable"/>
	public class UndoableCommand : Command
	{
		private readonly IMemorable _originator;
		private object _beginState;
		private object _endState;

		private event EventHandler _executed;
		private event EventHandler _unexecuted;

		/// <summary>
		/// Default constructor for subclasses.
		/// </summary>
		/// <remarks>
		/// Subclasses that use this constructor <b>must</b> override
		/// <see cref="Execute"/> and <see cref="Unexecute"/>, otherwise
		/// the command will do nothing (because <see cref="Originator"/> will be null).
		/// </remarks>
		protected UndoableCommand()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originator">The originator is the object responsible for creating
		/// memento objects and restoring state from them.</param>
		public UndoableCommand(IMemorable originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			_originator = originator;
		}

		/// <summary>
		/// Gets the originator.
		/// </summary>
		/// <remarks>
		/// The originator is the object responsible for creating
		/// mementos and restoring state from them.
		/// </remarks>
		protected IMemorable Originator
		{
			get { return _originator; }
		}

		/// <summary>
		/// Gets the begin state, which is the state of the <see cref="Originator"/> before
		/// the operation resulting in this <see cref="UndoableCommand"/> was performed.
		/// </summary>
		public virtual object BeginState
		{
			get { return _beginState; }
			set { _beginState = value; }
		}

		/// <summary>
		/// Gets the end state, which is the state of the <see cref="Originator"/> after
		/// the operation resulting in this <see cref="UndoableCommand"/> was performed.
		/// </summary>
		public virtual object EndState
		{
			get { return _endState; }
			set { _endState = value; }
		}

		public event EventHandler Executed
		{
			add { _executed += value; }
			remove { _executed -= value; }
		}

		public event EventHandler Unexecuted
		{
			add { _unexecuted += value; }
			remove { _unexecuted -= value; }
		}

		protected virtual void OnExecuted()
		{
			EventsHelper.Fire(_executed, this, EventArgs.Empty);
		}

		protected virtual void OnUnexecuted()
		{
			EventsHelper.Fire(_unexecuted, this, EventArgs.Empty);
		}

		/// <summary>
		/// Performs a 'redo' by calling <see cref="IMemorable.SetMemento"/> on the 
		/// <see cref="Originator"/> with the <see cref="EndState"/> as a parameter.
		/// </summary>
		public override void Execute()
		{
			if (_originator != null)
				_originator.SetMemento(_endState);

			OnExecuted();
		}

		/// <summary>
		/// Performs an 'undo' by calling <see cref="IMemorable.SetMemento"/> on the 
		/// <see cref="Originator"/> with the <see cref="BeginState"/> as a parameter.
		/// </summary>
		public virtual void Unexecute()
		{
			if (_originator != null)
				_originator.SetMemento(_beginState);

			OnUnexecuted();
		}
	}
}
