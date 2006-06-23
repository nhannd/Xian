using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Common.Application;
using ClearCanvas.Common.Application.Tools;

namespace ClearCanvas.Workstation.Tools.Standard
{
	/// <summary>
	/// Summary description for StackCommand.
	/// </summary>
	public class StackCommand : UndoableCommand
	{
		public StackCommand(ImageBox imageBox)
			: base(imageBox)
		{
		}

		public override void Execute()
		{
			base.Execute();
			ImageBox imageBox = base.Originator as ImageBox;
			imageBox.Draw(false);
		}

		public override void Unexecute()
		{
			base.Unexecute();
			ImageBox imageBox = base.Originator as ImageBox;
			imageBox.Draw(false);
		}
	}
}
