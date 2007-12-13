using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderNoteSummaryComponent"/>
    /// </summary>
    public partial class OrderNoteSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly OrderNoteSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderNoteSummaryComponentControl(OrderNoteSummaryComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _noteList.ToolbarModel = _component.NoteActionModel;
            _noteList.MenuModel = _component.NoteActionModel;
            _noteList.Table = _component.NoteTable;
            _noteList.DataBindings.Add("Selection", _component, "SelectedNote", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _noteList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedNote();
        }
    }
}
