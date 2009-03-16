#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{

	#region UserTextGraphicAnnotationSerializer

	internal class UserTextGraphicAnnotationSerializer : GraphicAnnotationSerializer<UserTextGraphic>
	{
		protected override void Serialize(UserTextGraphic textGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			// if the callout is not visible, don't serialize it!
			if (!textGraphic.Visible)
				return;

			if (string.IsNullOrEmpty(textGraphic.Text))
				return;

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			textGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(textGraphic.BoundingBox);
				text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;
				text.UnformattedTextValue = textGraphic.Text;
			}
			finally
			{
				textGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}

	#endregion

	/// <summary>
	/// A graphical representation of user-editable text area.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (UserTextGraphicAnnotationSerializer))]
	public class UserTextGraphic : InteractiveGraphic, IMemorable
	{
		#region Private Fields

		private event EventHandler<PointChangedEventArgs> _locationChanged;
		private event EventHandler _textChanged;

		[CloneIgnore]
		private EditBox _currentCalloutEditBox;

		[CloneIgnore]
		private InvariantTextPrimitive _textGraphic;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="UserTextGraphic"/>.
		/// </summary>
		public UserTextGraphic() : base()
		{
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected UserTextGraphic(UserTextGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			if (_textGraphic == null)
			{
				_textGraphic = new InvariantTextPrimitive();
				this.Graphics.Add(_textGraphic);
			}

			_textGraphic.AnchorPointChanged += OnTextAnchorPointChanged;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_textGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                           delegate(IGraphic test) { return test is InvariantTextPrimitive; }) as InvariantTextPrimitive;

			Platform.CheckForNullReference(_textGraphic, "_textGraphic");

			Initialize();
		}

		public event EventHandler TextChanged
		{
			add { _textChanged += value; }
			remove { _textChanged -= value; }
		}

		public event EventHandler<PointChangedEventArgs> LocationChanged
		{
			add { _locationChanged += value; }
			remove { _locationChanged -= value; }
		}

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

		public PointF Location
		{
			get { return _textGraphic.AnchorPoint; }
			set { _textGraphic.AnchorPoint = value; }
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

		public override RectangleF BoundingBox
		{
			get { return _textGraphic.BoundingBox; }
		}

		protected virtual void OnTextChanged(EventArgs e)
		{
			EventsHelper.Fire(_textChanged, this, new EventArgs());
		}

		protected override void OnColorChanged()
		{
			base.OnColorChanged();

			_textGraphic.Color = base.Color;
		}

		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			// no control points
		}

		#region Edit Support

		/// <summary>
		/// Starts edit mode on the callout graphic by installing a <see cref="EditBox"/> on the
		/// <see cref="Tile"/> of the <see cref="Graphic.ParentPresentationImage">parent PresentationImage</see>.
		/// </summary>
		/// <returns>True if edit mode was successfully started; False otherwise.</returns>
		public bool StartEdit()
		{
			// remove any pre-existing edit boxes
			EndEdit();

			bool result = false;
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
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
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return result;
		}

		/// <summary>
		/// Ends edit mode on the callout graphic if it is currently being edited. Has no effect otherwise.
		/// </summary>
		public void EndEdit()
		{
			if (_currentCalloutEditBox != null)
			{
				_currentCalloutEditBox.ValueAccepted -= OnEditBoxComplete;
				_currentCalloutEditBox.ValueCancelled -= OnEditBoxComplete;
				_currentCalloutEditBox = null;
			}
			InstallEditBox(null);
		}

		private void InstallEditBox(EditBox editBox)
		{
			if (base.ParentPresentationImage != null)
			{
				if (base.ParentPresentationImage.Tile != null)
					base.ParentPresentationImage.Tile.EditBox = editBox;
			}
		}

		private void OnEditBoxComplete(object sender, EventArgs e)
		{
			if (_currentCalloutEditBox != null)
			{
				this.Text = _currentCalloutEditBox.LastAcceptedValue;
				this.Draw();
			}
			EndEdit();
		}

		#endregion

		#region IMemorable Members

		public override object CreateMemento()
		{
			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			PointMemento memento = new PointMemento(this.Location);
			this.ResetCoordinateSystem();

			return memento;
		}

		public override void SetMemento(object memento)
		{
			PointMemento pointMemento = (PointMemento) memento;

			this.CoordinateSystem = CoordinateSystem.Source;
			this.Location = pointMemento.Point;
			this.ResetCoordinateSystem();
		}

		#endregion

		public override void Move(SizeF delta)
		{
			_textGraphic.Move(delta);
		}

		public override bool HitTest(Point point)
		{
			return _textGraphic.HitTest(point);
		}

		public override PointF GetClosestPoint(PointF point)
		{
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

		private void OnTextAnchorPointChanged(object sender, PointChangedEventArgs e)
		{
			if (!FloatComparer.AreEqual(_textGraphic.AnchorPoint, e.Point))
			{
				EventsHelper.Fire(_locationChanged, this, e);
			}
		}
	}
}