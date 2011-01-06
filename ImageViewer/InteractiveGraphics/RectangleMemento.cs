#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	internal class RectangleMemento : IEquatable<RectangleMemento>
	{
		PointF _topLeft;
		PointF _bottomRight;

		public RectangleMemento()
		{
		}

		public PointF TopLeft
		{
			get { return _topLeft; }
			set { _topLeft = value; }
		}
		
		public PointF BottomRight
		{
			get { return _bottomRight; }
			set { _bottomRight = value; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as RectangleMemento);
		}

		#region IEquatable<RectangleMemento> Members

		public bool Equals(RectangleMemento other)
		{
			if (other == null)
				return false;

			return TopLeft == other.TopLeft && BottomRight == other.BottomRight;
		}

		#endregion
	}
}
