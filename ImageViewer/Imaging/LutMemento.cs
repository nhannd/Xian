using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An implementation of <see cref="ILutMemento"/> used by the framework (typically implementors of 
	/// <see cref="IVoiLutManager"/> and <see cref="IPresentationLutManager"/>) to handle undo/redo 
	/// operations on Luts.
	/// </summary>
	/// <remarks>
	/// The <see cref="OriginatingLut"/> member stores the Lut that was installed at the time this memento
	/// was created.  The <see cref="InnerMemento"/> stores the memento (if applicable) created by the Lut itself.
	/// </remarks>
	public class LutMemento : ILutMemento, IEquatable<LutMemento>
	{
		private readonly ILut _originatingLut;
		private readonly IMemento _innerMemento;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originatingLut">The currently installed lut.</param>
		public LutMemento(ILut originatingLut)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
		}

		#region ILutMemento Members

		/// <summary>
		/// The <see cref="ILut"/> that was installed at the time this <see cref="LutMemento"/> was created.
		/// </summary>
		public ILut OriginatingLut
		{
			get { return _originatingLut; }
		}

		/// <summary>
		/// The <see cref="IMemento"/> that was created by the <see cref="OriginatingLut"/> at the time this <see cref="LutMemento"/> was created.
		/// </summary>
		public IMemento InnerMemento
		{
			get { return _innerMemento; }	
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is ILutMemento)
				return this.Equals((ILutMemento) obj);

			return false;
		}

		#region IEquatable<ILutMemento> Members

		public bool Equals(ILutMemento other)
		{
			if (other is LutMemento)
				return this.Equals((LutMemento)other);

			return false;
		}

		#endregion

		#region IEquatable<LutMemento> Members

		public bool Equals(LutMemento other)
		{
			if (this._innerMemento != null)
				return this._originatingLut.Equals(other._originatingLut) && this._innerMemento.Equals(other._innerMemento);
			
			return this._originatingLut.Equals(other._originatingLut);
		}

		#endregion
	}
}
