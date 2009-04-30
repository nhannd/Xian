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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A group of <see cref="ControlPoint"/>s. 
	/// </summary>
	[Cloneable(true)]
	public class ControlPointGroup : CompositeGraphic
	{
		private Color _color = Color.Yellow;
		private event EventHandler<ListEventArgs<PointF>> _controlPointChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="ControlPointGroup"/>.
		/// </summary>
		public ControlPointGroup()
		{
		}

		/// <summary>
		/// Occurs when the location of a <see cref="ControlPoint"/> has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="ControlPointGroup"/>.
		/// </summary>
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

		/// <summary>
		/// Returns the number of <see cref="ControlPoint"/>s in the
		/// <see cref="ControlPointGroup"/>.
		/// </summary>
		public int Count
		{
			get
			{
				return this.Graphics.Count;
			}
		}

		/// <summary>
		/// Gets or sets the location of the specified <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ControlPoint"/>.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Adds a new <see cref="ControlPoint"/> to the
		/// <see cref="ControlPointGroup"/>.
		/// </summary>
		/// <param name="point"></param>
		public void Add(PointF point)
		{
			ControlPoint controlPoint = new ControlPoint();
			this.Graphics.Add(controlPoint);
			controlPoint.Location = point;
			controlPoint.Color = this.Color;
			controlPoint.LocationChanged += OnControlPointChanged;
		}

		/// <summary>
		/// Removes a <see cref="ControlPoint"/> from the group.
		/// </summary>
		public void RemoveAt(int index)
		{
			if (index < Count)
			{
				ControlPoint point = base.Graphics[index] as ControlPoint;
				if (point != null)
					point.LocationChanged -= OnControlPointChanged;

				base.Graphics.RemoveAt(index);
			}
		}

		/// <summary>
		/// Removes all <see cref="ControlPoint"/>s from the <see cref="ControlPointGroup"/>.
		/// </summary>
		public void Clear()
		{
			for (int i = this.Count - 1; i >= 0; --i)
				RemoveAt(i);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="ControlPoint"/>s
		/// in the <see cref="ControlPointGroup"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>The index of the <see cref="ControlPoint"/> or
		/// -1 if the hit test failed for all <see cref="ControlPoint"/>s.</returns>
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

		/// <summary>
		/// Releases all resources used by this <see cref="ControlPointGroup"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (ControlPoint controlPoint in this.Graphics)
					controlPoint.LocationChanged -= OnControlPointChanged;
			}

			base.Dispose(disposing);
		}

		private void OnControlPointChanged(object sender, EventArgs e)
		{
			ControlPoint controlPoint = (ControlPoint) sender;
			EventsHelper.Fire(_controlPointChangedEvent, this, new ListEventArgs<PointF>(controlPoint.Location, this.Graphics.IndexOf(controlPoint)));
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			foreach (ControlPoint controlPoint in this.Graphics)
				controlPoint.LocationChanged += OnControlPointChanged;
		}
	}
}
