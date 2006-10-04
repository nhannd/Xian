using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AEServerEditorComponent"/>
    /// </summary>
    public partial class AEServerEditorComponentControl : CustomUserControl
    {
        private AEServerEditorComponent _component;
        private BindingSource _bindingSource;

        public event EventHandler AcceptClicked
        {
            add { _btnAccept.Click += value; }
            remove { _btnAccept.Click -= value; }
        }

        public event EventHandler CancelClicked
        {
            add { _btnCancel.Click += value; }
            remove { _btnCancel.Click -= value; }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public AEServerEditorComponentControl(AEServerEditorComponent component)
        {
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;

            AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _component;
            this._serverName.DataBindings.Add("Text", _bindingSource, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            this._location.DataBindings.Add("Text", _bindingSource, "ServerLocation", true, DataSourceUpdateMode.OnPropertyChanged);
            this._aetitle.DataBindings.Add("Text", _bindingSource, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
            this._host.DataBindings.Add("Text", _bindingSource, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            this._port.DataBindings.Add("Text", _bindingSource, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            this._serverName.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);
            this._host.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void OnAcceptClicked(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
