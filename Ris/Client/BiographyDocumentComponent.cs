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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="BiographyDocumentComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class BiographyDocumentComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// BiographyDocumentComponent class
    /// </summary>
    [AssociateView(typeof(BiographyDocumentComponentViewExtensionPoint))]
    public class BiographyDocumentComponent : ApplicationComponent
    {
        private readonly BiographyDocumentTable _documentTable;
        private DocumentSummary _selectedDocument;
        private SimpleActionModel _documentActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyDocumentComponent()
        {
            _documentTable = new BiographyDocumentTable();
        }

        public override void Start()
        {
            _documentActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_documentActionHandler.AddAction("AddDocument", "Add Document", "AddToolSmall.png", "Add Document",
                delegate{ this.Host.DesktopWindow.ShowMessageBox("Start importing a scan/fax document", MessageBoxActions.Ok); });

            AddDummyDocuments();

            // Sort the table by descending date initially
            int sortColumnIndex = _documentTable.Columns.FindIndex(delegate(TableColumnBase<DocumentSummary> column)
                { return column.Name.Equals("Date"); });

            _documentTable.Sort(new TableSortParams(_documentTable.Columns[sortColumnIndex], false));

            base.Start();
        }

        #region Presentation Model

        public ITable Documents
        {
            get { return _documentTable; }
        }

        public ActionModelNode DocumentListActionModel
        {
            get { return _documentActionHandler; }
        }

        public ISelection SelectedDocument
        {
            get { return _selectedDocument == null ? Selection.Empty : new Selection(_selectedDocument); }
            set
            {
                _selectedDocument = (DocumentSummary)value.Item;
                DocumentSelectionChanged();
            }
        }

        public string Category
        {
            get { return _selectedDocument == null ? "" : _selectedDocument.Category; }
        }

        public string Institution
        {
            get { return _selectedDocument == null ? "" : _selectedDocument.Institution; }
        }

        public string DocumentDate
        {
            get { return _selectedDocument == null ? "" : Format.Date(_selectedDocument.DocumentDate); }
        }

        public string Reason
        {
            get { return _selectedDocument == null ? "" : _selectedDocument.Reason; }
        }

        public int Pages
        {
            get { return _selectedDocument == null ? 0 : _selectedDocument.Pages; }
        }

        public string CreatedStaff
        {
            get { return _selectedDocument == null ? "" : _selectedDocument.CreatedStaff; }
        }

        public DateTime CreatedTime
        {
            get { return _selectedDocument == null ? new DateTime() : _selectedDocument.CreatedTime; }
        }

        #endregion

        private void DocumentSelectionChanged()
        {
            NotifyAllPropertiesChanged();
        }

        private void AddDummyDocuments()
        {
            _documentTable.Items.Add(new DocumentSummary(
                "Lab Result", 
                new DateTime(2000, 10, 10, 12, 1, 5), 
                "Elm Street Clinic", 
                "Request for X-Ray", 
                3,
                "John Smith", 
                new DateTime(1950, 10, 10)));

            _documentTable.Items.Add(new DocumentSummary(
                "Outside Imaging Report",
                new DateTime(2000, 10, 10, 12, 1, 5),
                "TGH",
                "Request for X-Ray",
                1,
                "John Smith",
                new DateTime(1950, 10, 10)));

            _documentTable.Items.Add(new DocumentSummary(
                "Surgical Report",
                new DateTime(2000, 10, 10, 12, 1, 5),
                "TWH",
                "Heart Surgery",
                4,
                "John Smith",
                new DateTime(1950, 10, 10)));

            _documentTable.Items.Add(new DocumentSummary(
                "Miscellaneous",
                new DateTime(2000, 10, 10, 12, 1, 5),
                "Sunnybrook Hospital",
                "Heart Surgery",
                1,
                "John Smith",
                new DateTime(1950, 10, 10)));

        }

    }
}
