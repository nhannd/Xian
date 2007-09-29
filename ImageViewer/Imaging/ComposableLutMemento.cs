using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ComposableLutMemento : IMemento, IEquatable<ComposableLutMemento>
	{
		private readonly IComposableLut _originatingLut;
		private readonly IMemento _innerMemento;

		public ComposableLutMemento(IComposableLut originatingLut)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
		}

		public IComposableLut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public IMemento InnerMemento
		{
			get { return _innerMemento; }	
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is ComposableLutMemento)
				return this.Equals((ComposableLutMemento) obj);

			return false;
		}

		#region IEquatable<LutMemento> Members

		public bool Equals(ComposableLutMemento other)
		{
			if (this._innerMemento != null)
				return this._originatingLut.Equals(other._originatingLut) && this._innerMemento.Equals(other._innerMemento);
			
			return this._originatingLut.Equals(other._originatingLut);
		}

		#endregion
	}
}
