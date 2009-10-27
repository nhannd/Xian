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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Externals.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	public partial class ExternalPropertiesComponentControl : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly IExternalPropertiesComponent _component;
		private string _externalName;

		private ExternalPropertiesComponentControl() : base()
		{
			InitializeComponent();

			_txtName.DataBindings.Add("Text", this, "ExternalName", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		public ExternalPropertiesComponentControl(ExternalPropertiesComponent component)
			: this()
		{
			_component = component;

			_pnlExternalType.Visible = false;

			SetClientControl(component.ExternalGuiElement);
			this.ExternalName = component.External.Label;
		}

		public ExternalPropertiesComponentControl(AddNewExternalComponent component)
			: this()
		{
			_component = component;

			_pnlExternalType.Visible = true;

			foreach (AddNewExternalComponent.ExternalType externalType in component.ExternalTypes)
			{
				_cboExternalType.Items.Add(externalType);
			}
			_cboExternalType.SelectedItem = component.SelectedExternalType;

			SetClientControl(component.ExternalGuiElement);
			this.ExternalName = component.External.Label;
		}

		public string ExternalName
		{
			get { return _externalName; }
			set
			{
				if (_externalName != value)
				{
					if (_component.External.Label != value)
						_component.External.Label = value;

					_externalName = value;
					EventsHelper.Fire(this.PropertyChanged, this, new PropertyChangedEventArgs("ExternalName"));
				}
			}
		}

		private void SetClientControl(Control control)
		{
			_pnlClientArea.Controls.Clear();
			if (control != null)
			{
				Size size = control.Size;
				size.Height += _pnlExternalType.Height + _pnlName.Height;
				this.Size = size;

				control.Dock = DockStyle.Fill;
				_pnlClientArea.Size = new Size(_pnlClientArea.Width, control.Height);
				_pnlClientArea.Controls.Add(control);
			}
		}

		private void _cboExternalType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_component is AddNewExternalComponent)
			{
				AddNewExternalComponent addComponent = (AddNewExternalComponent) _component;
				addComponent.SelectedExternalType = _cboExternalType.SelectedItem as AddNewExternalComponent.ExternalType;
				SetClientControl(addComponent.ExternalGuiElement);
				this.ExternalName = addComponent.External.Label;
			}
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			if (!_component.Accept())
			{
				MessageBox.Show(this, Resources.MessagesErrorsExist);
			}
		}
	}
}