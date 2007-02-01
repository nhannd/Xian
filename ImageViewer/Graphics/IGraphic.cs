using System;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;
namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IGraphic
	{
		IGraphic ParentGraphic { get; }
		IPresentationImage ParentPresentationImage { get; }
		IImageViewer ImageViewer { get; }

		bool Visible { get; set; }
		CoordinateSystem CoordinateSystem { get; set; }
		SpatialTransform SpatialTransform { get; }
		string Name { get; set; }

		void Draw();
		bool HitTest(Point point);
		void Move(SizeF delta);
		void ResetCoordinateSystem();
	}
}
