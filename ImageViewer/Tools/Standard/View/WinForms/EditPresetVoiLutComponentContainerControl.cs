using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="EditPresetVoiLutComponentContainer"/>
    /// </summary>
    public partial class EditPresetVoiLutComponentContainerControl : ApplicationComponentUserControl
    {
        private readonly EditPresetVoiLutComponentContainer _component;

		/// <summary>
        /// Constructor
        /// </summary>
        public EditPresetVoiLutComponentContainerControl(EditPresetVoiLutComponentContainer component)
            :base(component)
        {
			_component = component;
			IApplicationComponentView customEditView = _component.EditComponentHost.ComponentView;

			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			Size sizeBefore = _tableLayoutPanel.Size;

			_tableLayoutPanel.Controls.Add(customEditView.GuiElement as Control);
			_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			Size sizeAfter = _tableLayoutPanel.Size;

			this.Size += (sizeAfter - sizeBefore);

			_keyStrokeComboBox.DataSource = _component.AvailableKeyStrokes;
			_keyStrokeComboBox.DataBindings.Add("Value", source, "SelectedKeyStroke", true, DataSourceUpdateMode.OnPropertyChanged);
			
			BindingSource hostSource = new BindingSource();
			hostSource.DataSource = _component.EditComponentHost.Component;

			base.AcceptButton = _okButton;
			base.CancelButton = _cancelButton;

			_okButton.DataBindings.Add("Enabled", hostSource, "Valid", true, DataSourceUpdateMode.OnPropertyChanged);

			_cancelButton.Click += delegate { _component.Cancel(); };
			_okButton.Click += delegate { _component.OK(); };
        }
    }
}
