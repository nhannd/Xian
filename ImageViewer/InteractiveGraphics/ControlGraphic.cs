using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IControlGraphic : IDecoratorGraphic, ICursorTokenProvider, IMouseButtonHandler, IExportedActionsProvider
	{
		IGraphic Subject { get; }
		event EventHandler SubjectChanged;
		Color Color { get; set; }
		bool ShowControlGraphics { get; set; }
		IMouseButtonHandler CurrentHandler { get;}
	}

	[Cloneable]
	public abstract class ControlGraphic : DecoratorCompositeGraphic, IControlGraphic
	{
		private event EventHandler _subjectChanged;
		private Color _color = Color.Yellow;
		private bool _showControlGraphics = true;

		[CloneIgnore]
		private bool _notifyOnSubjectChanged = true;

		[CloneIgnore]
		private IMouseButtonHandler _capturedHandler = null;

		[CloneIgnore]
		private PointF _lastTrackedPosition = PointF.Empty;

		[CloneIgnore]
		private bool _isTracking = false;

		protected ControlGraphic(IGraphic subject) : base(subject)
		{
			Initialize();
		}

		protected ControlGraphic(ControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.PropertyChanged += OnSubjectPropertyChanged;
		}

		protected override void Dispose(bool disposing) {
			this.Subject.PropertyChanged -= OnSubjectPropertyChanged;

			base.Dispose(disposing);
		}

		public IGraphic Subject
		{
			get
			{
				if (this.DecoratedGraphic is IControlGraphic)
					return ((IControlGraphic) this.DecoratedGraphic).Subject;
				return this.DecoratedGraphic;
			}
		}

		public IMouseButtonHandler CurrentHandler
		{
			get
			{
				if (_capturedHandler == null)
					return this;
				if (_capturedHandler is IControlGraphic)
					return ((IControlGraphic) _capturedHandler).CurrentHandler;
				return _capturedHandler;
			}
		}

		public event EventHandler SubjectChanged
		{
			add { _subjectChanged += value; }
			remove { _subjectChanged -= value; }
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					this.OnColorChanged();
				}
			}
		}

		public bool ShowControlGraphics
		{
			get { return _showControlGraphics; }
			set
			{
				if (_showControlGraphics != value)
				{
					_showControlGraphics = value;
					this.OnShowControlGraphicsChanged();
				}
			}
		}

		protected PointF LastTrackedPosition
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					return this.SpatialTransform.ConvertToSource(_lastTrackedPosition);
				return _lastTrackedPosition;
			}
		}

		protected bool IsTracking
		{
			get { return _isTracking; }
		}

		private void OnSubjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_notifyOnSubjectChanged)
			{
				this.OnSubjectChanged();
				EventsHelper.Fire(_subjectChanged, this, new EventArgs());
			}
		}

		/// <summary>
		/// Suspends notification of <see cref="SubjectChanged"/> events.
		/// </summary>
		/// <remarks>
		/// There are times when it is desirable to suspend the notification of
		/// <see cref="SubjectChanged"/> events, such as when initializing 
		/// the <see cref="IControlGraphic.Subject"/> graphic.  To resume the raising of the event, call
		/// <see cref="Resume"/>.
		/// </remarks>
		public void Suspend()
		{
			_notifyOnSubjectChanged = false;
		}

		/// <summary>
		/// Resumes notification of <see cref="SubjectChanged"/> events.
		/// </summary>
		/// <param name="notifyNow">If <b>true</b>, the graphic is updated immediately.
		/// </param>
		public void Resume(bool notifyNow)
		{
			_notifyOnSubjectChanged = true;

			if (notifyNow)
				OnSubjectPropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
		}

		protected virtual void OnSubjectChanged() {}

		protected virtual void OnColorChanged() {}

		protected virtual void OnShowControlGraphicsChanged() {}

		protected virtual IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return null;
		}

		protected virtual CursorToken OnGetCursorToken(Point point)
		{
			return null;
		}

		protected virtual bool OnMouseStart(IMouseInformation mouseInformation)
		{
			return false;
		}

		protected virtual bool OnMouseTrack(IMouseInformation mouseInformation)
		{
			return false;
		}

		protected virtual bool OnMouseStop(IMouseInformation mouseInformation)
		{
			return false;
		}

		protected virtual void OnMouseCancel() {}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		public CursorToken GetCursorToken(Point point)
		{
			CursorToken cursor = null;

			if (_capturedHandler != null)
			{
				if (_capturedHandler is ICursorTokenProvider)
				{
					cursor = ((ICursorTokenProvider) _capturedHandler).GetCursorToken(point);
				}
			}

			if (cursor == null)
				cursor = OnGetCursorToken(point);

			if (cursor == null)
			{
				foreach (IGraphic graphic in this.EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					ICursorTokenProvider provider = graphic as ICursorTokenProvider;
					if (provider != null)
					{
						cursor = provider.GetCursorToken(point);
						if (cursor != null)
							break;
					}
				}
			}

			return cursor;
		}

		#endregion

		#region IMouseButtonHandler Members

		public bool Start(IMouseInformation mouseInformation)
		{
			bool result;

			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				if (this.HitTest(mouseInformation.Location))
				{
					_lastTrackedPosition = mouseInformation.Location;
					_isTracking = true;
				}
				result = this.OnMouseStart(mouseInformation);
				_isTracking = _isTracking && result;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			_capturedHandler = null;
			if (!result)
			{
				foreach (IGraphic graphic in this.EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Start(mouseInformation);
						if (result)
						{
							_capturedHandler = handler;
							break;
						}
					}
				}
			}
			
			return result;
		}

		public bool Track(IMouseInformation mouseInformation)
		{
			bool result;

			if (_capturedHandler != null)
				return _capturedHandler.Track(mouseInformation);

			try
			{
				result = this.OnMouseTrack(mouseInformation);
			}
			finally
			{
				if (_isTracking)
					_lastTrackedPosition = mouseInformation.Location;
			}

			if (!result)
			{
				foreach (IGraphic graphic in this.EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Track(mouseInformation);
						if (result)
							break;
					}
				}
			}

			return result;
		}

		public bool Stop(IMouseInformation mouseInformation)
		{
			bool result;

			if (_capturedHandler != null)
			{
				result = _capturedHandler.Stop(mouseInformation);
				_capturedHandler = null;
				return result;
			}

			try
			{
				result = this.OnMouseStop(mouseInformation);
			}
			finally
			{
				_isTracking = false;
				_lastTrackedPosition = PointF.Empty;
			}

			return result;
		}

		public void Cancel()
		{
			if (_capturedHandler != null)
			{
				_capturedHandler.Cancel();
				_capturedHandler = null;
			}

			try
			{
				this.OnMouseCancel();
			}
			finally
			{
				_isTracking = false;
				_lastTrackedPosition = PointF.Empty;
			}
		}

		public virtual MouseButtonHandlerBehaviour Behaviour
		{
			get
			{
				if (this.DecoratedGraphic is IControlGraphic)
					return ((IControlGraphic) this.DecoratedGraphic).Behaviour;
				return MouseButtonHandlerBehaviour.None;
			}
		}

		#endregion

		#region IExportedActionsProvider Members

		public IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			bool atLeastOne = false;
			IActionSet actions = new ActionSet();

			IActionSet myActions = this.OnGetExportedActions(site, mouseInformation);
			if (myActions != null)
			{
				actions = actions.Union(myActions);
				atLeastOne = true;
			}

			foreach (IGraphic graphic in this.EnumerateChildGraphics(true))
			{
				IExportedActionsProvider controlGraphic = graphic as IExportedActionsProvider;
				if (controlGraphic != null)
				{
					IActionSet otherActions = controlGraphic.GetExportedActions(site, mouseInformation);
					if (otherActions != null)
					{
						actions = actions.Union(otherActions);
						atLeastOne = true;
					}
				}
			}

			return atLeastOne ? actions : null;
		}

		#endregion
	}
}