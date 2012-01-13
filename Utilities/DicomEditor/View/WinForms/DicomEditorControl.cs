#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    public partial class DicomEditorControl : UserControl
    {
        private ClearCanvas.Utilities.DicomEditor.DicomEditorComponent _dicomEditorComponent;

        public DicomEditorControl(DicomEditorComponent component)
        {
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _dicomEditorComponent = component;

            _dicomTagTable.Table = _dicomEditorComponent.DicomTagData;
            _dicomTagTable.ToolbarModel = _dicomEditorComponent.ToolbarModel;
            _dicomTagTable.MenuModel = _dicomEditorComponent.ContextMenuModel;
            _dicomTagTable.SelectionChanged +=new EventHandler(OnDicomTagTableSelectionChanged);          
            _dicomTagTable.MultiLine = true;
            
            _dicomEditorTitleBar.DataBindings.Add("Text", _dicomEditorComponent, "DicomFileTitle", true, DataSourceUpdateMode.OnPropertyChanged);
            
        }

        void OnDicomTagTableSelectionChanged(object sender, EventArgs e)
        {
            _dicomEditorComponent.SetSelection(_dicomTagTable.Selection);
        }
    }
}
