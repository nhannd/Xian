using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class LutMemento : IMemento, IEquatable<LutMemento>
	{
		private readonly ILut _originatingLut;
		private readonly IMemento _innerMemento;

		public LutMemento(ILut originatingLut)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
		}

		#region LutMemento Members

		public ILut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public IMemento InnerMemento
		{
			get { return _innerMemento; }	
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is LutMemento)
				return this.Equals((LutMemento) obj);

			return false;
		}

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
