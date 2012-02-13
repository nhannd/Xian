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
	internal sealed class ColorMapMemento : IEquatable<ColorMapMemento>
	{
		#region Private Fields

		private readonly IColorMap _originator;
		private readonly object _innerMemento;

		#endregion

		public ColorMapMemento(IColorMap originator)
		{
			Platform.CheckForNullReference(originator, "originator");
			_originator = originator;
			_innerMemento = originator.CreateMemento();
		}

		#region Public Members

		public IColorMap Originator
		{
			get { return _originator; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }
		}

		public override int GetHashCode()
		{
			return 0x4462FFBB ^ _originator.GetHashCode() ^ (_innerMemento != null ? _innerMemento.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			return obj is ColorMapMemento && Equals((ColorMapMemento) obj);
		}

		public bool Equals(ColorMapMemento other)
		{
			return other != null && _originator.Equals(other._originator) && Equals(_innerMemento, other._innerMemento);
		}

		#endregion
	}
}