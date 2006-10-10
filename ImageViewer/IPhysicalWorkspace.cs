using System;

namespace ClearCanvas.ImageViewer
{
	public interface IPhysicalWorkspace : IDisposable
	{
		IImageViewer ImageViewer { get; }
		ILogicalWorkspace LogicalWorkspace { get; }
		ImageBoxCollection ImageBoxes { get; }
		IImageBox SelectedImageBox { get; }
		int Rows { get; }
		int Columns { get; }

		void Draw();
		void SetImageBoxGrid(int rows, int columns);
	}
}
