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

namespace ClearCanvas.ImageViewer.Services.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerEditComponent"/>
    /// </summary>
	public partial class DicomServerEditComponentControl : ApplicationComponentUserControl
    {
        private DicomServerEditComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerEditComponentControl(DicomServerEditComponent component)
			: base(component)
		{
            InitializeComponent();

            _component = component;

			this.AcceptButton = _btnAccept;
			this.CancelButton = _btnCancel;

            AcceptClicked += new EventHandler(OnAcceptClicked);
            CancelClicked += new EventHandler(OnCancelClicked);

			this._serverName.DataBindings.Add("Text", _component, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
			this._location.DataBindings.Add("Text", _component, "ServerLocation", true, DataSourceUpdateMode.OnPropertyChanged);
			this._ae.DataBindings.Add("Text", _component, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
			this._host.DataBindings.Add("Text", _component, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
			_btnAccept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding portBinding = new Binding("Text", _component, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);
			portBinding.Format += new ConvertEventHandler(OnPortBindingFormat);
			portBinding.Parse += new ConvertEventHandler(OnPortBindingParse);
        	_port.DataBindings.Add(portBinding);

            this._serverName.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);
            this._host.DataBindings.Add("Readonly", _component, "FieldReadonly", true, DataSourceUpdateMode.OnPropertyChanged);

        	this._isStreaming.DataBindings.Add("Checked", _component, "IsStreaming", true, DataSourceUpdateMode.OnPropertyChanged);
        	this._headerServicePort.DataBindings.Add("Text", _component, "HeaderServicePort", true, DataSourceUpdateMode.OnPropertyChanged);
			this._wadoServicePort.DataBindings.Add("Text", _component, "WadoServicePort", true, DataSourceUpdateMode.OnPropertyChanged);
			this._headerServicePort.DataBindings.Add("Enabled", _component, "IsStreaming", true, DataSourceUpdateMode.OnPropertyChanged);
			this._wadoServicePort.DataBindings.Add("Enabled", _component, "IsStreaming", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnPortBindingFormat(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			if ((int)e.Value <= 0)
				e.Value = "";
			else 
				e.Value = e.Value.ToString();
		}

		void OnPortBindingParse(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(int))
				return;

			int value;
			if (!(e.Value is string) || !int.TryParse((string)e.Value, out value))
			{
				e.Value = 0;
			}
			else
			{
				e.Value = value;
			}
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
