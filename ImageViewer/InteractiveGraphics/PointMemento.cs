#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class PointMemento : IEquatable<PointMemento>
	{
		public readonly PointF Point;

		public PointMemento(PointF point)
		{
			this.Point = point;
		}

		public override int GetHashCode()
		{
			return this.Point.GetHashCode() ^ 0x0BE4AD82;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointMemento)
				return this.Equals((PointMemento)obj);
			return false;
		}

		public bool Equals(PointMemento other)
		{
			if (other == null)
				return false;
			return this.Point == other.Point;
		}

		public override string ToString()
		{
			return this.Point.ToString();
		}
	}
}
