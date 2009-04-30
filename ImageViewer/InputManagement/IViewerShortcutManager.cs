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

using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Used internally by the framework to lookup different interfaces registered by, say, an <see cref="ImageViewerComponent"/> 
	/// that correspond to different mouse/keyboard shortcuts.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The shortcuts are usually associated with an <see cref="ClearCanvas.Desktop.Tools.ITool"/> having one or more of 
	/// the <see cref="MenuActionAttribute"/> or <see cref="KeyboardActionAttribute"/> attributes defined, or implementing
	/// either <see cref="IMouseButtonHandler"/> or <see cref="IMouseWheelHandler"/>.
	/// </para>
	/// <para>
	/// This interface is intended for internal framework use only.
	/// </para>
	/// </remarks>
	/// <seealso cref="IClickAction"/>
	/// <seealso cref="IMouseButtonHandler"/>
	/// <seealso cref="IMouseWheelHandler"/>
	/// <seealso cref="KeyboardButtonShortcut"/>
	/// <seealso cref="MouseButtonShortcut"/>
	/// <seealso cref="MouseWheelShortcut"/>
	public interface IViewerShortcutManager
	{
		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with the input <paramref name="shortcut"/>.
		/// </summary>
		/// <remarks>
		/// Will return null if there is no <see cref="IClickAction"/> associated with the <paramref name="shortcut"/>.
		/// </remarks>
		IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut);

		/// <summary>
		/// Gets the <see cref="IMouseButtonHandler"/>s assigned to the given shortcut.
		/// </summary>
		/// <remarks>
		/// In the case of <see cref="MouseImageViewerTool"/>s, the tool assigned to the specified shortcut
		/// is returned first, followed by any whose default shortcut matches the one specified.
		/// </remarks>
		IEnumerable<IMouseButtonHandler> GetMouseButtonHandlers(MouseButtonShortcut shortcut);

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with the input <paramref name="shortcut"/>.
		/// </summary>
		/// <remarks>
		/// Will return null if there is no <see cref="IMouseWheelHandler"/> associated with the <paramref name="shortcut"/>.
		/// </remarks>
		IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut);
	}
}
