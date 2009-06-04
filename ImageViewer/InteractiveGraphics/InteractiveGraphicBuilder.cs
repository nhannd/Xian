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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Base class for builder objects to which mouse input can be forwarded in order to build graphics in a standardized manner.
	/// </summary>
	/// <remarks>
	/// In order to standardize the way in which common-shaped graphics are created based on user input, it is recommended that
	/// tools collecting a successive series of user mouse inputs to create a graphic (such as <see cref="MouseImageViewerTool"/>s)
	/// instantiate an <see cref="InteractiveGraphicBuilder"/> and forward to it the mouse information.
	/// </remarks>
	public abstract class InteractiveGraphicBuilder : IMouseButtonHandler, ICursorTokenProvider
	{
		private event EventHandler<GraphicEventArgs> _graphicComplete;
		private event EventHandler<GraphicEventArgs> _graphicCancelled;
		private static readonly CursorToken _crossCursorToken = new CursorToken(CursorToken.SystemCursors.Cross);
		private readonly IGraphic _graphic;

		/// <summary>
		/// Constructs a new <see cref="InteractiveGraphicBuilder"/> to operate on the specified graphic.
		/// </summary>
		/// <param name="graphic">The graphic on which the builder will operate.</param>
		protected InteractiveGraphicBuilder(IGraphic graphic)
		{
			_graphic = graphic;
		}

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public IGraphic Graphic
		{
			get { return _graphic; }
		}

		/// <summary>
		/// Occurs when the builder is done building the <see cref="Graphic"/>.
		/// </summary>
		public event EventHandler<GraphicEventArgs> GraphicComplete
		{
			add { _graphicComplete += value; }
			remove { _graphicComplete -= value; }
		}

		/// <summary>
		/// Occurs when the builder is cancelling building the <see cref="Graphic"/> due to user cancellation.
		/// </summary>
		public event EventHandler<GraphicEventArgs> GraphicCancelled
		{
			add { _graphicCancelled += value; }
			remove { _graphicCancelled -= value; }
		}

		//TODO (CR May09):notify vs On, Notify should not be virtual.
		/// <summary>
		/// Notifies listeners that the builder is done building the <see cref="Graphic"/>.
		/// </summary>
		protected virtual void NotifyGraphicComplete()
		{
			this.OnGraphicComplete();
			EventsHelper.Fire(_graphicComplete, this, new GraphicEventArgs(_graphic));
		}

		/// <summary>
		/// Notifies listeners that the builder is cancelling building the <see cref="Graphic"/> due to user cancellation.
		/// </summary>
		protected virtual void NotifyGraphicCancelled()
		{
			this.OnGraphicCancelled();
			EventsHelper.Fire(_graphicCancelled, this, new GraphicEventArgs(_graphic));
		}

		/// <summary>
		/// Called when the builder is done building the <see cref="Graphic"/>.
		/// </summary>
		protected virtual void OnGraphicComplete() {}

		/// <summary>
		/// Called when the builder is cancelling building the <see cref="Graphic"/> due to user cancellation.
		/// </summary>
		protected virtual void OnGraphicCancelled() {}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public virtual bool Start(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public virtual bool Track(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Cancels the building of the graphic.
		/// </summary>
		public virtual void Cancel()
		{
			this.NotifyGraphicCancelled();
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public virtual void Reset() {}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected virtual void Rollback() {}

		/// <summary>
		/// Gets the builder-recommended behaviour that the owning tool should use.
		/// </summary>
		public virtual MouseButtonHandlerBehaviour Behaviour
		{
			get { return MouseButtonHandlerBehaviour.None; }
		}

		/// <summary>
		/// Gets the builder-recommended cursor that the owning tool should use.
		/// </summary>
		/// <param name="point">The cursor location, in destination coordinates.</param>
		/// <returns>The recommended cursor.</returns>
		public virtual CursorToken GetCursorToken(Point point)
		{
			return _crossCursorToken;
		}
	}
}