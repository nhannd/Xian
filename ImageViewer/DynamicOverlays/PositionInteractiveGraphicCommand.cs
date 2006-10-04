using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class PositionGraphicCommand : UndoableCommand
	{
		private bool _create;

		public PositionGraphicCommand(
			Graphic graphic, bool create) 
			: base(graphic as IMemorable)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			_create = create;
		}

		public override void Execute()
		{
			Graphic graphic = base.Originator as Graphic;

			if (_create)
				graphic.Visible = true;
			else
				base.Execute();

			graphic.Draw();
		}

		public override void Unexecute()
		{
			// If this command created the ruler, we simulate an undo not
			// by actually destroying the ruler, but by just making it invisible.
			// If we actually destroyed the ruler and removed it from the
			// layer hierarchy, then subsequently recreated
			// a new one through a redo operation, any commands in the command
			// history that depended on the old instance of the ruler would
			// be useless, since that ruler is no longer part of the layer hiearchy,
			// and thus would never be rendered
			Graphic graphic = base.Originator as Graphic;

			if (_create)
				graphic.Visible = false;
			else
				base.Unexecute();

			graphic.Draw();
		}
	}
}
