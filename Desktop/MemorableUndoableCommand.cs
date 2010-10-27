#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A command that facilitates undo/redo using the <b>Memento</b> design pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is common for the framework to check the equality of the 
	/// <see cref="BeginState"/> and <see cref="EndState"/> mementos in order to decide whether or not an
	/// <see cref="MemorableUndoableCommand"/> should be added to a <see cref="CommandHistory"/>
	/// object.  Therefore, it is good practice to override and implement the
	/// <see cref="object.Equals(object)"/> method on memento classes.
	/// </para>
	/// </remarks>
	/// <seealso cref="CommandHistory"/>
	/// <seealso cref="IMemorable"/>
	public class MemorableUndoableCommand : UndoableCommand
	{
		private readonly IMemorable _originator;
		private object _beginState;
		private object _endState;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originator">The originator is the object responsible for creating
		/// memento objects and restoring state from them.</param>
		public MemorableUndoableCommand(IMemorable originator)
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
		/// the operation resulting in this <see cref="MemorableUndoableCommand"/> was performed.
		/// </summary>
		public virtual object BeginState
		{
			get { return _beginState; }
			set { _beginState = value; }
		}

		/// <summary>
		/// Gets the end state, which is the state of the <see cref="Originator"/> after
		/// the operation resulting in this <see cref="MemorableUndoableCommand"/> was performed.
		/// </summary>
		public virtual object EndState
		{
			get { return _endState; }
			set { _endState = value; }
		}

		/// <summary>
		/// Performs a 'redo' by calling <see cref="IMemorable.SetMemento"/> on the 
		/// <see cref="Originator"/> with the <see cref="EndState"/> as a parameter.
		/// </summary>
		public override void Execute()
		{
			if (_originator != null)
				_originator.SetMemento(_endState);
		}

		/// <summary>
		/// Performs an 'undo' by calling <see cref="IMemorable.SetMemento"/> on the 
		/// <see cref="Originator"/> with the <see cref="BeginState"/> as a parameter.
		/// </summary>
		public override void Unexecute()
		{
			if (_originator != null)
				_originator.SetMemento(_beginState);
		}
	}
}
