using System;
using System.ComponentModel;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderExplorerGroupComponent"/>
    /// </summary>
	public partial class FolderExplorerGroupComponentControl : ApplicationComponentUserControl
    {
        private readonly FolderExplorerGroupComponent _component;

		private readonly ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
		private readonly ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Left;
		private readonly TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

		private bool _isLoaded = false;

		private ActionModelNode _toolbarModel;
		private ActionModelNode _menuModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerGroupComponentControl(FolderExplorerGroupComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_menuModel = _component.ContextMenuModel;
			_toolbarModel = _component.ToolbarModel;

			Control stackTabGroups = (Control)_component.StackTabComponentContainerHost.ComponentView.GuiElement;
			stackTabGroups.Dock = DockStyle.Fill;
			_groupPanel.Controls.Add(stackTabGroups);
		}

		#region Properties

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set
			{
				_toolbarModel = value;

				// Defer initialization of ToolStrip until after Load() has been called
				// so that parameters from application settings are initialized properly
				if (_isLoaded)
					InitializeToolStrip();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			set
			{
				_menuModel = value;
				ToolStripBuilder.Clear(_contextMenu.Items);
				if (_menuModel != null)
				{
					ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
				}
			}
		}

		#endregion

		#region Event Handlers

		private void FolderExplorerGroupComponentControl_Load(object sender, EventArgs e)
		{
			InitializeToolStrip();
			_isLoaded = true;
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			_component.Search(new SearchData(_searchTextBox.Text, false));
		}

		private void _searchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				_component.Search(new SearchData(_searchTextBox.Text, false));
		}

		private void _searchTextBox_TextChanged(object sender, EventArgs e)
		{
			_searchButton.Enabled = !string.IsNullOrEmpty(_searchTextBox.Text);
		}

		#endregion

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);
			if (_toolbarModel != null)
			{
				if (_toolStripItemAlignment == ToolStripItemAlignment.Right)
				{
					_toolbarModel.ChildNodes.Reverse();
				}

				ToolStripBuilder.BuildToolbar(
					_toolStrip.Items,
					_toolbarModel.ChildNodes,
					new ToolStripBuilder.ToolStripBuilderStyle(
						_toolStripItemDisplayStyle, 
						_toolStripItemAlignment, 
						_textImageRelation));
			}
		}
	}
}
