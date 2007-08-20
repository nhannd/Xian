using System;
namespace ClearCanvas.ImageViewer.Imaging
{
	/*
	/// <summary>
	/// It is not recommended that you actually implement your own <see cref="IVoiLutLinearState"/> 
	/// and <see cref="IStatefulVoiLutLinear"/>, but instead use the sealed class <see cref="StatefulVoiLutLinear"/>
	/// and classes derived from <see cref="VoiLutLinearState"/>.  These interfaces only exist to keep
	/// things as flexible as possible, and to emphasize that the objects should only be accessed via
	/// their defined interfaces.
	/// </summary>
	public interface IVoiLutLinearState : IMemorableComposableLutMemento, IEquatable<IVoiLutLinearState>
	{
		/// <summary>
		/// Because the state object is itself a memento, we must be able to capture 'snapshots' or clones of the
		/// state object in order to set/restore the owner Lut to its previous state.  Also, when applying a memento
		/// across a number of images, we need to use snapshots so that each Lut gets its own state object.
		/// </summary>
		/// <returns></returns>
		IMemorableComposableLutMemento SnapshotMemento();

		/// <summary>
		/// Should be set by the owner Lut when its state changes.
		/// </summary>
		IStatefulVoiLutLinear OwnerLut { get; set; }

		/// <summary>
		/// Should be used only by the owner Lut to get/set its own WindowCenter.
		/// </summary>
		double WindowCenter { get; set; }

		/// <summary>
		/// Should be used only by the owner Lut to get/set its own WindowWidth.
		/// </summary>
		double WindowWidth { get; set; }
	}*/
}
