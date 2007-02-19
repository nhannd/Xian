using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class PositionGraphicCommand : UndoableCommand
	{
		public enum CreateOperation { None, Create, Delete };

		private CreateOperation _createOperation;

		public PositionGraphicCommand(IGraphic graphic)
			: this(graphic, CreateOperation.None)
		{
		}

		public PositionGraphicCommand(
			IGraphic graphic, CreateOperation createOperation) 
			: base(graphic as IMemorable)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			_createOperation = createOperation;
		}

		public override void Execute()
		{
			IGraphic graphic = base.Originator as IGraphic;

			if (_createOperation != CreateOperation.None)
				graphic.Visible = _createOperation == CreateOperation.Create;
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
			IGraphic graphic = base.Originator as IGraphic;

			if (_createOperation != CreateOperation.None)
				graphic.Visible = _createOperation == CreateOperation.Delete;
			else
				base.Unexecute();

			graphic.Draw();
		}
	}
}
