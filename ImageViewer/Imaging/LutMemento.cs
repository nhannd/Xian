using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class LutMemento : ILutMemento, IEquatable<LutMemento>
	{
		private ILut _originatingLut;
		private IMemento _innerMemento;

		public LutMemento(ILut originatingLut, IMemento innerMemento)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = innerMemento;
		}

		public LutMemento(ILut originatingLut)
			: this(originatingLut, null)
		{
		}

		#region ILutMemento Members

		public ILut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public IMemento InnerMemento
		{
			get { return _innerMemento; }	
		}

		#endregion

		#region IEquatable<ILutMemento> Members

		public bool Equals(ILutMemento other)
		{
			if (other == this)
				return true;

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
