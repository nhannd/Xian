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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	internal sealed class ViewerShortcutManager : IViewerShortcutManager
	{
		private readonly Dictionary<ITool, ITool> _setRegisteredTools;

		public ViewerShortcutManager()
		{
			_setRegisteredTools = new Dictionary<ITool, ITool>();
		}

		private void RegisterMouseToolButton(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, false);

			mouseTool.MouseButtonChanged += new EventHandler(OnMouseToolMouseButtonChanged);
			mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);
		}

		private MouseImageViewerTool GetActiveMouseTool(XMouseButtons button)
		{
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;
				if (mouseTool != null)
				{
					if (mouseTool.Active && mouseTool.MouseButton == button)
						return mouseTool;
				}
			}

			return null;
		}

		private void DeactivateMouseTools(MouseImageViewerTool activating)
		{
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;
				if (mouseTool != null)
				{
					if (mouseTool.Active && mouseTool != activating && mouseTool.MouseButton == activating.MouseButton)
						mouseTool.Active = false;
				}
			}
		}

		private void ActivateMouseTool(MouseImageViewerTool activateMouseTool, bool replaceExisting)
		{
			if (activateMouseTool.MouseButton == XMouseButtons.None)
			{
				Platform.Log(LogLevel.Error, String.Format(SR.FormatMouseToolHasNoAssignment, activateMouseTool.GetType().FullName));
				activateMouseTool.Active = false;
				return;
			}

			MouseImageViewerTool current = GetActiveMouseTool(activateMouseTool.MouseButton);
			if (!replaceExisting && current != null && current != activateMouseTool)
			{
				activateMouseTool.Active = false;
				return;
			}

			DeactivateMouseTools(activateMouseTool);
		}

		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		private void OnMouseToolMouseButtonChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		/// <summary>
		/// Registers the tool with the viewer shortcut manager.
		/// </summary>
		/// <param name="tool">the tool to register.</param>
		public void RegisterImageViewerTool(ITool tool)
		{
			Platform.CheckForNullReference(tool, "tool");

			if (tool is MouseImageViewerTool)
				RegisterMouseToolButton((MouseImageViewerTool)tool);

			_setRegisteredTools[tool] = tool;
		}

		#region IViewerShortcutManager Members

		/// <summary>
		/// Gets the <see cref="IMouseButtonHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseButtonHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseButtonHandler"/> or null.</returns>
		public IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut)
		{
			if (shortcut == null)
				return null;

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseButtonHandler = tool as MouseImageViewerTool;
				if (mouseButtonHandler != null)
				{
					//Active mouse button assignments take precedence over inactive ones.
					if (mouseButtonHandler.Active && shortcut.Equals(mouseButtonHandler.MouseButton))
						return mouseButtonHandler;
				}
			}

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseButtonHandler = tool as MouseImageViewerTool;
				if (mouseButtonHandler != null)
				{
					if (shortcut.Equals(mouseButtonHandler.DefaultMouseButtonShortcut))
						return mouseButtonHandler;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseWheelHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseWheelHandler"/> or null.</returns>
		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			if (shortcut == null)
				return null;

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool viewerTool = tool as MouseImageViewerTool;
				if (viewerTool != null)
				{
					if (shortcut.Equals(viewerTool.MouseWheelShortcut))
						return viewerTool;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IClickAction"/> is to be found.</param>
		/// <returns>An <see cref="IClickAction"/> or null.</returns>
		public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
		{
			if (shortcut == null)
				return null;
			
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				foreach (IAction action in tool.Actions)
				{
					IClickAction clickAction = action as IClickAction;
					if (clickAction != null)
					{
						if (shortcut.Equals(clickAction.KeyStroke))
							return clickAction;
					}
				}
			}

			return null;
		}

		#endregion
	}
}
