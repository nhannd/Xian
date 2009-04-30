#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
