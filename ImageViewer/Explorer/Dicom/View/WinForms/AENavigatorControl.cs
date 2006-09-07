using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class AENavigatorControl : UserControl
	{
        private AENavigatorComponent _aenavigatorComponent;
        private BindingSource _bindingSource;
        
        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _aenavigatorComponent = component;
            _aeserverTreeForm1.UpdateClicked += new EventHandler(OnUpdateClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _aenavigatorComponent;

            _aeserverTreeForm1.ServerName.DataBindings.Add("Text", _bindingSource, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerDesc.DataBindings.Add("Text", _bindingSource, "ServerDesc", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerAE.DataBindings.Add("Text", _bindingSource, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerHost.DataBindings.Add("Text", _bindingSource, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerPort.DataBindings.Add("Text", _bindingSource, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        void OnUpdateClicked(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _aenavigatorComponent.Update();
            }
        }
    }
}
