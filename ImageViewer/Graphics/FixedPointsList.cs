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