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

using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PriorReportComponent"/>
	/// </summary>
	public partial class PriorReportComponentControl : ApplicationComponentUserControl
	{
		private PriorReportComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PriorReportComponentControl(PriorReportComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			Control reportViewer = (Control)_component.ReportViewComponentHost.ComponentView.GuiElement;
			reportViewer.Dock = DockStyle.Fill;
			splitContainer1.Panel2.Controls.Add(reportViewer);

			_reportList.Table = _component.Reports;
			_reportList.DataBindings.Add("Selection", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioRelevantPriors.DataBindings.Add("Checked", _component, "RelevantPriorsOnly", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioAllPriors.DataBindings.Add("Checked", _component, "AllPriors", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
