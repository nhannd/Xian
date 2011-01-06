#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;
using System;

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

			IApplicationComponentView customEditView;
			try
			{
				customEditView = _component.ComponentHost.ComponentView;

				Size sizeBefore = _tableLayoutPanel.Size;

				_tableLayoutPanel.Controls.Add(customEditView.GuiElement as Control);
				_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

				Size sizeAfter = _tableLayoutPanel.Size;

				this.Size += (sizeAfter - sizeBefore);
			}
			catch(NotSupportedException)
			{
			}

			base.AcceptButton = _okButton;
			base.CancelButton = _cancelButton;

			_cancelButton.Click += delegate { _component.Cancel(); };
			_okButton.Click += delegate { _component.OK(); };
        }
    }
}
