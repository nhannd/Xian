using System;

namespace ClearCanvas.Desktop
{
	public abstract class Command
	{
		private string _Name;

		public Command()
		{
		}

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public abstract void Execute();
	}
}
