using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Any <see cref="IMemorableComposableLutMemento"/> that need access to the associated <see cref="StandardGrayscaleImageGraphic"/>
	/// should implement this interface.  Because we often use a memento from a different image to set/alter the Lut, implementations of
	/// <see cref="IVoiLutManager"/> should set the <see cref="Graphic"/> variable appropriately for these cases (for an example, see <see cref="StandardVoiLutManager"/>).
	/// </summary>
	public interface IStandardGrayscaleImageGraphicMemorableComposableLutMemento : IMemorableComposableLutMemento
	{
		StandardGrayscaleImageGraphic Graphic { get; set; }
	}
}
