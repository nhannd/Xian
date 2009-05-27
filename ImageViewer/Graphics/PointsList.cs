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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	public sealed class PointsList : IPointsList
	{
		private readonly List<PointF> _sourcePoints = new List<PointF>();
		private readonly IGraphic _owner;
		private bool _enableEvents = true;

		public PointsList(IGraphic owner)
		{
			_owner = owner;
		}

		public PointsList(IEnumerable<PointF> points, IGraphic owner)
			: this(owner)
		{
			foreach (PointF point in points)
				_sourcePoints.Add(point);
		}

		public bool IsClosed
		{
			get { return _sourcePoints.Count > 2 && FloatComparer.AreEqual(_sourcePoints[0], _sourcePoints[_sourcePoints.Count - 1]); }
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

			return _sourcePoints.IndexOf(item);
		}

		public void Insert(int index, PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);

			_sourcePoints.Insert(index, item);
			NotifyPointAdded(index);
		}

		public void RemoveAt(int index)
		{
			_sourcePoints.RemoveAt(index);
			NotifyPointRemoved(index);
		}

		public PointF this[int index]
		{
			get
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					return _owner.SpatialTransform.ConvertToDestination(_sourcePoints[index]);
				return _sourcePoints[index];
			}
			set
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					value = _owner.SpatialTransform.ConvertToSource(value);

				if (!FloatComparer.AreEqual(_sourcePoints[index], value))
				{
					_sourcePoints[index] = value;
					NotifyPointChanged(index);
				}
			}
		}

		public void Add(PointF item)
		{
			this.Insert(_sourcePoints.Count, item);
		}

		public void Clear()
		{
			_sourcePoints.Clear();
			NotifyPointsCleared();
		}

		public bool Contains(PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);
			return _sourcePoints.Contains(item);
		}

		public void CopyTo(PointF[] array, int arrayIndex)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
			{
				foreach (PointF point in this)
					array[arrayIndex++] = point;

				return;
			}
			_sourcePoints.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _sourcePoints.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(PointF item)
		{
			int index = _sourcePoints.IndexOf(item);
			if (index < 0)
				return false;
			this.RemoveAt(index);
			return true;
		}

		private IEnumerator<PointF> EnumerateDestinationPoints()
		{
			foreach (PointF point in _sourcePoints)
				yield return _owner.SpatialTransform.ConvertToDestination(point);
		}

		public IEnumerator<PointF> GetEnumerator()
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				return this.EnumerateDestinationPoints();
			return _sourcePoints.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}