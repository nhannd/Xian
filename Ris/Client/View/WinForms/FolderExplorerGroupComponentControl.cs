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
using System.ComponentModel;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="FolderExplorerGroupComponent"/>
	/// </summary>
	public partial class FolderExplorerGroupComponentControl : ApplicationComponentUserControl
	{
		private readonly FolderExplorerGroupComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public FolderExplorerGroupComponentControl(FolderExplorerGroupComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_component.SelectedFolderExplorerChanged += delegate
				{
					InitializeToolStrip();
				};

			Control stackTabGroups = (Control)_component.StackTabComponentContainerHost.ComponentView.GuiElement;
			stackTabGroups.Dock = DockStyle.Fill;
			_groupPanel.Controls.Add(stackTabGroups);

			this.DataBindings.Add("SearchTextBoxEnabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.DataBindings.Add("SearchTextBoxMessage", _component, "SearchMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.DataBindings.Add("SearchButtonEnabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			this.DataBindings.Add("AdvancedSearchButtonEnabled", _component, "AdvancedSearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		#region Properties

		// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SearchTextBoxEnabled
		{
			get { return _searchTextBox.Enabled; }
			set { _searchTextBox.Enabled = value; }
		}

		// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SearchTextBoxMessage
		{
			get { return _searchTextBox.ToolTipText; }
			set { _searchTextBox.ToolTipText = value; }
		}

		// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SearchButtonEnabled
		{
			get { return _searchButton.Enabled; }
			set { _searchButton.Enabled = value; }
		}

		// used by databinding within this control only
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AdvancedSearchButtonEnabled
		{
			get { return _advancedSearch.Enabled; }
			set { _advancedSearch.Enabled = value; }
		}

		#endregion

		#region Event Handlers

		private void FolderExplorerGroupComponentControl_Load(object sender, EventArgs e)
		{
			InitializeToolStrip();
		}

		private void _searchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				_component.Search(new SearchParams(_searchTextBox.Text));
		}

		private void _searchButton_ButtonClick(object sender, EventArgs e)
		{
			_component.Search(new SearchParams(_searchTextBox.Text));
		}

		private void _advancedSearch_Click(object sender, EventArgs e)
		{
			_component.AdvancedSearch();
		}

		#endregion

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);
			if (_component.ToolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(_toolStrip.Items, _component.ToolbarModel.ChildNodes);
			}
		}
	}
}
