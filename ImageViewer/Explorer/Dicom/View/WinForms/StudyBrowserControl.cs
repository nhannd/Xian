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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class StudyBrowserControl : UserControl
	{
		private StudyBrowserComponent _studyBrowserComponent;
		private BindingSource _bindingSource;

		public StudyBrowserControl(StudyBrowserComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			InitializeComponent();

			ClearCanvasStyle.SetTitleBarStyle(_resultsTitleBar);

			_studyBrowserComponent = component;
			_studyBrowserComponent.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);

			_studyTableView.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_studyTableView.Table = _studyBrowserComponent.StudyList;
			_studyTableView.ToolbarModel = _studyBrowserComponent.ToolbarModel;
			_studyTableView.MenuModel = _studyBrowserComponent.ContextMenuModel;
			_studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
			_studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _studyBrowserComponent;

			_resultsTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "ResultsTitle", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnSelectedServerChanged(object sender, EventArgs e)
		{
            _studyTableView.Table = _studyBrowserComponent.StudyList;
		}

		void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
		{
			_studyBrowserComponent.SetSelection(_studyTableView.Selection);
		}

		void OnStudyTableViewDoubleClick(object sender, EventArgs e)
		{
			_studyBrowserComponent.ItemDoubleClick();
		}
	}
}
