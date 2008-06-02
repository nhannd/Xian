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

			_cannedTexts.Table = _component.SummaryTable;
			_cannedTexts.MenuModel = _component.SummaryTableActionModel;
			_cannedTexts.ToolbarModel = _component.SummaryTableActionModel;
			_cannedTexts.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_group.DataBindings.Add("Value", _component, "Group", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_text.DataBindings.Add("Value", _component, "Preview", true, DataSourceUpdateMode.OnPropertyChanged);
			_component.CopyCannedTextRequested += _component_CopyCannedTextRequested;
		}

		private void _component_CopyCannedTextRequested(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(_component.Preview))
				Clipboard.SetDataObject(_component.Preview, true);
		}

		private void _cannedTexts_ItemDrag(object sender, ItemDragEventArgs e)
		{
			_cannedTexts.DoDragDrop(_component.Preview, DragDropEffects.All);
		}

		private void _cannedTexts_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditSelectedItems();
		}
	}
}
