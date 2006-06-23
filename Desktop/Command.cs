using System;

namespace ClearCanvas.Desktop
{
	public abstract class Command
	{
		private string m_Name;

		public Command()
		{
		}

		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		public abstract void Execute();
	}
}
