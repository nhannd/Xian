using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyNoteComponent"/>
    /// </summary>
    public partial class BiographyNoteComponentControl : CustomUserControl
    {
        private BiographyNoteComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyNoteComponentControl(BiographyNoteComponent component)
        {
            InitializeComponent();
            _component = component;

            this.Dock = DockStyle.Fill;

            _noteTable.Table = _component.Notes;
            _noteTable.DataBindings.Add("Selection", _component, "SelectedNote", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
