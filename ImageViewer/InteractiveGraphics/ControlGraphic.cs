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
	/// <summary>
	/// Defines a graphic that decorates an <see cref="IGraphic"/> with user interaction
	/// components controlling an underlying graphic in the scene graph.
	/// </summary>
	public interface IControlGraphic : IDecoratorGraphic, ICursorTokenProvider, IMouseButtonHandler, IExportedActionsProvider
	{
		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		/// <remarks>
		/// The controlled graphic is the first non-decorator graphic in the
		/// control graphics chain (the first graphic that doesn't implement
		/// <see cref="IDecoratorGraphic"/> when recursively following the
		/// <see cref="IDecoratorGraphic.DecoratedGraphic"/> property.)
		/// </remarks>
		IGraphic Subject { get; }

		/// <summary>
		/// Gets or sets the color of the control graphic.
		/// </summary>
		Color Color { get; set; }

		/// <summary>
		/// Gets or sets a value to show or hide this control graphic without affecting the
		/// visibility of the underlying subject or other control graphics.
		/// </summary>
		bool Show { get; set; }
	}

	/// <summary>
	/// Abstract base class for implementations of <see cref="IControlGraphic"/>.
	/// </summary>
	[Cloneable]
	public abstract class ControlGraphic : DecoratorCompositeGraphic, IControlGraphic
	{
		private event EventHandler _subjectChanged;
		private Color _color = Color.Yellow;
		private bool _show = true;

		[CloneIgnore]
		private bool _notifyOnSubjectChanged = true;

		[CloneIgnore]
		private IMouseButtonHandler _capturedHandler = null;

		[CloneIgnore]
		private PointF _lastTrackedPosition = PointF.Empty;

		[CloneIgnore]
		private bool _isTracking = false;

		/// <summary>
		/// Constructs a new control graphic to control the given subject graphic.
		/// </summary>
		/// <param name="subject">The graphic to control.</param>
		protected ControlGraphic(IGraphic subject) : base(subject)
		{
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
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

		/// <summary>
		/// Releases all resources used by this <see cref="ControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.PropertyChanged -= OnSubjectPropertyChanged;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		/// <remarks>
		/// The controlled graphic is the first non-decorator graphic in the
		/// control graphics chain (the first graphic that doesn't implement
		/// <see cref="IDecoratorGraphic"/> when recursively following the
		/// <see cref="IDecoratorGraphic.DecoratedGraphic"/> property.)
		/// </remarks>
		public IGraphic Subject
		{
			get
			{
				if (this.DecoratedGraphic is IControlGraphic)
					return ((IControlGraphic) this.DecoratedGraphic).Subject;
				return this.DecoratedGraphic;
			}
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public virtual string CommandName
		{
			get { return null; }
		}

		/// <summary>
		/// Gets or sets the color of the control graphic.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value to show or hide this control graphic without affecting the
		/// visibility of the underlying subject or other control graphics.
		/// </summary>
		public bool Show
		{
			get { return _show; }
			set
			{
				if (_show != value)
				{
					_show = value;
					this.OnShowControlGraphicsChanged();
				}
			}
		}

		/// <summary>
		/// Gets the last tracked cursor position in source or destination coordinates.
		/// </summary>
		protected PointF LastTrackedPosition
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
					return this.SpatialTransform.ConvertToSource(_lastTrackedPosition);
				return _lastTrackedPosition;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the control graphic is currently tracking mouse input.
		/// </summary>
		protected bool IsTracking
		{
			get { return _isTracking; }
		}

		private void OnSubjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_notifyOnSubjectChanged)
			{
				this.OnSubjectChanged();
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

		/// <summary>
		/// Called when properties on the <see cref="Subject"/> have changed.
		/// </summary>
		protected virtual void OnSubjectChanged() {}

		/// <summary>
		/// Called when the <see cref="Color"/> property changes.
		/// </summary>
		protected virtual void OnColorChanged() {}

		/// <summary>
		/// Called when the <see cref="Show"/> property changes.
		/// </summary>
		protected virtual void OnShowControlGraphicsChanged() {}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting exported actions via <see cref="GetExportedActions"/>.
		/// </summary>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		protected virtual IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return null;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		/// <returns></returns>
		protected virtual CursorToken OnGetCursorToken(Point point)
		{
			return null;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to a mouse button click via <see cref="Start"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		protected virtual bool OnMouseStart(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework tracking mouse input via <see cref="Track"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		protected virtual bool OnMouseTrack(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response a mouse button release via <see cref="Stop"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		protected virtual bool OnMouseStop(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to an attempt to cancel the current operation via <see cref="Cancel"/>.
		/// </summary>
		protected virtual void OnMouseCancel() { }

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <remarks>
		/// The <see cref="ControlGraphic"/> implementation returns the the cursor token
		/// provided by the <see cref="CurrentHandler">current input handler</see>,
		/// <see cref="OnGetCursorToken"/>, or any child graphics implementing <see cref="ICursorTokenProvider"/>,
		/// in decreasing order of priority.
		/// </remarks>
		CursorToken ICursorTokenProvider.GetCursorToken(Point point)
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
				cursor = this.OnGetCursorToken(point);

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

		/// <summary>
		/// Called by the framework each time a mouse button is pressed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// As a general rule, if the <see cref="IMouseButtonHandler"/> object did anything as a result of this call, it must 
		/// return true.  If false is returned, <see cref="IMouseButtonHandler.Start"/> is called on other <see cref="IMouseButtonHandler"/>s
		/// until one returns true.
		/// </para>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation finds a handler by trying <see cref="OnMouseStart"/>,
		/// and any child graphics implementing <see cref="IMouseButtonHandler"/>, in decreasing order of priority.
		/// Successful capture results in the <see cref="CurrentHandler"/> property being set to the captured handler.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		bool IMouseButtonHandler.Start(IMouseInformation mouseInformation)
		{
			//TODO (CR May09):route to captured handler until it returns false.
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

		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </para>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Track"/> on
		/// the current handler, <see cref="OnMouseTrack"/>, or any child graphics implementing <see cref="IMouseButtonHandler"/>,
		/// in decreasing order of priority.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		bool IMouseButtonHandler.Track(IMouseInformation mouseInformation)
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

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Stop"/> on
		/// the current handler, <see cref="OnMouseStop"/>, or any child graphics implementing <see cref="IMouseButtonHandler"/>,
		/// in decreasing order of priority.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		bool IMouseButtonHandler.Stop(IMouseInformation mouseInformation)
		{
			bool result;

			//TODO (CR May09):route to captured handler until it returns false.

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

		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Cancel"/> on
		/// the current handler or <see cref="OnMouseCancel"/> in decreasing order of priority.
		/// </para>
		/// </remarks>
		void IMouseButtonHandler.Cancel()
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

		/// <summary>
		/// Gets the desired behaviour of this mouse input handler.
		/// </summary>
		/// <remarks>
		/// The default implementation returns the behaviour of the lowest control
		/// graphic in the control chain, or <see cref="MouseButtonHandlerBehaviour.None"/>
		/// if this graphic is the lowest in the control chain.
		/// </remarks>
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

		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This mechanism is useful when a particular component defines generally useful <see cref="IAction"/>s
		/// without requiring specific knowledge of the action model sites that the client code uses.
		/// </para>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation returns the combination of its own exported actions
		/// (as provided by <see cref="OnGetExportedActions"/>) and those of its child graphics.
		/// </para>
		/// </remarks>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		IActionSet IExportedActionsProvider.GetExportedActions(string site, IMouseInformation mouseInformation)
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