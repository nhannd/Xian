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
    /// Provides a Windows Forms user-interface for <see cref="AEServerGroupEditorComponent"/>
    /// </summary>
    public partial class AEServerGroupEditorComponentControl : CustomUserControl
    {
        private AEServerGroupEditorComponent _component;
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
        public AEServerGroupEditorComponentControl(AEServerGroupEditorComponent component)
        {
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;

            AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _component;
            this._serverGroupName.DataBindings.Add("Text", _bindingSource, "ServerGroupName", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
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
