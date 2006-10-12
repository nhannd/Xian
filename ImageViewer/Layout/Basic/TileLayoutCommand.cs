using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class TileLayoutCommand : UndoableCommand
	{
		public TileLayoutCommand(IImageBox imageBox)
			: base(imageBox)
		{
		}

		public override void Execute()
		{
			base.Execute();
			IImageBox imageBox = base.Originator as IImageBox;
			imageBox.Draw();
		}

		public override void Unexecute()
		{
			base.Unexecute();
			IImageBox imageBox = base.Originator as IImageBox;
			imageBox.Draw();
		}
	}
}
