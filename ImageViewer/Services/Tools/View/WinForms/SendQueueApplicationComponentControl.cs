using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SendQueueApplicationComponent"/>
    /// </summary>
    public partial class SendQueueApplicationComponentControl : UserControl
    {
        private SendQueueApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public SendQueueApplicationComponentControl(SendQueueApplicationComponent component)
        {
            InitializeComponent();

			_component = component;

			_sendTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_sendTable.Table = _component.SendTable;

			//_sendReceiveProgressTable.ToolbarModel = _component.ToolbarModel;
			//_sendReceiveProgressTable.MenuModel = _component.ContextMenuModel;

			_sendTable.SelectionChanged += new EventHandler(OnSelectionChanged);

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_titleBar.DataBindings.Add("Text", _component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_sendTable.Selection);
		}
	}
}
