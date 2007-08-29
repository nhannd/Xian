using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLut : ILut, IEquatable<IPresentationLut>
	{
		bool Invert { get; set; }
	}
}
