using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	internal sealed class MouseImageViewerToolInitializer
	{
		private MouseImageViewerToolInitializer()
		{ 
		}

		public static void Initialize(MouseImageViewerTool mouseTool)
		{
			object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
			if (buttonAssignment == null || buttonAssignment.Length == 0)
			{
				throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));
			}
			else
			{
				MouseToolButtonAttribute attribute = buttonAssignment[0] as MouseToolButtonAttribute;

				if (attribute.MouseButton == XMouseButtons.None)
				{
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));
				}
				else
				{
					mouseTool.MouseButton = attribute.MouseButton;
					mouseTool.Active = attribute.InitiallyActive;
				}
			}
		}
	}
}
