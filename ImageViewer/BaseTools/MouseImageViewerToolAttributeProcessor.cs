using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	internal sealed class MouseImageViewerToolAttributeProcessor
	{
		private MouseImageViewerToolAttributeProcessor()
		{ 
		}

		public static void Process(MouseImageViewerTool mouseTool)
		{
			Platform.CheckForNullReference(mouseTool, "mouseTool");

			InitializeMouseToolButton(mouseTool);
			InitializeModifiedMouseToolButton(mouseTool);
		}

		private static void InitializeMouseToolButton(MouseImageViewerTool mouseTool)
		{
			object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
			if (buttonAssignment == null || buttonAssignment.Length == 0)
			{
				throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolMustHaveDefault, mouseTool.GetType().FullName));
			}
			else
			{
				MouseToolButtonAttribute attribute = buttonAssignment[0] as MouseToolButtonAttribute;

				if (attribute.MouseButton == XMouseButtons.None)
				{
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolMustHaveDefault, mouseTool.GetType().FullName));
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
			object[] modifiedButtonAssignments = mouseTool.GetType().GetCustomAttributes(typeof(ModifiedMouseToolButtonAttribute), true);
			if (modifiedButtonAssignments == null || modifiedButtonAssignments.Length == 0)
				return;

			ModifiedMouseToolButtonAttribute attribute = modifiedButtonAssignments[0] as ModifiedMouseToolButtonAttribute;

			try
			{
				mouseTool.ModifiedMouseButtonShortcut = attribute.Shortcut;
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}
	}
}
