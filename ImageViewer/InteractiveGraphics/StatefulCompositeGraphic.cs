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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	// TODO (Stewart): Get rid of InstallDefaultCursors; each graphic should install its own

	/// <summary>
	/// A <see cref="CompositeGraphic"/> with state.
	/// </summary>
	public abstract class StatefulCompositeGraphic 
		: CompositeGraphic, IStatefulGraphic, IMouseButtonHandler, ICursorTokenProvider
	{
		private GraphicStateManager _graphicStateManager = new GraphicStateManager();

		/// <summary>
		/// Initializes a new instance of <see cref="StatefulCompositeGraphic"/>.
		/// </summary>
		protected StatefulCompositeGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState State
		{
			get { return _graphicStateManager.State; }
			set { _graphicStateManager.State = value; }
		}

		/// <summary>
		/// Occurs when the <see cref="GraphicState"/> has changed.
		/// </summary>
		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { _graphicStateManager.StateChanged += value; }
			remove { _graphicStateManager.StateChanged -= value; }
		}

		#region IMouseButtonHandler Members

		/// <summary>
		/// Called by the framework each time a mouse button is pressed.
		/// </summary>
		/// <remarks>
		/// As a general rule, if the <see cref="IMouseButtonHandler"/> object did anything as a result of this call, it must 
		/// return true.  If false is returned, <see cref="IMouseButtonHandler.Start"/> is called on other <see cref="IMouseButtonHandler"/>s
		/// until one returns true.
		/// </remarks>
		/// <returns>
		/// True if the <see cref="IMouseButtonHandler"/> did something as a result of the call, 
		/// and hence would like to receive capture.  Otherwise, false.
		/// </returns>
		public virtual bool Start(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Start(mouseInformation);
		}

		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </remarks>
		/// <returns>True if the message was handled, otherwise false.</returns>
		public virtual bool Track(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Track(mouseInformation);
		}

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <returns>
		/// True if the framework should <b>not</b> release capture, otherwise false.
		/// </returns>
		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Stop(mouseInformation);
		}

		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// It is important that this method is implemented correctly and doesn't simply do nothing when it is inappropriate
		/// to do so, otherwise odd behaviour may be experienced.
		/// </remarks>
		public virtual void Cancel()
		{
			this.State.Cancel();
		}

		/// <summary>
		/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
		/// </summary>
		public MouseButtonHandlerBehaviour Behaviour
		{
			get { return this.State.Behaviour; }
		}

		#endregion

		#region ICursorTokenProvider Members

		public virtual CursorToken GetCursorToken(Point point)
		{
			return null;
		}

		#endregion

		public virtual void InstallDefaultCursors()
		{
		}	
	}
}
