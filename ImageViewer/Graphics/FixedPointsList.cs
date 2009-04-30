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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class FixedPointsList : IList<PointF>
	{
		public delegate PointF PointGetter(int index);

		public delegate void PointSetter(int index, PointF point);

		public delegate int CountGetter();

		private readonly PointGetter _getter;
		private readonly PointSetter _setter;
		private readonly CountGetter _counter;

		public FixedPointsList(PointGetter getter, PointSetter setter, int count) : this(getter, setter, delegate { return count; }) { }

		public FixedPointsList(PointGetter getter, PointSetter setter, CountGetter counter)
		{
			_getter = getter;
			_setter = setter;
			_counter = counter;
		}

		#region IList<PointF> Members

		public int IndexOf(PointF item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, PointF item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public PointF this[int index]
		{
			get { return _getter(index); }
			set { _setter(index, value); }
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(PointF item)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(PointF[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				if (_counter == null)
					throw new NotSupportedException();
				return _counter();
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PointF item)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<PointF> Members

		public IEnumerator<PointF> GetEnumerator()
		{
			int count = this.Count;
			for (int n = 0; n < count; n++)
				yield return this[n];
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}