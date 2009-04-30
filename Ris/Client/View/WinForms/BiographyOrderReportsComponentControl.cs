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

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	public partial class BiographyOrderReportsComponentControl : ApplicationComponentUserControl
	{
		private BiographyOrderReportsComponent _component;
		private ActionModelNode _toolbarActionModel;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiographyOrderReportsComponentControl(BiographyOrderReportsComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			Control reportPreview = (Control) _component.ReportPreviewComponentHost.ComponentView.GuiElement;
			reportPreview.Dock = DockStyle.Fill;
			_reportPreviewPanel.Controls.Add(reportPreview);

			_reports.DataSource = _component.Reports;
			_reports.DataBindings.Add("Value", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);
			_reports.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatReportListItem(e.ListItem); };

			_toolbarActionModel = _component.ActionModel;
			ToolStripBuilder.BuildToolbar(_toolstrip.Items, _toolbarActionModel.ChildNodes);

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			_reports.DataSource = _component.Reports;
		}
	}
}
