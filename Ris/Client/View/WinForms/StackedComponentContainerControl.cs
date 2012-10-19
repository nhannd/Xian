#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class StackedComponentContainerControl : UserControl
	{
		readonly StackedComponentContainer _component;

		public StackedComponentContainerControl(StackedComponentContainer component)
		{
			Platform.CheckForNullReference(component, "component");

			InitializeComponent();
			_component = component;
			_component.CurrentPageChanged += OnComponentCurrentPageChanged;

			ShowPage(_component.CurrentPage);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
			ShowPage(_component.CurrentPage);
		}

		private void ShowPage(ContainerPage page)
		{
			// get the control to show
			var toShow = (Control)_component.GetPageView(page).GuiElement;

			// hide all others
			foreach (var c in this.Controls.Cast<Control>().Where(c => c != toShow))
			{
				c.Visible = false;
			}

			// if the control has not been added to the content panel, add it now
			if (!this.Controls.Contains(toShow))
			{
				toShow.Dock = DockStyle.Fill;
				this.Controls.Add(toShow);
			}

			toShow.Visible = true;

			// HACK: for some reason the error provider symbols don't show up the first time the control is shown
			// therefore we need to force it
			if (toShow is ApplicationComponentUserControl)
			{
				(toShow as ApplicationComponentUserControl).ErrorProvider.UpdateBinding();
			}
		}

	}
}
