using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	public sealed class IndexEventArgs : EventArgs
	{
		public readonly int Index;

		public IndexEventArgs(int index)
		{
			this.Index = index;
		}
	}

	public class PointsList : IList<PointF>
	{
		private readonly List<PointF> _innerList = new List<PointF>();
		private readonly IGraphic _owner;
		private bool _enableEvents = true;

		public PointsList(IGraphic owner)
		{
			_owner = owner;
		}

		public PointsList Clone(IGraphic owner)
		{
			PointsList clone = new PointsList(owner);
			foreach (PointF point in _innerList)
				clone._innerList.Add(point);
			return clone;
		}

		public bool IsClosed
		{
			get { return _innerList.Count > 2 && FloatComparer.AreEqual(_innerList[0], _innerList[_innerList.Count - 1]); }
		}

		public void SuspendEvents()
		{
			_enableEvents = false;
		}

		public void ResumeEvents()
		{
			_enableEvents = true;
		}

		public event EventHandler<IndexEventArgs> PointAdded;
		public event EventHandler<IndexEventArgs> PointChanged;
		public event EventHandler<IndexEventArgs> PointRemoved;
		public event EventHandler PointsCleared;

		private void NotifyPointAdded(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointAdded, this, new IndexEventArgs(index));
		}

		private void NotifyPointRemoved(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointRemoved, this, new IndexEventArgs(index));
		}

		private void NotifyPointChanged(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointChanged, this, new IndexEventArgs(index));
		}

		private void NotifyPointsCleared()
		{
			if (_enableEvents)
				EventsHelper.Fire(PointsCleared, this, new EventArgs());
		}

		public int IndexOf(PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);

			return _innerList.IndexOf(item);
		}

		public void Insert(int index, PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);

			_innerList.Insert(index, item);
			NotifyPointAdded(index);
		}

		public void RemoveAt(int index)
		{
			_innerList.RemoveAt(index);
			NotifyPointRemoved(index);
		}

		public PointF this[int index]
		{
			get
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					return _owner.SpatialTransform.ConvertToDestination(_innerList[index]);
				return _innerList[index];
			}
			set
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					value = _owner.SpatialTransform.ConvertToSource(value);

				if (!FloatComparer.AreEqual(_innerList[index], value))
				{
					_innerList[index] = value;
					NotifyPointChanged(index);
				}
			}
		}

		public void Add(PointF item)
		{
			this.Insert(_innerList.Count, item);
		}

		public void Clear()
		{
			_innerList.Clear();
			NotifyPointsCleared();
		}

		public bool Contains(PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);
			return _innerList.Contains(item);
		}

		public void CopyTo(PointF[] array, int arrayIndex)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
			{
				foreach (PointF point in this)
					array[arrayIndex++] = point;

				return;
			}
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PointF item)
		{
			int index = _innerList.IndexOf(item);
			if (index < 0)
				return false;
			this.RemoveAt(index);
			return true;
		}

		private IEnumerator<PointF> EnumerateDestPoints()
		{
			foreach (PointF point in _innerList)
				yield return _owner.SpatialTransform.ConvertToDestination(point);
		}

		public IEnumerator<PointF> GetEnumerator()
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				return this.EnumerateDestPoints();
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}