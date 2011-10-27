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

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class VoiLutMemento : IEquatable<VoiLutMemento>
	{
		private readonly IVoiLut _originatingLut;
		private readonly object _innerMemento;
		private readonly bool _invert;

		public VoiLutMemento(IVoiLut originatingLut, bool invert)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
			_invert = invert;
		}

		public IVoiLut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }
		}

		public bool Invert
		{
			get { return _invert; }
		}

		public override int GetHashCode()
		{
			return 0x76E013EC ^ _originatingLut.GetHashCode() ^ _invert.GetHashCode() ^ (_innerMemento != null ? _innerMemento.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(obj, this) || obj is VoiLutMemento && Equals((VoiLutMemento) obj);
		}

		public bool Equals(VoiLutMemento other)
		{
			return other != null && _originatingLut.Equals(other._originatingLut) && Equals(_innerMemento, other._innerMemento) && _invert == other._invert;
		}
	}
}