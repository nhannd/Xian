using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class ImageBoxLayoutCommand : UndoableCommand
	{
		public ImageBoxLayoutCommand(IPhysicalWorkspace physicalWorkspace)
			: base(physicalWorkspace)
		{
		}

		public override void Execute()
		{
			base.Execute();
			IPhysicalWorkspace physicalWorkspace = base.Originator as IPhysicalWorkspace;
			physicalWorkspace.Draw();
		}

		public override void Unexecute()
		{
			base.Unexecute();
			IPhysicalWorkspace physicalWorkspace = base.Originator as IPhysicalWorkspace;
			physicalWorkspace.Draw();
		}
	}
}
