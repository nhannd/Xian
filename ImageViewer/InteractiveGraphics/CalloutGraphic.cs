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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A graphical representation of a callout.
	/// </summary>
	/// <remarks>
	/// A callout can be used to label something in the scene graph. It is
	/// composed of a text label and a line that extends from the label
	/// to some user defined point in the scene.
	/// </remarks>
	[DicomSerializableGraphicAnnotation(typeof (CalloutGraphicAnnotationSerializer))]
	[Cloneable]
	public class CalloutGraphic : CompositeGraphic, ITextGraphic, IMemorable, IMouseButtonHandler, IExportedActionsProvider, ICursorTokenProvider
	{
		#region Private fields

		private event EventHandler<PointChangedEventArgs> _locationChanged;
		private event EventHandler<EventArgs> _textChanged;

		private Color _color = Color.Yellow;

		[CloneIgnore]
		private IControlGraphic _textControlGraphic;

		[CloneIgnore]
		private ArrowGraphic _lineGraphic;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="CalloutGraphic"/>.
		/// </summary>
		protected CalloutGraphic() : base()
		{
			Initialize();
		}

		/// <summary>
		/// Instantiates a new instance of <see cref="CalloutGraphic"/>.
		/// </summary>
		/// <param name="text">The label text to display on the callout.</param>
		public CalloutGraphic(string text) : this()
		{
			this.Text = text;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CalloutGraphic(CalloutGraphic source, ICloningContext context)
			: base()
		{
			context.CloneFields(source, this);
		}

		protected InvariantTextPrimitive TextGraphic
		{
			get { return _textControlGraphic.Subject as InvariantTextPrimitive; }
		}

		protected IControlGraphic TextControlGraphic
		{
			get { return _textControlGraphic; }
		}

		/// <summary>
		/// Gets the text label.
		/// </summary>
		public string Text
		{
			get { return this.TextGraphic.Text; }
			protected set
			{
				if (this.TextGraphic.Text != value)
				{
					this.TextGraphic.Text = value;
					OnTextChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the location of the center of the text label.
		/// </summary>
		public PointF Location
		{
			get { return this.TextGraphic.AnchorPoint; }
			set { this.TextGraphic.AnchorPoint = value; }
		}

		/// <summary>
		/// Gets the starting point of the callout line.
		/// </summary>
		/// <remarks>
		/// The starting point of the callout line is automatically
		/// calculated so that it appears as though it starts
		/// from the center of the text label.
		/// </remarks>
		public PointF StartPoint
		{
			get { return _lineGraphic.StartPoint; }
		}

		/// <summary>
		/// Gets or sets the ending point of the callout line.
		/// </summary>
		public PointF EndPoint
		{
			get { return _lineGraphic.EndPoint; }
			set
			{
				if (!FloatComparer.AreEqual(_lineGraphic.EndPoint, value))
				{
					_lineGraphic.EndPoint = value;
					SetCalloutLineStart();
					OnEndPointChanged();
				}
			}
		}

		public string FontName
		{
			get { return this.TextGraphic.Font; }
			set { this.TextGraphic.Font = value; }
		}

		public float FontSize
		{
			get { return this.TextGraphic.SizeInPoints; }
			set { this.TextGraphic.SizeInPoints = value; }
		}

		public LineStyle LineStyle
		{
			get { return _lineGraphic.LineStyle; }
			set { _lineGraphic.LineStyle = value; }
		}

		public bool ShowArrowhead
		{
			get { return _lineGraphic.ShowArrowhead; }
			set { _lineGraphic.ShowArrowhead = value; }
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					OnColorChanged();
				}
			}
		}

		protected virtual void OnColorChanged()
		{
			_lineGraphic.Color = this.Color;
			_textControlGraphic.Color = this.Color;
		}

		/// <summary>
		/// Occurs when the location of the label portion of the callout
		/// has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> LocationChanged
		{
			add { _locationChanged += value; }
			remove { _locationChanged -= value; }
		}

		/// <summary>
		/// Creates a memento of this object.
		/// </summary>
		/// <returns></returns>
		public virtual object CreateMemento()
		{
			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			PointMemento memento = new PointMemento(this.Location);
			this.ResetCoordinateSystem();

			return memento;
		}

		/// <summary>
		/// Sets a memento for this object.
		/// </summary>
		/// <param name="memento"></param>
		public virtual void SetMemento(object memento)
		{
			PointMemento pointMemento = (PointMemento) memento;

			this.CoordinateSystem = CoordinateSystem.Source;
			this.Location = pointMemento.Point;
			this.ResetCoordinateSystem();

			SetCalloutLineStart();
		}

		private void Initialize()
		{
			if (_textControlGraphic == null)
			{
				_textControlGraphic = InitializeTextControlGraphic(new InvariantTextPrimitive());
				_textControlGraphic.Name = "Text";
				this.Graphics.Add(_textControlGraphic);
			}

			this.TextGraphic.AnchorPointChanged += OnTextAnchorPointChanged;
			this.TextGraphic.BoundingBoxChanged += OnTextBoundingBoxChanged;

			if (_lineGraphic == null)
			{
				_lineGraphic = new ArrowGraphic(false);
				_lineGraphic.Name = "Line";
				this.Graphics.Add(_lineGraphic);
				_lineGraphic.LineStyle = LineStyle.Dash;
			}
		}

		protected virtual IControlGraphic InitializeTextControlGraphic(IGraphic textGraphic)
		{
			return new MoveControlGraphic(textGraphic);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_textControlGraphic = (CollectionUtils.SelectFirst(base.Graphics,
			                                                   delegate(IGraphic test) { return test.Name == "Text"; }) as IControlGraphic);

			_lineGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                           delegate(IGraphic test) { return test.Name == "Line"; }) as ArrowGraphic;

			Platform.CheckForNullReference(_lineGraphic, "_lineGraphic");
			Platform.CheckForNullReference(_textControlGraphic, "_textControlGraphic");

			Initialize();
		}

		private void SetCalloutLineStart()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			this._lineGraphic.StartPoint = GetClosestPoint(this.EndPoint);
			this.ResetCoordinateSystem();
		}

		/// <summary>
		/// Gets the point on the graphic closest to the specified point in either source or destination coordinates.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override PointF GetClosestPoint(PointF point)
		{
			RectangleF boundingBox = _textControlGraphic.BoundingBox;
			boundingBox.Inflate(3, 3);

			PointF topLeft = new PointF(boundingBox.Left, boundingBox.Top);
			PointF topRight = new PointF(boundingBox.Right, boundingBox.Top);
			PointF bottomLeft = new PointF(boundingBox.Left, boundingBox.Bottom);
			PointF bottomRight = new PointF(boundingBox.Right, boundingBox.Bottom);
			PointF geoCenter = Vector.Midpoint(topLeft, bottomRight);
			PointF intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topLeft, topRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, bottomLeft, bottomRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topLeft, bottomLeft, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topRight, bottomRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			return point;
		}

		private void OnTextAnchorPointChanged(object sender, PointChangedEventArgs e)
		{
			EventsHelper.Fire(_locationChanged, this, e);
			NotifyPropertyChanged("Location");
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			SetCalloutLineStart();
		}

		protected virtual void OnTextChanged(EventArgs e)
		{
			EventsHelper.Fire(_textChanged, this, new EventArgs());
			NotifyPropertyChanged("Text");
		}

		protected virtual void OnEndPointChanged()
		{
			NotifyPropertyChanged("EndPoint");
		}

		/// <summary>
		/// Gets the bounding box for the text portion of the callout.
		/// </summary>
		public override RectangleF BoundingBox
		{
			get { return _textControlGraphic.BoundingBox; }
		}

		#region IMouseButtonHandler Members

		bool IMouseButtonHandler.Start(IMouseInformation mouseInformation)
		{
			return _textControlGraphic.Start(mouseInformation);
		}

		bool IMouseButtonHandler.Track(IMouseInformation mouseInformation)
		{
			return _textControlGraphic.Track(mouseInformation);
		}

		bool IMouseButtonHandler.Stop(IMouseInformation mouseInformation)
		{
			return _textControlGraphic.Stop(mouseInformation);
		}

		void IMouseButtonHandler.Cancel()
		{
			_textControlGraphic.Cancel();
		}

		MouseButtonHandlerBehaviour IMouseButtonHandler.Behaviour
		{
			get { return _textControlGraphic.Behaviour; }
		}

		#endregion

		#region IExportedActionsProvider Members

		protected virtual IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			if (_textControlGraphic.HitTest(mouseInformation.Location))
				return _textControlGraphic.GetExportedActions(site, mouseInformation);
			return null;
		}

		IActionSet IExportedActionsProvider.GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return this.OnGetExportedActions(site, mouseInformation);
		}

		#endregion

		#region ICursorTokenProvider Members

		protected virtual CursorToken OnGetCursorToken(Point point)
		{
			return _textControlGraphic.GetCursorToken(point);
		}

		CursorToken ICursorTokenProvider.GetCursorToken(Point point)
		{
			return this.OnGetCursorToken(point);
		}

		#endregion

		#region ITextGraphic Members

		string ITextGraphic.Text
		{
			get { return this.Text; }
			set { throw new NotSupportedException(); }
		}

		float ITextGraphic.SizeInPoints
		{
			get { return this.FontSize; }
			set { this.FontSize = value; }
		}

		string ITextGraphic.Font
		{
			get { return this.FontName; }
			set { this.FontName = value; }
		}

		SizeF ITextGraphic.Dimensions
		{
			get { return this.BoundingBox.Size; }
		}

		#endregion
	}
}