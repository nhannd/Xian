using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	public interface IImageBox : IDisposable
	{
		IImageViewer ImageViewer { get; }
		IPhysicalWorkspace ParentPhysicalWorkspace { get; }
		TileCollection Tiles { get; }
		ITile SelectedTile { get; }
		RectangleF NormalizedRectangle { get; set; }
		IDisplaySet DisplaySet { get; set; }
		bool Selected { get; }
		int Rows { get; }
		int Columns { get; }
		ITile this[int row, int column] { get; }
		IPresentationImage TopLeftPresentationImage { get; set; }
		int TopLeftPresentationImageIndex { get; set; }

		void Draw();
		void SetTileGrid(int numberOfRows, int numberOfColumns);
	}
}
