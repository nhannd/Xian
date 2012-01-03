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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Base <see cref="UserControl"/> class providing runtime localization update functionality for localized controls.
	/// </summary>
	public class LocalizableUserControl : UserControl
	{
		private readonly MethodInvoker _onCurrentUICultureChangedMethod;
		private readonly MethodInvoker _onCurrentUIThemeChangedMethod;

		/// <summary>
		/// Initializes a new instance of a <see cref="LocalizableUserControl"/>.
		/// </summary>
		public LocalizableUserControl()
		{
			if (!DesignMode)
			{
				Application.CurrentUICultureChanged += Application_CurrentUICultureChanged;
				Application.CurrentUIThemeChanged += Application_CurrentUIThemeChanged;
				_onCurrentUICultureChangedMethod = OnCurrentUICultureChanged;
				_onCurrentUIThemeChangedMethod = OnCurrentUIThemeChanged;
			}
		}

		/// <summary>
		/// Called to release any resources held by the <see cref="LocalizableUserControl"/>.
		/// </summary>
		/// <param name="disposing">True if the <see cref="LocalizableUserControl"/> is being disposed; False if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Application.CurrentUICultureChanged -= Application_CurrentUICultureChanged;
				Application.CurrentUIThemeChanged -= Application_CurrentUIThemeChanged;
			}
			base.Dispose(disposing);
		}

		private void Application_CurrentUICultureChanged(object sender, EventArgs e)
		{
			if (_onCurrentUICultureChangedMethod != null)
			{
				if (InvokeRequired)
					Invoke(_onCurrentUICultureChangedMethod);
				else
					_onCurrentUICultureChangedMethod.Invoke();
			}
		}

		private void Application_CurrentUIThemeChanged(object sender, EventArgs e)
		{
			if (_onCurrentUIThemeChangedMethod != null)
			{
				if (InvokeRequired)
					Invoke(_onCurrentUIThemeChangedMethod);
				else
					_onCurrentUIThemeChangedMethod.Invoke();
			}
		}

		/// <summary>
		/// Called when the current application UI culture has changed.
		/// </summary>
		/// <seealso cref="Application.CurrentUICulture"/>
		protected virtual void OnCurrentUICultureChanged()
		{
			ApplyControlResources(this);
		}

		/// <summary>
		/// Called when the current application UI theme has changed.
		/// </summary>
		/// <seealso cref="Application.CurrentUITheme"/>
		protected virtual void OnCurrentUIThemeChanged() {}

		/// <summary>
		/// Convenience helper method to apply component resources for a top-level <see cref="UserControl"/>
		/// or <see cref="Form"/> and its descendants using the current application UI culture.
		/// </summary>
		/// <param name="control">A top-level <see cref="Control"/> that has an associated resource (RESX) file.</param>
		public static void ApplyControlResources(Control control)
		{
			Platform.CheckForNullReference(control, "control");
			control.SuspendLayout();
			try
			{
				// use the resource manager associated with the control class
				var resourceManager = new ComponentResourceManager(control.GetType());
				var cultureInfo = Application.CurrentUICulture;

				// apply any resources to child controls first
				ApplyChildControlResources(resourceManager, cultureInfo, control);

				// apply the resources associated with ourself using the special key "$this"
				resourceManager.ApplyResources(control, "$this", cultureInfo);
			}
			finally
			{
				control.ResumeLayout(true);
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