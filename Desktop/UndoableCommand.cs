using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class UndoableCommand : Command
	{
		private IMemorable _Originator;
		private IMemento _BeginState;
		private IMemento _EndState;

		public UndoableCommand(IMemorable originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			_Originator = originator;
		}

		protected IMemorable Originator
		{
			get { return _Originator; }
		}

		public virtual IMemento BeginState
		{
			get { return _BeginState; }
			set { _BeginState = value; }
		}

		public virtual IMemento EndState
		{
			get { return _EndState; }
			set { _EndState = value; }
		}

		public override void Execute()
		{
			if (_Originator != null)
				_Originator.SetMemento(_EndState);
		}

		public virtual void Unexecute()
		{
			if (_Originator != null)
				_Originator.SetMemento(_BeginState);
		}
	}
}
