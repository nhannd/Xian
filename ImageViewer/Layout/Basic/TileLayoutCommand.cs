using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Summary description for LayoutCommand.
	/// </summary>
	public class TileLayoutCommand : UndoableCommand
	{
		public TileLayoutCommand(ImageBox imageBox)
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
