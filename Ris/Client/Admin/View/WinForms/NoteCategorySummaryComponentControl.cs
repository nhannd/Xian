using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="NoteCategorySummaryComponent"/>
    /// </summary>
    public partial class NoteCategorySummaryComponentControl : ApplicationComponentUserControl
    {
        private NoteCategorySummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategorySummaryComponentControl(NoteCategorySummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _noteCategories.ToolbarModel = _component.NoteCategoryListActionModel;
            _noteCategories.MenuModel = _component.NoteCategoryListActionModel;

            _noteCategories.Table = _component.NoteCategories;
            _noteCategories.DataBindings.Add("Selection", _component, "SelectedNoteCategory", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _noteCategories_Load(object sender, EventArgs e)
        {
        }

        private void _noteCategories_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedNoteCategory();
        }
    }
}
