using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics {
	public delegate GraphicState CreateGraphicStateDelegate(IStandardStatefulGraphic statefulGraphic);

	public interface IStandardStatefulInteractiveGraphic : IStandardStatefulGraphic, IInteractiveGraphic {
		InteractiveGraphic InteractiveGraphic { get; }
	}

	[Cloneable]
	public class StandardStatefulInteractiveGraphic : StandardStatefulCompositeGraphic, IStandardStatefulInteractiveGraphic
	{
		[CloneIgnore]
		private InteractiveGraphic _interactiveGraphic;

		private CreateGraphicStateDelegate _createInactiveStateDelegate;
		private CreateGraphicStateDelegate _createSelectedStateDelegate;
		private CreateGraphicStateDelegate _createFocusedStateDelegate;
		private CreateGraphicStateDelegate _createFocusedSelectedStateDelegate;

		public StandardStatefulInteractiveGraphic(InteractiveGraphic interactiveGraphic) {
			base.Graphics.Add(_interactiveGraphic = interactiveGraphic);
		}

		protected StandardStatefulInteractiveGraphic(StandardStatefulInteractiveGraphic source, ICloningContext context) {
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete() {
			_interactiveGraphic = FindInteractiveGraphic();

			this.State = this.CreateInactiveState();
		}

		protected virtual InteractiveGraphic FindInteractiveGraphic()
		{
			return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is InteractiveGraphic; }) as InteractiveGraphic;
		}

		public InteractiveGraphic InteractiveGraphic {
			get { return _interactiveGraphic; }
		}

		public CreateGraphicStateDelegate CreateInactiveStateDelegate
		{
			get { return _createInactiveStateDelegate; }
			set { _createInactiveStateDelegate = value; }
		}

		public CreateGraphicStateDelegate CreateSelectedStateDelegate
		{
			get { return _createSelectedStateDelegate; }
			set { _createSelectedStateDelegate = value; }
		}

		public CreateGraphicStateDelegate CreateFocusedStateDelegate
		{
			get { return _createFocusedStateDelegate; }
			set { _createFocusedStateDelegate = value; }
		}

		public CreateGraphicStateDelegate CreateFocusedSelectedStateDelegate
		{
			get { return _createFocusedSelectedStateDelegate; }
			set { _createFocusedSelectedStateDelegate = value; }
		}

		protected override void OnEnterInactiveState(IMouseInformation mouseInformation) {
			_interactiveGraphic.ControlPoints.Visible = false;
			_interactiveGraphic.Color = Color.Yellow;

			base.OnEnterInactiveState(mouseInformation);
		}

		protected override void OnEnterFocusState(IMouseInformation mouseInformation) {
			_interactiveGraphic.ControlPoints.Visible = true;
			_interactiveGraphic.Color = Color.Orange;

			base.OnEnterFocusState(mouseInformation);
		}

		protected override void OnEnterSelectedState(IMouseInformation mouseInformation) {
			_interactiveGraphic.ControlPoints.Visible = false;
			_interactiveGraphic.Color = Color.Tomato;

			base.OnEnterSelectedState(mouseInformation);
		}

		protected override void OnEnterFocusSelectedState(IMouseInformation mouseInformation) {
			_interactiveGraphic.ControlPoints.Visible = true;
			_interactiveGraphic.Color = Color.Tomato;

			base.OnEnterFocusSelectedState(mouseInformation);
		}

		public override GraphicState CreateInactiveState() {
			if (_createInactiveStateDelegate != null)
				return _createInactiveStateDelegate(this);
			return base.CreateInactiveState();
		}

		public override GraphicState CreateSelectedState() {
			if (_createSelectedStateDelegate != null)
				return _createSelectedStateDelegate(this);
			return base.CreateSelectedState();
		}

		public override GraphicState CreateFocussedState() {
			if (_createFocusedStateDelegate != null)
				return _createFocusedStateDelegate(this);
			return base.CreateFocussedState();
		}

		public override GraphicState CreateFocussedSelectedState() {
			if (_createFocusedSelectedStateDelegate != null)
				return _createFocusedSelectedStateDelegate(this);
			return base.CreateFocussedSelectedState();
		}

		#region IInteractiveGraphic Members

		public ControlPointGroup ControlPoints {
			get { return this.InteractiveGraphic.ControlPoints; }
		}

		public Color Color {
			get { return this.InteractiveGraphic.Color; }
			set { this.InteractiveGraphic.Color = value; }
		}

		public RectangleF BoundingBox {
			get { return this.InteractiveGraphic.BoundingBox; }
		}

		public ClearCanvas.Desktop.CursorToken StretchingToken {
			get { return this.InteractiveGraphic.StretchingToken; }
			set { this.InteractiveGraphic.StretchingToken = value; }
		}

		public StretchCursorTokenStrategy StretchCursorTokenStrategy {
			get { return this.InteractiveGraphic.StretchCursorTokenStrategy; }
			set { this.InteractiveGraphic.StretchCursorTokenStrategy = value; }
		}

		public PointF GetClosestPoint(PointF point) {
			return this.InteractiveGraphic.GetClosestPoint(point);
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento() {
			return this.InteractiveGraphic.CreateMemento();
		}

		public void SetMemento(object memento) {
			this.InteractiveGraphic.SetMemento(memento);
		}

		#endregion
	}

	[Cloneable]
	public class StandardStatefulInteractiveGraphic<T> : StandardStatefulInteractiveGraphic, IStandardStatefulInteractiveGraphic where T : InteractiveGraphic, new() {
		public StandardStatefulInteractiveGraphic() : base(new T()) {}

		public StandardStatefulInteractiveGraphic(T interactiveGraphic) : base(interactiveGraphic) {}

		protected StandardStatefulInteractiveGraphic(StandardStatefulInteractiveGraphic<T> source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new T InteractiveGraphic {
			get { return (T)base.InteractiveGraphic; }
		}

		InteractiveGraphic IStandardStatefulInteractiveGraphic.InteractiveGraphic
		{
			get { return base.InteractiveGraphic; }
		}

		protected override InteractiveGraphic FindInteractiveGraphic() {
			return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is T; }) as T;
		}
	}
}