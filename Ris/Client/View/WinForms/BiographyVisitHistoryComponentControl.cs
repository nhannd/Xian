#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System.Collections.Generic;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyVisitHistoryComponentControl"/>
	/// </summary>
	public partial class BiographyVisitHistoryComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyVisitHistoryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyVisitHistoryComponentControl(BiographyVisitHistoryComponent component)
		{
			InitializeComponent();
			_component = component;

			_visitList.Table = _component.Visits;
			_visitList.DataBindings.Add("Selection", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);

			Control detailView = (Control)_component.VisitDetailComponentHost.ComponentView.GuiElement;
			detailView.Dock = DockStyle.Fill;
			_detailPanel.Controls.Add(detailView);
		}
	}
}
