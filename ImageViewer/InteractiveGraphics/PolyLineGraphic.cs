#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A polyline graphic.
	/// </summary>
	/// <remarks>
	/// A polyline is specified by a series of <i>n</i> anchor points, which
	/// are connected with a series of <i>n-1</i> line segments.
	/// </remarks>
	[Cloneable(true)]
	public class PolyLineGraphic 
		: CompositeGraphic, IMemorable
	{
		#region Private fields

		private int _numberOfPoints = 0;
		private Color _color = Color.Yellow;
		private event EventHandler<ListEventArgs<PointF>> _anchorPointChangedEvent;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="PolyLineGraphic"/>.
		/// </summary>
		public PolyLineGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="PolyLineGraphic"/>.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;

				foreach (LinePrimitive line in this.Graphics)
					line.Color = _color;
			}
		}

		/// <summary>
		/// Gets the number of anchor points in the <see cref="PolyLineGraphic"/>.
		/// </summary>
		public int Count
		{
			get { return _numberOfPoints; }
		}

		/// <summary>
		/// Occurs when an anchor point has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> AnchorPointChangedEvent
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		/// <summary>
		/// Adds a new anchor point to the <see cref="PolyLineGraphic"/>.
		/// </summary>
		/// <param name="point"></param>
		public void Add(PointF point)
		{
			_numberOfPoints++;

			if (this.Count == 1)
			{
				LinePrimitive line = new LinePrimitive();
				line.Color = this.Color;
				this.Graphics.Add(line);
				line.Pt1 = point;
			}
			else if (this.Count == 2)
			{
				((LinePrimitive)this.Graphics[0]).Pt2 = point;
			}
			else
			{
				int previousLineIndex = this.Graphics.Count - 1;
				LinePrimitive previousLine = ((LinePrimitive)this.Graphics[previousLineIndex]);
				LinePrimitive newLine = new LinePrimitive();
				newLine.Color = this.Color;
				this.Graphics.Add(newLine);
				newLine.Pt1 = previousLine.Pt2;
				newLine.Pt2 = point;
			}
		}

		/// <summary>
		/// Removes an anchor point from the <see cref="PolyLineGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method does not correctly handle the case for removing points from the middle of the polyline.
		/// </remarks>
		/// <param name="index">The zero-based index of the anchor point to remove.</param>
		public void RemoveAt(int index)
		{
			// TODO make it work properly when removing points from middle of polyline.
			if (index == 0)
			{
				this.Graphics.RemoveAt(index);
			}
			else if (index == 1)
			{
				// do nothing - removal of index=1 is just removal of pt2 of first line segment which must still be there.	
			}
			else {
				this.Graphics.RemoveAt(index-1);
			}
			_numberOfPoints--;
		}

		/// <summary>
		/// Gets or sets the location of the specified anchor point.
		/// </summary>
		/// <param name="index">The zero-based index of the anchor point.</param>
		/// <returns></returns>
		public PointF this[int index]
		{
			get
			{
				if (index == 0)
					return ((LinePrimitive)this.Graphics[0]).Pt1;
				else if (index == 1)
					return ((LinePrimitive)this.Graphics[0]).Pt2;
				else
					return ((LinePrimitive)this.Graphics[index - 1]).Pt2;
			}
			set
			{
				PointF pt;

				if (index == 0)
				{
					pt = ((LinePrimitive)this.Graphics[0]).Pt1;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[0]).Pt1 = value;
						NotifyListeners(0, value);
					}
				}
				else if (index == 1)
				{
					pt = ((LinePrimitive)this.Graphics[0]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[0]).Pt2 = value;
						NotifyListeners(1, value);

						if (this.Count > 2)
							((LinePrimitive)this.Graphics[1]).Pt1 = value;
					}
				}
				else
				{
					pt = ((LinePrimitive)this.Graphics[index - 1]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[index - 1]).Pt2 = value;
						NotifyListeners(index, value);

						if (index < this.Count - 1)
							((LinePrimitive)this.Graphics[index]).Pt1 = value;
					}
				}
			}
		}

		/// <summary>
		/// Removes all anchor points from the <see cref="PolyLineGraphic"/>.
		/// </summary>
		public void Clear()
		{
			this.Graphics.Clear();
			_numberOfPoints = 0;
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the polyline.
		/// </summary>
		/// <returns></returns>
		public virtual object CreateMemento()
		{
			PointsMemento memento = new PointsMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;

			for (int i = 0; i < this.Count; i++)
				memento.Add(this[i]);

			this.ResetCoordinateSystem();

			return memento;
		}

		/// <summary>
		/// Restores the state of the polyline.
		/// </summary>
		/// <param name="memento"></param>
		public virtual void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			PointsMemento anchorPointsMemento = memento as PointsMemento;
			Platform.CheckForInvalidCast(anchorPointsMemento, "memento", "PointsMemento");

			this.CoordinateSystem = CoordinateSystem.Source;

			int i = 0;

			foreach (PointF point in anchorPointsMemento)
			{
				this[i] = point;
				i++;
			}

			this.ResetCoordinateSystem();
		}

		#endregion

		/// <summary>
		/// Performs a hit test on the polyline.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			foreach (LinePrimitive line in this.Graphics)
			{
				if (line.HitTest(point))
					return true;
			}

			return false;
		}

		private void NotifyListeners(int anchorPointIndex, PointF anchorPoint)
		{
			EventsHelper.Fire(_anchorPointChangedEvent, this, new ListEventArgs<PointF>(anchorPoint, anchorPointIndex));
		}
	}
}
