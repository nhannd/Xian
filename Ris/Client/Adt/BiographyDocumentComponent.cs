using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
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
        private BiographyDocumentTable _documentTable;
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
            _documentActionHandler.AddAction("AddDocument", "Add Document", "Add.png", "Add Document",
                delegate() { Platform.ShowMessageBox("Start importing a scan/fax document"); });

            AddDummyDocuments();

            // Sort the table by descending date initially
            int sortColumnIndex = _documentTable.Columns.FindIndex(delegate(TableColumnBase<DocumentSummary> column)
                { return column.Name.Equals("Date"); });

            _documentTable.Sort(new TableSortParams(_documentTable.Columns[sortColumnIndex], false));

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
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

        public DateTime DocumentDate
        {
            get { return _selectedDocument == null ? new DateTime() : _selectedDocument.DocumentDate; }
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
