#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	// TODO (Norman): Remove IObservableList interface; retain other convenience
	// methods like Add, Remove, etc.

	/// <summary>
	/// A collection of 
	/// </summary>
	public class ControlPointGroup : CompositeGraphic, IObservableList<PointF, ControlPointEventArgs>
	{
		private event EventHandler<ControlPointEventArgs> _controlPointChangedEvent;
		private Color _color = Color.Yellow;

		public ControlPointGroup()
		{
		}

		public event EventHandler<ControlPointEventArgs> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		public Color Color
		{
			get { return _color; }
			set 
			{
				_color = value;

				foreach (ControlPoint controlPoint in this.Graphics)
					controlPoint.Color = _color;
			}
		}

		#region IObservableCollection<PointF,ControlPointEventArgs> Members

		public event EventHandler<ControlPointEventArgs> ItemAdded
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		public event EventHandler<ControlPointEventArgs> ItemRemoved
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		public event EventHandler<ControlPointEventArgs> ItemChanged
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		#endregion

		#region IList<PointF> Members

		public int IndexOf(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Insert(int index, PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public PointF this[int index]
		{
			get
			{
				return ((ControlPoint)this.Graphics[index]).Location;
			}
			set
			{
				((ControlPoint)this.Graphics[index]).Location = value;
			}
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF point)
		{
			int controlPointIndex = this.Count;
			ControlPoint controlPoint = new ControlPoint(controlPointIndex);
			this.Graphics.Add(controlPoint);
			controlPoint.Location = point;
			controlPoint.Color = this.Color;
			controlPoint.LocationChanged += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}

		public void Clear()
		{
			this.Graphics.Clear();
		}

		public bool Contains(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CopyTo(PointF[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int Count
		{
			get
			{
				return this.Graphics.Count;
			}
		}

		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool Remove(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<PointF> Members

		public IEnumerator<PointF> GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool HitTest(Point point)
		{
			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint != null)
					if (controlPoint.HitTest(point))
						return true;
			}

			return false;
		}

		public int HitTestControlPoint(Point point)
		{
			int controlPointIndex = 0;

			// Check if mouse is over a control point
			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint.HitTest(point))
					return controlPointIndex;

				controlPointIndex++;
			}

			return -1;
		}

		private void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			EventsHelper.Fire(_controlPointChangedEvent, this, e);
		}

	}
}
