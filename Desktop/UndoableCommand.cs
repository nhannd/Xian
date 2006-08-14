using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class UndoableCommand : Command
	{
		private IMemorable _originator;
		private IMemento _beginState;
		private IMemento _endState;

		public UndoableCommand(IMemorable originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			_originator = originator;
		}

		protected IMemorable Originator
		{
			get { return _originator; }
		}

		public virtual IMemento BeginState
		{
			get { return _beginState; }
			set { _beginState = value; }
		}

		public virtual IMemento EndState
		{
			get { return _endState; }
			set { _endState = value; }
		}

		public override void Execute()
		{
			if (_originator != null)
				_originator.SetMemento(_endState);
		}

		public virtual void Unexecute()
		{
			if (_originator != null)
				_originator.SetMemento(_beginState);
		}
	}
}
