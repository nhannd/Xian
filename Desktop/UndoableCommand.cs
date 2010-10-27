#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Abstract base class for 'undoable' commands.
	/// </summary>
	public abstract class UndoableCommand : Command
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected UndoableCommand()
		{
		}

		/// <summary>
		/// Performs and 'undo' of the <see cref="Command.Execute"/> operation.
		/// </summary>
		public abstract void Unexecute();
	}
}
