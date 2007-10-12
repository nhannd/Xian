#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
    /// Provides a Windows Forms user-interface for <see cref="BiographyDocumentComponent"/>
    /// </summary>
    public partial class BiographyDocumentComponentControl : CustomUserControl
    {
        private BiographyDocumentComponent _component;
        private int _totalPage = 0;
        private int _pageIndex = 0;
        private int _printPageIndex = 0;
        private System.Drawing.Printing.PrintDocument _printDocument;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyDocumentComponentControl(BiographyDocumentComponent component)
        {
            InitializeComponent();
            _component = component;

            _documentList.ToolbarModel = _component.DocumentListActionModel;
            _documentList.MenuModel = _component.DocumentListActionModel;
            _documentList.Table = _component.Documents;
            _documentList.DataBindings.Add("Selection", _component, "SelectedDocument", true, DataSourceUpdateMode.OnPropertyChanged);
            
            _category.DataBindings.Add("Text", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
            _institution.DataBindings.Add("Value", _component, "Institution", true, DataSourceUpdateMode.OnPropertyChanged);
            _documentDate.DataBindings.Add("Value", _component, "DocumentDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _reason.DataBindings.Add("Value", _component, "Reason", true, DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("TotalPages", _component, "Pages", true, DataSourceUpdateMode.OnPropertyChanged);

            _printDocument = new System.Drawing.Printing.PrintDocument();
            _printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(_printDocument_PrintPage);

            ResetPage();
        }

        public int TotalPages
        {
            get { return _totalPage; }
            set 
            { 
                _totalPage = value;
                ResetPage();
            }
        }

        #region Button Events

        private void _prevPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex--;
            if (_pageIndex < 0)
                _pageIndex = 0;

            UpdateImage();
        }

        private void _nextPageButton_Click(object sender, EventArgs e)
        {
            _pageIndex++;
            if (_pageIndex > _totalPage - 1)
                _pageIndex = _totalPage - 1;

            UpdateImage();
        }

        private void _previewButton_Click(object sender, EventArgs e)
        {
            ShowPreview();
        }

        private void _documentList_ItemDoubleClicked(object sender, EventArgs e)
        {
            ShowPreview();
        }

        #endregion

        #region Helper Methods

        private void ResetPage()
        {
            _pageIndex = 0;
            UpdateImage();
        }

        private void UpdateImage()
        {
            _pageIndicator.Text = String.Format("{0} of {1}", _pageIndex + 1, _totalPage);
        }

        private void ShowPreview()
        {
            _printDocument.DocumentName = _category.Text;
            _printPageIndex = 0;

            _printPreviewDialog.Document = _printDocument;
            _printPreviewDialog.ShowDialog();
        }

        #endregion

        void _printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(_previewImage.Image, new Point(0, 0));

            e.HasMorePages = _printPageIndex < _totalPage - 1;

            _printPageIndex++;
        }
    }
}
