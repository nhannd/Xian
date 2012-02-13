#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	internal static class ActionViewUtils
	{
		/// <summary>
		/// Sets the tooltip text on the specified item, from the specified action.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		internal static void SetTooltipText(ToolStripItem item, IAction action)
		{
			var actionTooltip = action.Tooltip;
			if (string.IsNullOrEmpty(actionTooltip))
				actionTooltip = (action.Label ?? string.Empty).Replace("&", "");

			var clickAction = action as IClickAction;

			if (clickAction == null || clickAction.KeyStroke == XKeys.None)
			{
				item.ToolTipText = actionTooltip;
				return;
			}

			var keyCode = clickAction.KeyStroke & XKeys.KeyCode;

			var builder = new StringBuilder();
			builder.Append(actionTooltip);

			if (keyCode != XKeys.None)
			{
				if (builder.Length > 0)
					builder.AppendLine();
				builder.AppendFormat("{0}: ", SR.LabelKeyboardShortcut);
				builder.Append(XKeysConverter.Format(clickAction.KeyStroke));
			}

			item.ToolTipText = builder.ToString();
		}

		/// <summary>
		/// Sets the icon on the specified item, from the specified action.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		/// <param name="iconSize"></param>
		internal static void SetIcon(ToolStripItem item, IAction action, IconSize iconSize)
		{
			if (action.IconSet != null && action.ResourceResolver != null)
			{
				try
				{
					var oldImage = item.Image;

					item.Image = action.IconSet.CreateIcon(iconSize, action.ResourceResolver);
					if (oldImage != null)
						oldImage.Dispose();

					item.Invalidate();
				}
				catch (Exception e)
				{
					// the icon was either null or not found - log some helpful message
					Platform.Log(LogLevel.Error, e);
				}
			}
		}
	}
}
