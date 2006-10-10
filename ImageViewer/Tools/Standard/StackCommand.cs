using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
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
			imageBox.Draw();
		}

		public override void Unexecute()
		{
			base.Unexecute();
			ImageBox imageBox = base.Originator as ImageBox;
			imageBox.Draw();
		}
	}
}
