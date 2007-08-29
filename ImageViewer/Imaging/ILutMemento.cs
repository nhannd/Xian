using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface ILutMemento : IMemento, IEquatable<ILutMemento>
	{
		ILut OriginatingLut { get; }
		IMemento InnerMemento { get; }
	}
}
