
using ClearCanvas.Desktop;
using System;
namespace ClearCanvas.ImageViewer.Imaging
{
	internal class VoiLutMemento : IMemento, IEquatable<VoiLutMemento>
	{
		private readonly ComposableLutMemento _composableLutMemento;
		private readonly bool _invert;

		public VoiLutMemento(IComposableLut originatingLut, bool invert)
		{
			_composableLutMemento = new ComposableLutMemento(originatingLut);
			_invert = invert;
		}

		public ComposableLutMemento ComposableLutMemento
		{
			get { return _composableLutMemento; }	
		}

		public bool Invert
		{
			get { return _invert; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is VoiLutMemento)
				return this.Equals((VoiLutMemento) obj);

			return false;
		}

		#region IEquatable<VoiLutMemento> Members

		public bool Equals(VoiLutMemento other)
		{
			if (other == null)
				return false;

			return this._invert == other._invert && this._composableLutMemento.Equals(other._composableLutMemento);
		}

		#endregion
	}
}
