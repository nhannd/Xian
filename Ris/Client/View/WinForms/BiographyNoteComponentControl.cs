#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyNoteComponent"/>
    /// </summary>
    public partial class BiographyNoteComponentControl : ApplicationComponentUserControl
    {
        private readonly BiographyNoteComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyNoteComponentControl(BiographyNoteComponent component)
        {
            InitializeComponent();
            _component = component;

            this.Dock = DockStyle.Fill;

            _noteTable.Table = _component.NoteTable;
            _noteTable.DataBindings.Add("Selection", _component, "SelectedNote", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
