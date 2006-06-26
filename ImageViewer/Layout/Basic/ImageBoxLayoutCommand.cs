using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Summary description for LayoutCommand.
	/// </summary>
	public class ImageBoxLayoutCommand : UndoableCommand
	{
		public ImageBoxLayoutCommand(PhysicalWorkspace physicalWorkspace)
			: base(physicalWorkspace)
		{
		}

		public override void Execute()
		{
			base.Execute();
			PhysicalWorkspace physicalWorkspace = base.Originator as PhysicalWorkspace;
			physicalWorkspace.Draw(false);
		}

		public override void Unexecute()
		{
			base.Unexecute();
			PhysicalWorkspace physicalWorkspace = base.Originator as PhysicalWorkspace;
			physicalWorkspace.Draw(false);
		}
	}
}
