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
				_component.Search(_searchTextBox.Text);
		}

		private void _searchButton_ButtonClick(object sender, EventArgs e)
		{
			_component.Search(_searchTextBox.Text);
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
