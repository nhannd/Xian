using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface ISpatialTransform : IMemorable
	{
		bool FlipHorizontal { get; set; }
		bool FlipVertical { get; set; }
		int Rotation { get; set; }
		float Scale { get; set; }
		bool ScaleToFit { get; set; }
		float ScaleX { get; }
		float ScaleY { get; }
		float TranslationX { get; set; }
		float TranslationY { get; set; }
	}
}
