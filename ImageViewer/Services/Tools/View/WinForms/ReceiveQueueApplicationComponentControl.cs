using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    public partial class ReceiveQueueApplicationComponentControl : UserControl
    {
        private ReceiveQueueApplicationComponent _component;

        public ReceiveQueueApplicationComponentControl(ReceiveQueueApplicationComponent component)
        {
            InitializeComponent();

            _component = component;

			_receiveTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_receiveTable.Table = _component.ReceiveTable;

			//_sendReceiveProgressTable.ToolbarModel = _component.ToolbarModel;
			//_sendReceiveProgressTable.MenuModel = _component.ContextMenuModel;

			_receiveTable.SelectionChanged += new EventHandler(OnSelectionChanged);

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_titleBar.DataBindings.Add("Text", _component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_receiveTable.Selection);
		}
    }
}
