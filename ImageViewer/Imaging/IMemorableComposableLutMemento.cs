using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/*
	/// <summary>
	/// Should be used in conjunction with a <see cref="VoiLutOperationApplicator"/> to implement
	/// the Memento pattern without need for explicit knowledge of the contents of the memento.
	/// The <see cref="IMemorableComposableLutMemento"/> and <see cref="IMemorableComposableLut"/> pattern
	/// is used to remember/restore incremental changes in Luts, completely restoring Luts when necessary.
	/// That being said, any <see cref="IMemorableComposableLutMemento"/> must be capable of completely 
	/// restoring a Lut via the <see cref="RestoreLut"/> method.
	/// </summary>
	/// <remarks>
	/// The memento, in some cases, could just be the Lut object itself as long as it does not contain a large byte[] array
	/// that is going to be stored in the Undo/Redo history.  The memento should be a lightweight object that contains enough
	/// information to completely restore the Lut without storing the entire Lut itself (byte []).  This can almost always be achieved since
	/// Luts are normally calculated in some way via an algorithm, or exist in a Dicom Header as either Window/Center values or
	/// a Data Lut.  Whenever possible, the parameters necessary to restore the Lut should be all that is contained in the memento.
	/// </remarks>
	public interface IMemorableComposableLutMemento : IMemento
	{
		/// <summary>
		/// Called when the currently installed Lut cannot use this memento in any way for a given undo/redo operation.
		/// </summary>
		/// <param name="minInputValue">the minimum input value (should come from the minimum output value of the modality lut).</param>
		/// <param name="maxInputValue">the maximum input value (should come from the maximum output value of the modality lut).</param>
		/// <returns>the restored Lut</returns>
		IMemorableComposableLut RestoreLut(int minInputValue, int maxInputValue);
	}

	public interface IMemorableComposableLut : IComposableLUT, IMemorable<IMemorableComposableLutMemento>
	{
		/// <summary>
		/// Returns whether or not the given memento can be used by this Lut to restore/return to a different state via an Undo/Redo operation.
		/// This method will not throw an exception, unlike <see cref="IMemorable.SetMemento"/>.  In the case where the memento cannot be used, the
		/// Originator (see <see cref="VoiLutOperationApplicator"/> and <see cref="IVoiLutManager"/>) should install a new Lut via 
		/// <see cref="IMemorableComposableLutMemento.RestoreLut"/>.
		/// </summary>
		/// <param name="memento">the memento</param>
		/// <returns>whether or not the memento can be used internally to restore/return to a given state</returns>
		bool TrySetMemento(IMemorableComposableLutMemento memento);
	}*/
}
