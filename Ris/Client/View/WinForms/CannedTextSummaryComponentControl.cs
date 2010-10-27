#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

			_component.CopyCannedTextRequested += _component_CopyCannedTextRequested;
		}

		private void _component_CopyCannedTextRequested(object sender, EventArgs e)
		{
			string fullCannedText = _component.GetFullCannedText();
			if (!string.IsNullOrEmpty(fullCannedText))
				Clipboard.SetDataObject(fullCannedText, true);
		}

		private void _cannedTexts_ItemDrag(object sender, ItemDragEventArgs e)
		{
			string fullCannedText = _component.GetFullCannedText();
			if (!string.IsNullOrEmpty(fullCannedText))
				_cannedTexts.DoDragDrop(fullCannedText, DragDropEffects.All);
		}

		private void _cannedTexts_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditSelectedItems();
		}
	}
}
