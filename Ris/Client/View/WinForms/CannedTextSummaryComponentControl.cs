using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="CannedTextSummaryComponent"/>
    /// </summary>
    public partial class CannedTextSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly CannedTextSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CannedTextSummaryComponentControl(CannedTextSummaryComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_cannedTexts.ToolbarModel = _component.CannedTextActionModel;
			_cannedTexts.MenuModel = _component.CannedTextActionModel;
			_cannedTexts.Table = _component.CannedTextTable;
			_cannedTexts.DataBindings.Add("Selection", _component, "SelectedCannedText", true, DataSourceUpdateMode.OnPropertyChanged);

			_text.DataBindings.Add("Value", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.CopyCannedTextRequested += _component_CopyCannedTextRequested;
		}

		private void _component_CopyCannedTextRequested(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_component.Text))
				Clipboard.SetDataObject(_component.Text, true);
		}

		private void _cannedTexts_ItemDrag(object sender, ItemDragEventArgs e)
		{
			_cannedTexts.DoDragDrop(_component.Text, DragDropEffects.All);
		}

		private void _cannedTexts_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditCannedText();
		}
	}
}
