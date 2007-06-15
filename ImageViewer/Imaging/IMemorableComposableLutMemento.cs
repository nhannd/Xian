using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IMemorableComposableLutMemento : IMemento
	{
		IMemorableComposableLut RestoreLut();
	}

	public interface IMemorableComposableLut : IComposableLUT, IMemorable<IMemorableComposableLutMemento>
	{
		bool TrySetMemento(IMemorableComposableLutMemento memento);
	}
}
