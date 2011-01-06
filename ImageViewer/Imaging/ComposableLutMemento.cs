#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ComposableLutMemento : IEquatable<ComposableLutMemento>
	{
		#region Private Fields

		private readonly IComposableLut _originatingLut;
		private readonly object _innerMemento;

		#endregion

		public ComposableLutMemento(IComposableLut originatingLut)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
		}

		#region Public Members

		public IComposableLut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }	
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
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
			if (other == null)
				return false;

			if (this._innerMemento != null)
				return this._originatingLut.Equals(other._originatingLut) && this._innerMemento.Equals(other._innerMemento);
			
			return this._originatingLut.Equals(other._originatingLut);
		}

		#endregion
		#endregion
	}
}
