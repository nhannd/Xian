using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    public partial class PresetVoiLutApplicatorComponentContainerControl : ApplicationComponentUserControl
    {
        private readonly PresetVoiLutApplicatorComponentContainer _component;

		/// <summary>
        /// Constructor
        /// </summary>
        public PresetVoiLutApplicatorComponentContainerControl(PresetVoiLutApplicatorComponentContainer component)
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
