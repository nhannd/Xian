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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IInteractiveGraphic : IGraphic, IMemorable
	{
		/// <summary>
		/// A group of control points.
		/// </summary>
		ControlPointGroup ControlPoints { get; }

		/// <summary>
		/// Gets or sets the colour of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Gets the graphic's tightest bounding box.
		/// </summary>
		RectangleF BoundingBox { get; }

		/// <summary>
		/// Gets the point on the graphic closest to the specified point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		PointF GetClosestPoint(PointF point);
	}

	/// <summary>
	/// A base class graphic that has a set of control points
	/// that can be manipulated by the user.
	/// </summary>
	[Cloneable]
	public abstract class InteractiveGraphic : CompositeGraphic, IInteractiveGraphic
	{
		[CloneIgnore]
		private ControlPointGroup _controlPointGroup;

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected InteractiveGraphic(InteractiveGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected InteractiveGraphic()
		{
			Initialize();
		}

		/// <summary>
		/// A group of control points.
		/// </summary>
		public ControlPointGroup ControlPoints
		{
			get { return _controlPointGroup; }
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		public virtual Color Color
		{
			get { return _controlPointGroup.Color; }
			set
			{
				if (_controlPointGroup.Color != value)
				{
					_controlPointGroup.Color = value;
					OnColorChanged();
				}
			}
		}

		protected virtual void OnColorChanged()
		{
		}

		/// <summary>
		/// Gets the graphic's tightest bounding box.
		/// </summary>
		public abstract RectangleF BoundingBox { get; }

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <returns></returns>
		public abstract object CreateMemento();

		/// <summary>
		/// Restores the state of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <param name="memento"></param>
		public abstract void SetMemento(object memento);

		#endregion

		
		/// <summary>
		/// Gets the point on the graphic closest to the specified point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract PointF GetClosestPoint(PointF point);

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected abstract void OnControlPointChanged(object sender, ListEventArgs<PointF> e);

		/// <summary>
		/// Releases all resources used by this <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_controlPointGroup.ControlPointChangedEvent -= new EventHandler<ListEventArgs<PointF>>(OnControlPointChanged);

			base.Dispose(disposing);
		}

		private void Initialize()
		{
			if (_controlPointGroup == null)
			{
				_controlPointGroup = new ControlPointGroup();
				base.Graphics.Add(_controlPointGroup);
			}

			// Make sure we know when the control points change
			_controlPointGroup.ControlPointChangedEvent += new EventHandler<ListEventArgs<PointF>>(OnControlPointChanged);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_controlPointGroup = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is ControlPointGroup; }) as ControlPointGroup;

			Platform.CheckForNullReference(_controlPointGroup, "_controlPointGroup");
			Initialize();
		}
	}
}
