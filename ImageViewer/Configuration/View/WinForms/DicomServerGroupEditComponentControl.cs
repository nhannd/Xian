#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerGroupEditComponent"/>
    /// </summary>
    public partial class DicomServerGroupEditComponentControl : ApplicationComponentUserControl
	{
        private readonly DicomServerGroupEditComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerGroupEditComponentControl(DicomServerGroupEditComponent component)
			: base(component)
		{
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;
			this.CancelButton = _btnCancel;
			
			AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

            this._serverGroupName.DataBindings.Add("Text", _component, "ServerGroupName", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

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
