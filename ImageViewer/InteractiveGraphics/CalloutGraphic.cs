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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
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
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof(CalloutGraphicAnnotationSerializer))]
	public class CalloutGraphic : InteractiveGraphic, IMemorable
	{
		#region Private fields

		private event EventHandler<PointChangedEventArgs> _locationChanged;
		private event EventHandler<EventArgs> _textChanged;

		[CloneIgnore]
		private InvariantTextPrimitive _textGraphic;
		[CloneIgnore]
		private ArrowGraphic _lineGraphic;
		private bool _enableControlPoint;

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
			_textGraphic.Text = text;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CalloutGraphic(CalloutGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the text label.
		/// </summary>
		public string Text
		{
			get { return _textGraphic.Text; }
			protected set
			{
				if (_textGraphic.Text != value)
				{
					_textGraphic.Text = value;
					OnTextChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the location of the center of the text label.
		/// </summary>
		public PointF Location
		{
			get { return base.ControlPoints[0]; }
			set { base.ControlPoints[0] = value; }
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
				_lineGraphic.EndPoint = value;
				SetCalloutLineStart();
			}
		}

		public bool EnableControlPoint
		{
			get { return _enableControlPoint; }
			set
			{
				if (_enableControlPoint != value)
				{
					_enableControlPoint = value;
					base.ControlPoints.Visible = _enableControlPoint && string.IsNullOrEmpty(_textGraphic.Text);
				}
			}
		}

		public string FontName
		{
			get { return _textGraphic.Font; }
			set { _textGraphic.Font = value; }
		}

		public float FontSize
		{
			get { return _textGraphic.SizeInPoints; }
			set { _textGraphic.SizeInPoints = value; }
		}

		public LineStyle LineStyle
		{
			get { return _lineGraphic.LineStyle; }
			set { _lineGraphic.LineStyle = value; }
		}

		public bool ShowArrow
		{
			get { return _lineGraphic.ShowArrowhead; }
			set { _lineGraphic.ShowArrowhead = value; }
		}

		protected override void OnColorChanged() 
		{
			base.OnColorChanged();

			_lineGraphic.Color = base.Color;
			_textGraphic.Color = base.Color;
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
		public override object CreateMemento()
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
        public override void SetMemento(object memento)
        {
			PointMemento pointMemento = (PointMemento)memento;

			this.CoordinateSystem = CoordinateSystem.Source;
			this.Location = pointMemento.Point;
			this.ResetCoordinateSystem();

			SetCalloutLineStart();
		}

		/// <summary>
		/// This method overrides <see cref="Graphic.Move"/>.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
			_textGraphic.Move(delta);
		}
 
		/// <summary>
		/// This method overrides <see cref="Graphic.HitTest"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		/// <remarks>
		/// A hit on either the text label or callout line consitutes
		/// a valid hit on the <see cref="CalloutGraphic"/>.
		/// </remarks>
		public override bool HitTest(Point point)
        {
			return _textGraphic.HitTest(point) || _lineGraphic.HitTest(point);
        }

		private void Initialize()
		{
			if (_textGraphic == null)
			{
				_textGraphic = new InvariantTextPrimitive();
				this.Graphics.Add(_textGraphic);
			}

			_textGraphic.AnchorPointChanged += OnTextAnchorPointChanged;
			_textGraphic.BoundingBoxChanged += OnTextBoundingBoxChanged;

			if (_lineGraphic == null)
			{
				_lineGraphic = new ArrowGraphic(false);
				this.Graphics.Add(_lineGraphic);
				_lineGraphic.LineStyle = LineStyle.Dash;
			}

			base.ControlPoints.Add(_textGraphic.AnchorPoint);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_textGraphic = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is InvariantTextPrimitive; }) as InvariantTextPrimitive;

			_lineGraphic = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is ArrowGraphic; }) as ArrowGraphic;

			Platform.CheckForNullReference(_lineGraphic, "_lineGraphic");
			Platform.CheckForNullReference(_textGraphic, "_textGraphic");

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
		public override PointF GetClosestPoint(PointF point) {
			RectangleF boundingBox = _textGraphic.BoundingBox;
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

		private void OnTextAnchorPointChanged(object sender, PointChangedEventArgs e) {
			if (!FloatComparer.AreEqual(base.ControlPoints[0], e.Point))
			{
				base.ControlPoints[0] = e.Point;
				EventsHelper.Fire(_locationChanged, this, e);
			}
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			SetCalloutLineStart();
		}

		protected virtual void OnTextChanged(EventArgs e) {
			base.ControlPoints.Visible = _enableControlPoint && string.IsNullOrEmpty(_textGraphic.Text);
			EventsHelper.Fire(_textChanged, this, new EventArgs());
		}

		/// <summary>
		/// Gets the bounding box for the text portion of the callout.
		/// </summary>
		public override RectangleF BoundingBox
		{
			get { return _textGraphic.BoundingBox; }
		}

		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			if(!FloatComparer.AreEqual(_textGraphic.AnchorPoint, e.Item))
			{
				_textGraphic.AnchorPoint = e.Item;
			}
		}
	}

	[Cloneable]
	public class UserCalloutGraphic : CalloutGraphic {

		[CloneIgnore]
		private EditBox _currentCalloutEditBox;

		/// <summary>
		/// Instantiates a new instance of <see cref="UserCalloutGraphic"/>.
		/// </summary>
		public UserCalloutGraphic() : base("") {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected UserCalloutGraphic(UserCalloutGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Starts edit mode on the callout graphic by installing a <see cref="EditBox"/> on the
		/// <see cref="Tile"/> of the <see cref="Graphic.ParentPresentationImage">parent PresentationImage</see>.
		/// </summary>
		/// <returns>True if edit mode was successfully started; False otherwise.</returns>
		public bool StartEdit() {
			// remove any pre-existing edit boxes
			EndEdit();

			bool result = false;
			this.CoordinateSystem = CoordinateSystem.Destination;
			try {
				EditBox editBox = new EditBox(this.Text ?? string.Empty);
				if (string.IsNullOrEmpty(this.Text))
					editBox.Value = SR.StringEnterText;
				editBox.Location = Point.Round(this.Location);
				editBox.Size = Rectangle.Round(this.BoundingBox).Size;
				editBox.FontName = this.FontName;
				editBox.FontSize = this.FontSize;
				editBox.ValueAccepted += OnEditBoxComplete;
				editBox.ValueCancelled += OnEditBoxComplete;
				InstallEditBox(_currentCalloutEditBox = editBox);
				result = true;
			} finally {
				this.ResetCoordinateSystem();
			}

			return result;
		}

		/// <summary>
		/// Ends edit mode on the callout graphic if it is currently being edited. Has no effect otherwise.
		/// </summary>
		public void EndEdit() {
			if (_currentCalloutEditBox != null) {
				_currentCalloutEditBox.ValueAccepted -= OnEditBoxComplete;
				_currentCalloutEditBox.ValueCancelled -= OnEditBoxComplete;
				_currentCalloutEditBox = null;
			}
			InstallEditBox(null);
		}

		private void InstallEditBox(EditBox editBox) {
			if (base.ParentPresentationImage != null) {
				if (base.ParentPresentationImage.Tile != null)
					base.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e) {
			if (_currentCalloutEditBox != null) {
				this.Text = _currentCalloutEditBox.LastAcceptedValue;
				this.Draw();
			}
			EndEdit();
		}
	}
}
