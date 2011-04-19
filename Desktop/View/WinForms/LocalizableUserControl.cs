#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Base <see cref="UserControl"/> class providing runtime localization update functionality for localized controls.
	/// </summary>
	public abstract class LocalizableUserControl : UserControl
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="LocalizableUserControl"/>.
		/// </summary>
		protected LocalizableUserControl()
		{
			if (!DesignMode)
				Application.CurrentUICultureChanged += Application_CurrentUICultureChanged;
		}

		/// <summary>
		/// Called to release any resources held by the <see cref="LocalizableUserControl"/>.
		/// </summary>
		/// <param name="disposing">True if the <see cref="LocalizableUserControl"/> is being disposed; False if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			if (!DesignMode && disposing)
				Application.CurrentUICultureChanged -= Application_CurrentUICultureChanged;
			base.Dispose(disposing);
		}

		private void Application_CurrentUICultureChanged(object sender, EventArgs e)
		{
			OnCurrentUICultureChanged();
		}

		/// <summary>
		/// Called when the current application UI culture has changed.
		/// </summary>
		protected virtual void OnCurrentUICultureChanged()
		{
			SuspendLayout();
			try
			{
				// since we are the top level user control, use the resource manager associated with this class
				var resourceManager = new ComponentResourceManager(GetType());
				var cultureInfo = Application.CurrentUICulture;

				// apply any resources to child controls first
				ApplyChildControlResources(resourceManager, cultureInfo, this);

				// apply the resources associated with ourself using the special key "$this"
				resourceManager.ApplyResources(this, "$this", cultureInfo);
			}
			finally
			{
				ResumeLayout(true);
			}
		}

		private static void ApplyChildControlResources(ComponentResourceManager resourceManager, CultureInfo cultureInfo, Control parent)
		{
			foreach (Control child in parent.Controls)
			{
				child.SuspendLayout();
				try
				{
					// apply any resources to further nested child controls first
					ApplyChildControlResources(resourceManager, cultureInfo, child);

					// apply the resources associated with the child control
					resourceManager.ApplyResources(child, child.Name, cultureInfo);
				}
				finally
				{
					child.ResumeLayout(false);
				}
			}
		}
	}
}