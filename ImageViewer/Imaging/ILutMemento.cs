using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A memento used specifically for storing undo/redo information for <see cref="ILut"/>s.
	/// </summary>
	/// <remarks>
	/// These objects are normally created/handled by an <see cref="IVoiLutManager"/> or <see cref="IPresentationLutManager"/>.
	/// </remarks>
	public interface ILutMemento : IMemento, IEquatable<ILutMemento>
	{
		/// <summary>
		/// Gets the originating lut (e.g. the one that was installed when this memento was created).
		/// </summary>
		ILut OriginatingLut { get; }

		/// <summary>
		/// Gets the inner memento (created by the <see cref="OriginatingLut"/> itself when this memento was created).
		/// </summary>
		IMemento InnerMemento { get; }
	}
}
