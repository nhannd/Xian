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
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	internal sealed class MouseToolAttributeProcessor
	{
		private MouseToolAttributeProcessor()
		{ 
		}

		public static void Process(MouseImageViewerTool mouseTool)
		{
			Platform.CheckForNullReference(mouseTool, "mouseTool");

			InitializeMouseToolButton(mouseTool);
			InitializeModifiedMouseToolButton(mouseTool);
			InitializeMouseWheel(mouseTool);
		}

		private static void InitializeMouseToolButton(MouseImageViewerTool mouseTool)
		{
			object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
			if (buttonAssignment != null && buttonAssignment.Length > 0)
			{
				MouseToolButtonAttribute attribute = (MouseToolButtonAttribute)buttonAssignment[0];
				if (attribute.MouseButton == XMouseButtons.None)
				{
					Platform.Log(LogLevel.Warn, String.Format(SR.FormatMouseToolInvalidAssignment, mouseTool.GetType().FullName));
				}
				else
				{
					mouseTool.MouseButton = attribute.MouseButton;
					mouseTool.Active = attribute.InitiallyActive;
				}
			}
		}

		private static void InitializeModifiedMouseToolButton(MouseImageViewerTool mouseTool)
		{
			object[] defaultButtonAssignments = mouseTool.GetType().GetCustomAttributes(typeof(DefaultMouseToolButtonAttribute), true);
			if (defaultButtonAssignments == null || defaultButtonAssignments.Length == 0)
				return;

			DefaultMouseToolButtonAttribute attribute = (DefaultMouseToolButtonAttribute)defaultButtonAssignments[0];

			try
			{
				mouseTool.DefaultMouseButtonShortcut = attribute.Shortcut;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		private static void InitializeMouseWheel(MouseImageViewerTool mouseTool)
		{
			object[] attributes = mouseTool.GetType().GetCustomAttributes(typeof(MouseWheelHandlerAttribute), false);
			if (attributes == null || attributes.Length == 0)
				return;

			MouseWheelHandlerAttribute attribute = (MouseWheelHandlerAttribute)attributes[0];
			mouseTool.MouseWheelShortcut = attribute.Shortcut;
		}
	}
}
