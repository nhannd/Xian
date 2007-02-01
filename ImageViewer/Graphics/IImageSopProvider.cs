using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IImageSopProvider
	{
		ImageSop ImageSop { get; }
	}
}
