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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Defines constants for modifying the behaviour of an <see cref="IMouseButtonHandler"/>.
	/// </summary>
	[Flags]
	public enum MouseButtonHandlerBehaviour
	{
		/// <summary>
		/// Indicates to the framework that no special behaviour, not event the defaults, should be applied.
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Indicates to the framework that the default behaviour should be applied.
		/// </summary>
		Default = CancelStartOnDoubleClick,
		
		/// <summary>
		/// Indicates to the framework that the context menu should be suppressed, usually because this 
		/// object is going to return true from <see cref="IMouseButtonHandler.Stop"/> and keep capture 
		/// after the mouse button has been released.
		/// </summary>
		SuppressContextMenu = 0x1,

		/// <summary>
		/// Indicates to the framework that <see cref="IMouseButtonHandler.Track"/> calls should not be 
		/// made for points outside the <see cref="ITile"/>'s client rectangle.
		/// </summary>
		ConstrainToTile = 0x2,
		
		/// <summary>
		/// Indicates to the framework that the <see cref="IMouseButtonHandler"/> should be ignored
		/// when a tile is first activated.  If this flag is not specified, then the <see cref="IMouseButtonHandler"/>
		/// will <b>not</b> be ignored on tile activation.
		/// </summary>
		SuppressOnTileActivate = 0x4,

		/// <summary>
		/// Because a click must happen before a double-click, this allows an <see cref="IMouseButtonHandler"/> to opt-out
		/// after gaining initial capture (via <see cref="IMouseButtonHandler.Start"/>) and have the framework 
		/// call <see cref="IMouseButtonHandler.Cancel"/> in order to allow another handler to process the double-click.
		/// </summary>
		CancelStartOnDoubleClick = 0x8,

		/// <summary>
		/// Indicates to the framework that the tool does not want to receive double-click notifications.
		/// </summary>
		IgnoreDoubleClicks = 0x10
	}

	/// <summary>
	/// An interface for objects that handle mouse button input.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The framework will look for this interface first on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>), then on 
	/// <see cref="ClearCanvas.Desktop.Tools.ITool"/>s belonging to the current <see cref="IImageViewer"/> 
	/// (via <see cref="IViewerShortcutManager.GetMouseButtonHandler"/>) and if an appropriate one is found, it will be given capture.
	/// </para>
	/// <para>
	/// An <see cref="IMouseButtonHandler"/> gets capture by returning true from <see cref="Start"/> indicating to the framework
	/// that it would like to handle mouse button input.  Similarly, capture is not released until <see cref="Stop"/> returns
	/// false, or <see cref="Cancel"/> is called by the framework.
	/// </para>
	/// <para>
	/// When an <see cref="IMouseButtonHandler"/> has capture, all mouse input defined by the <see cref="IMouseButtonHandler"/> interface
	/// is handled by the object with capture.  No other objects receive any input until capture is released.
	/// </para>
	/// </remarks>
	/// <seealso cref="IImageViewer"/>
	/// <seealso cref="ImageViewerComponent"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="TileController"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.Graphic"/>
	/// <seealso cref="IMouseInformation"/>
	public interface IMouseButtonHandler
	{
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
		bool Start(IMouseInformation mouseInformation);
		
		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </remarks>
		/// <returns>True if the message was handled, otherwise false.</returns>
		bool Track(IMouseInformation mouseInformation);

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <returns>
		/// True if the framework should <b>not</b> release capture, otherwise false.
		/// </returns>
		bool Stop(IMouseInformation mouseInformation);
		
		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// It is important that this method is implemented correctly and doesn't simply do nothing when it is inappropriate
		/// to do so, otherwise odd behaviour may be experienced.
		/// </remarks>
		void Cancel();

		/// <summary>
		/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
		/// </summary>
		MouseButtonHandlerBehaviour Behaviour { get; }
	}
}
