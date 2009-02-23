using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IStandardStatefulInteractiveGraphic : IStandardStatefulGraphic, IInteractiveGraphic
	{
		InteractiveGraphic InteractiveGraphic { get; }
	}

	// TODO: instead of this wrapping a single interactive graphic, can we make it a state controller for collections
	// of interactive graphics?

	//TODO: how do we set it's initial state (e.g. inactive).  Is it always up to client code?  Doing it in constructor wouldn't work right.
	//TODO: how do we control the initial 'entry point' state?  Should it be restricted to Create or Inactive?

	[Cloneable]
	public class StandardStatefulInteractiveGraphic : StandardStatefulCompositeGraphic, IStandardStatefulInteractiveGraphic, ICursorTokenProvider
	{
		protected static readonly Color DefaultFocusColor = Color.Orange;
		protected static readonly Color DefaultFocusSelectedColor = Color.Tomato;
		protected static readonly Color DefaultSelectedColor = Color.Tomato;
		protected static readonly Color DefaultInactiveColor = Color.Yellow;

		[CloneCopyReference]
		private Color _focusColor = DefaultFocusColor;
		[CloneCopyReference]
		private Color _focusSelectedColor = DefaultFocusSelectedColor;
		[CloneCopyReference]
		private Color _selectedColor = DefaultSelectedColor;
		[CloneCopyReference]
		private Color _inactiveColor = DefaultInactiveColor;

		[CloneCopyReference]
		private CursorToken _moveToken;
		[CloneCopyReference]
		private CursorToken _stretchingToken;
		private StretchCursorTokenStrategy _stretchCursorTokenStrategy;

		[CloneIgnore]
		private InteractiveGraphic _interactiveGraphic;


		public StandardStatefulInteractiveGraphic(InteractiveGraphic interactiveGraphic)
			: base()
		{
			base.Graphics.Add(_interactiveGraphic = interactiveGraphic);
			Initialize();
		}

		protected StandardStatefulInteractiveGraphic(StandardStatefulInteractiveGraphic source, ICloningContext context)
			: base()
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_interactiveGraphic = FindInteractiveGraphic();

			this.State = this.CreateInactiveState();

			Initialize();
		}

		private void Initialize()
		{
			if (_moveToken == null)
				_moveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);

			if (_stretchingToken == null)
				_stretchingToken = new CursorToken(CursorToken.SystemCursors.Cross);

			if (_stretchCursorTokenStrategy == null)
				_stretchCursorTokenStrategy = new CompassStretchCursorTokenStrategy();

			_stretchCursorTokenStrategy.TargetGraphic = this;
		}

		public Color FocusColor
		{
			get { return _focusColor; }
			set { _focusColor = value; }
		}

		public Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		public Color FocusSelectedColor
		{
			get { return _focusSelectedColor; }
			set { _focusSelectedColor = value; }
		}

		public Color InactiveColor
		{
			get { return _inactiveColor; }
			set { _inactiveColor = value; }
		}

		public InteractiveGraphic InteractiveGraphic
		{
			get { return _interactiveGraphic; }
		}

		protected bool IsStretching
		{
			get { return this.State is MoveControlPointGraphicState || this.State is CreateGraphicState; }
		}

		protected virtual InteractiveGraphic FindInteractiveGraphic()
		{
			return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is InteractiveGraphic; }) as InteractiveGraphic;
		}

		protected override void OnEnterInactiveState(IMouseInformation mouseInformation)
		{
			_interactiveGraphic.ControlPoints.Visible = false;
			_interactiveGraphic.Color = InactiveColor;

			base.OnEnterInactiveState(mouseInformation);
		}

		protected override void OnEnterFocusState(IMouseInformation mouseInformation)
		{
			_interactiveGraphic.ControlPoints.Visible = true;
			_interactiveGraphic.Color = FocusColor;

			base.OnEnterFocusState(mouseInformation);
		}

		protected override void OnEnterSelectedState(IMouseInformation mouseInformation)
		{
			_interactiveGraphic.ControlPoints.Visible = false;
			_interactiveGraphic.Color = FocusSelectedColor;

			base.OnEnterSelectedState(mouseInformation);
		}

		protected override void OnEnterFocusSelectedState(IMouseInformation mouseInformation)
		{
			_interactiveGraphic.ControlPoints.Visible = true;
			_interactiveGraphic.Color = FocusSelectedColor;

			base.OnEnterFocusSelectedState(mouseInformation);
		}

		protected override void OnEnterCreateState(IMouseInformation mouseInformation)
		{
			_interactiveGraphic.ControlPoints.Visible = true;
			_interactiveGraphic.Color = FocusSelectedColor;

			base.OnEnterCreateState(mouseInformation);
		}

		public override GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedInteractiveGraphicState(this);
		}

		#region ICursorTokenProvider

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.		
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public virtual CursorToken GetCursorToken(Point point)
		{
			if (IsStretching)
				return StretchingToken;

			if (ControlPoints.HitTest(point))
			{
				if (StretchCursorTokenStrategy != null)
					return StretchCursorTokenStrategy.GetCursorToken(point);
			}

			if (InteractiveGraphic.HitTest(point))
				return MoveToken;

			return null;
		}

		#endregion

		#region IInteractiveGraphic Members

		public ControlPointGroup ControlPoints
		{
			get { return this.InteractiveGraphic.ControlPoints; }
		}

		public Color Color
		{
			get { return this.InteractiveGraphic.Color; }
		}

		Color IInteractiveGraphic.Color
		{
			get { return this.Color; }
			set { throw new InvalidOperationException("The colour is set internally as the state changes."); }
		}

		public RectangleF BoundingBox
		{
			get { return this.InteractiveGraphic.BoundingBox; }
		}

		public PointF GetClosestPoint(PointF point)
		{
			return this.InteractiveGraphic.GetClosestPoint(point);
		}

		#endregion

		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		public CursorToken StretchingToken
		{
			get { return _stretchingToken; }
			set { _stretchingToken = value; }
		}

		public StretchCursorTokenStrategy StretchCursorTokenStrategy
		{
			get { return _stretchCursorTokenStrategy; }
			set { _stretchCursorTokenStrategy = value; }
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			return this.InteractiveGraphic.CreateMemento();
		}

		public void SetMemento(object memento)
		{
			this.InteractiveGraphic.SetMemento(memento);
		}

		#endregion
	}

	[Cloneable]
	public class StandardStatefulInteractiveGraphic<T> : StandardStatefulInteractiveGraphic, IStandardStatefulInteractiveGraphic where T : InteractiveGraphic, new()
	{
		public StandardStatefulInteractiveGraphic() : base(new T()) { }

		public StandardStatefulInteractiveGraphic(T interactiveGraphic) : base(interactiveGraphic) { }

		protected StandardStatefulInteractiveGraphic(StandardStatefulInteractiveGraphic<T> source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new T InteractiveGraphic
		{
			get { return (T)base.InteractiveGraphic; }
		}

		InteractiveGraphic IStandardStatefulInteractiveGraphic.InteractiveGraphic
		{
			get { return base.InteractiveGraphic; }
		}

		protected override InteractiveGraphic FindInteractiveGraphic()
		{
			return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic test) { return test is T; }) as T;
		}
	}
}