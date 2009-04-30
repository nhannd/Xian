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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    public partial class PresetVoiLutOperationComponentContainerControl : ApplicationComponentUserControl
    {
        private readonly PresetVoiLutOperationsComponentContainer _component;

		/// <summary>
        /// Constructor
        /// </summary>
        public PresetVoiLutOperationComponentContainerControl(PresetVoiLutOperationsComponentContainer component)
            :base(component)
        {
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_keyStrokeComboBox.DataSource = _component.AvailableKeyStrokes;
			_keyStrokeComboBox.DataBindings.Add("Value", source, "SelectedKeyStroke", true, DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Enabled", source, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			if (_component.ComponentHost.HasAssociatedView)
			{
				IApplicationComponentView customEditView = _component.ComponentHost.ComponentView;
				Size sizeBefore = _tableLayoutPanel.Size;

				_tableLayoutPanel.Controls.Add(customEditView.GuiElement as Control);
				_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

				Size sizeAfter = _tableLayoutPanel.Size;

				this.Size += (sizeAfter - sizeBefore);
			}

			base.AcceptButton = _okButton;
			base.CancelButton = _cancelButton;

			_cancelButton.Click += delegate { _component.Cancel(); };
			_okButton.Click += delegate { _component.OK(); };
        }
    }
}
