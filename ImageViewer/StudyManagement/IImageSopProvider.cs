using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface IImageSopProvider
	{
		ImageSop ImageSop { get; }
	}
}
