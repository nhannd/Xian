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
	/// Abstract base class for commands.
	/// </summary>
	public abstract class Command
	{
		private string _name;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected Command()
		{
		}

		/// <summary>
		/// Gets or sets the command name.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		public abstract void Execute();
	}
}
