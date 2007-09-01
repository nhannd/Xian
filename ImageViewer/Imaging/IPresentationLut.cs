using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLut : ILut, IEquatable<IPresentationLut>
	{
		bool Invert { get; set; }
	}
}
