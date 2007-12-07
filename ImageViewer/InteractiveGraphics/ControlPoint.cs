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
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A graphical representation of the "handles" that allow 
	/// the user to move and resize <see cref="InteractiveGraphic"/>s.
	/// </summary>
	public class ControlPoint : CompositeGraphic
	{
		#region Private fields

		private int _index;
		private PointF _location;
		private InvariantRectanglePrimitive _rectangle;
		private event EventHandler<CollectionEventArgs<PointF>> _locationChangedEvent;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="index">The index at which a <see cref="ControlPoint"/>
		/// is inserted into a <see cref="ControlPointGroup"/>.
		/// </param>
		internal ControlPoint(int index)
		{
			Platform.CheckNonNegative(index, "index");

			_index = index;
			_rectangle = new InvariantRectanglePrimitive();
			_rectangle.InvariantTopLeft = new PointF(-4, -4);
			_rectangle.InvariantBottomRight = new PointF(4, 4);
			this.Graphics.Add(_rectangle);
		}

		/// <summary>
		/// Gets or sets the location of the control point.
		/// </summary>
		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _location;
				else
					return base.SpatialTransform.ConvertToDestination(_location);
			}
			set
			{
				if (!FloatComparer.AreEqual(this.Location, value))
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					if (base.CoordinateSystem == CoordinateSystem.Source)
						_location = value;
					else
						_location = base.SpatialTransform.ConvertToSource(value);

					Trace.Write(String.Format("Control Point: {0}\n", _location.ToString()));

					_rectangle.AnchorPoint = this.Location;
					EventsHelper.Fire(_locationChangedEvent, this, new CollectionEventArgs<PointF>(this.Location, _index));
				}
			}
		}

		/// <summary>
		/// Gets or sets the colour of the control point.
		/// </summary>
		public Color Color
		{
			get { return _rectangle.Color; }
			set { _rectangle.Color = value; }
		}
	
		/// <summary>
		/// Occurs when the location of the control point has changed.
		/// </summary>
		public event EventHandler<CollectionEventArgs<PointF>> LocationChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// This method overrides <see cref="Graphic.HitTest"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return _rectangle.HitTest(point);
		}
	}
}
