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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive polygon graphic.
	/// </summary>
	[Cloneable]
	public class PolygonInteractiveGraphic : PolyLineInteractiveGraphic
	{
		private bool _polygonIsClosed = false;
		private bool _moveInProgress = false;

		/// <summary>
		/// Initializes a new instance of <see cref="PolygonInteractiveGraphic"/>
		/// </summary>
		/// <param name="userCreated">Indicates whether the graphic was created
		/// through user interaction.</param>
		public PolygonInteractiveGraphic(bool userCreated) : base(userCreated, int.MaxValue) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected PolygonInteractiveGraphic(PolygonInteractiveGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Get the number of vertices in the polygon.
		/// </summary>
		public int VertexCount
		{
			get { return this.PolyLine.Count - 1; }
		}

		/// <summary>
		/// Gets a value indicating if the polygon has been constructed and closed.
		/// </summary>
		/// <remarks>
		/// If the polygon is "open" then the polygon has not finished constructing yet, or is invalid.
		/// </remarks>
		public bool IsClosed
		{
			get { return _polygonIsClosed; }
		}

		/// <summary>
		/// Closes the polygon by connecting the last point with the first point and marking the graphic as closed.
		/// </summary>
		public void ClosePolygon()
		{
			if (_polygonIsClosed)
				throw new InvalidOperationException("The polygon is already closed.");

			_polygonIsClosed = true;

			this.PolyLine[this.PolyLine.Count - 1] = this.PolyLine[0];
		}

		/// <summary>
		/// Moves the <see cref="PolyLineInteractiveGraphic"/> by
		/// the specified delta.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
			_moveInProgress = true;
			try
			{
				base.Move(delta);
			}
			finally
			{
				_moveInProgress = false;
			}
		}

		/// <summary>
		/// Creates a creation <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		protected override GraphicState CreateCreateState()
		{
			return new CreatePolygonGraphicState(this);
		}

		/// <summary>
		/// Executed when the position of an anchor point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnAnchorPointChanged(object sender, ListEventArgs<PointF> e)
		{
			if (_polygonIsClosed && e.Index == this.VertexCount)
			{
				// both the first and last anchor points map to the first control point
				base.OnAnchorPointChanged(sender, new ListEventArgs<PointF>(e.Item, 0));
				return;
			}
			base.OnAnchorPointChanged(sender, e);
		}

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			if (_polygonIsClosed && !_moveInProgress && e.Index == 0)
			{
				// the first control point maps to both the first and last anchor points
				base.OnControlPointChanged(sender, new ListEventArgs<PointF>(e.Item, this.VertexCount));
			}
			base.OnControlPointChanged(sender, e);
		}
	}
}