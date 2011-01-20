#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class PointsMemento : List<PointF>, IEquatable<PointsMemento>
	{
		public PointsMemento() {}
		public PointsMemento(int capacity) : base(capacity) {}

		public override int GetHashCode()
		{
			int hashcode = -0x573C799C;
			foreach (PointF point in this)
			{
				hashcode ^= point.GetHashCode();
			}
			return hashcode;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointsMemento)
				return this.Equals((PointsMemento)obj);
			return false;
		}

		public bool Equals(PointsMemento other)
		{
			if (this == other)
				return true;
			if (other == null || this.Count != other.Count)
				return false;

			for(int i = 0; i < this.Count; i++)
			{
				if (this[i] != other[i])
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			const string separator = ", ";
			StringBuilder sb = new StringBuilder();
			sb.Append('{');
			foreach (PointF f in this)
			{
				sb.Append(f.ToString());
				sb.Append(separator);
			}
			if (this.Count > 0)
				sb.Remove(sb.Length - separator.Length, separator.Length);
			sb.Append('}');
			return sb.ToString();
		}
	}
}
