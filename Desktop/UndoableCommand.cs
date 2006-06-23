using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class UndoableCommand : Command
	{
		private IMemorable m_Originator;
		private IMemento m_BeginState;
		private IMemento m_EndState;

		public UndoableCommand(IMemorable originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			m_Originator = originator;
		}

		protected IMemorable Originator
		{
			get { return m_Originator; }
		}

		public virtual IMemento BeginState
		{
			get { return m_BeginState; }
			set { m_BeginState = value; }
		}

		public virtual IMemento EndState
		{
			get { return m_EndState; }
			set { m_EndState = value; }
		}

		public override void Execute()
		{
			if (m_Originator != null)
				m_Originator.SetMemento(m_EndState);
		}

		public virtual void Unexecute()
		{
			if (m_Originator != null)
				m_Originator.SetMemento(m_BeginState);
		}
	}
}
