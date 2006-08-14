using System;

namespace ClearCanvas.Desktop
{
	public abstract class Command
	{
		private string _name;

		public Command()
		{
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public abstract void Execute();
	}
}
