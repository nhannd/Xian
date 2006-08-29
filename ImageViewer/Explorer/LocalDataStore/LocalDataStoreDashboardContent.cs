namespace ClearCanvas.ImageViewer.Dashboard.LocalDataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;
    using System.Windows.Forms;
    using System.IO;
	using ClearCanvas.Common;
    using ClearCanvas.Desktop;
    using ClearCanvas.ImageViewer.StudyManagement;
    using ClearCanvas.ImageViewer.Dashboard;
    using ClearCanvas.Desktop.Dashboard;
    using ClearCanvas.Controls.WinForms;
    using ClearCanvas.Dicom;
    using ClearCanvas.DataStore;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.Dashboard.DashboardContentExtensionPoint))]
    public class LocalDataStoreDashboardContent : DashboardContent
    {
        private MasterViewControl _masterView = new MasterViewControl();
        private StudySearchPanel _detailView = new StudySearchPanel();

        public LocalDataStoreDashboardContent()
        {
            _detailView.StudySearchForm.SearchClicked += OnSearch;
            _detailView.StudyGridView.DataGridView.CellDoubleClick += OnCellDoubleClick;

            ToolStripButton button = new ToolStripButton("Open");
			button.ToolTipText = "Open selected study";
			button.Image = new Bitmap(typeof(LocalDataStoreDashboardContent), "Icons.OpenStudySmall.png");
            button.Click += OnStudyOpen;
            _detailView.StudyGridView.ToolStrip.Items.Add(button);

            button = new ToolStripButton("Delete");
			button.ToolTipText = "Delete selected study";
			button.Image = new Bitmap(typeof(LocalDataStoreDashboardContent), "Icons.DeleteStudySmall.png");
            button.Click += OnDelete;
            _detailView.StudyGridView.ToolStrip.Items.Add(button);

            _detailView.HeaderText = "Search My DataStore";
            _detailView.StudyGridView.AddColumn("Patient ID");
            _detailView.StudyGridView.AddColumn("Last Name");
            _detailView.StudyGridView.AddColumn("First Name");
            _detailView.StudyGridView.AddColumn("DOB");
            _detailView.StudyGridView.AddColumn("Description");
            _detailView.StudyGridView.AddColumn("Date");
            _detailView.StudyGridView.AddColumn("Accession");
            _detailView.StudyGridView.AddColumn("Modalities In Study");
        }

        public override string Name
        {
            get { return "My DataStore"; }
        }

        public override void OnSelected()
        {
            base.MasterView = _masterView;
            base.DetailView = _detailView;
        }

        private void OnSearch(object sender, EventArgs e)
        {
            using (new CursorManager(_detailView, Cursors.WaitCursor))
            {
                _detailView.StudyGridView.DataGridView.Rows.Clear();

                IStudyFinder localStudyFinder = ImageViewerComponent.StudyManager.StudyFinders["My DataStore"];

                string patientId = _detailView.StudySearchForm.PatientId.ToString();
                // string patientsName = "" + _detailView.StudySearchForm.LastName.ToString() + "*^*" + _detailView.StudySearchForm.FirstName.ToString() + "*";
                StringBuilder patientsName = new StringBuilder(64);
                if (_detailView.StudySearchForm.LastName.Length > 0)
                    patientsName.Append(_detailView.StudySearchForm.LastName.ToString());
                if (_detailView.StudySearchForm.FirstName.Length > 0)
                    patientsName.AppendFormat("%^{0}%", _detailView.StudySearchForm.FirstName.ToString());

                string accessionNumber = _detailView.StudySearchForm.AccessionNumber.ToString();
                string studyDescription = _detailView.StudySearchForm.StudyDescription.ToString();

                QueryParameters queryParams = new QueryParameters();
                queryParams.Add("PatientsName", patientsName.ToString());
                queryParams.Add("PatientId", patientId);
                queryParams.Add("AccessionNumber", accessionNumber);
                queryParams.Add("StudyDescription", studyDescription);

                StudyItemList studyItemList = localStudyFinder.Query(queryParams);
                if (null == studyItemList)
                    return;

                foreach (StudyItem item in studyItemList)
                {
                    string[] itemData = new string[7];
                    itemData[0] = item.PatientId;
                    itemData[1] = item.LastName;
                    itemData[2] = item.FirstName;
                    itemData[3] = item.PatientsBirthDate;
                    itemData[4] = item.StudyDescription;
                    itemData[5] = item.StudyDate;
                    itemData[6] = item.AccessionNumber;
                    //itemData[7] = item.ModalitiesInStudy;

                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = item;

                    for (int i = 0; i < itemData.Length; i++)
                    {
                        DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                        row.Cells.Add(cell);
                    }

                    bool result = row.SetValues(itemData);

                    _detailView.StudyGridView.DataGridView.Rows.Add(row);
                }
            }
        }

        private void OnStudyOpen(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = _detailView.StudyGridView.DataGridView.SelectedRows;

			if (selectedRows.Count == 0)
				return;

            DataGridViewRow row = selectedRows[0];
            OpenStudy(row.Tag as StudyItem);
        }

        private void OnDelete(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = _detailView.StudyGridView.DataGridView.SelectedRows;

			if (selectedRows.Count == 0)
				return;
			
            DataGridViewRow row = selectedRows[0];
            DeleteStudy(row.Tag as StudyItem);
            OnSearch(sender, e);
        }

        private void OnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = _detailView.StudyGridView.DataGridView.Rows[e.RowIndex];
            OpenStudy(row.Tag as StudyItem);
        }

        private void OpenStudy(StudyItem item)
        {
            using (new CursorManager(_detailView, Cursors.WaitCursor))
            {
                IStudyLoader studyLoader = ImageViewerComponent.StudyManager.StudyLoaders[item.StudyLoaderName];
                studyLoader.LoadStudy(item.StudyInstanceUID);
            }
        }

        private void DeleteStudy(StudyItem item)
        {
            using (new CursorManager(_detailView, Cursors.WaitCursor))
            {
                using (DatabaseConnector database = new DatabaseConnector(_connectionString))
                {
                    try
                    {
                        database.SetupConnector();

                        List<LocationUri> listOfFiles = database.SopInstanceLocationQuery(new Uid(item.StudyInstanceUID));
                        foreach (LocationUri uri in listOfFiles)
                        {
                            // TODO: Don't use literal string comparisons
                            if ("file" == uri.ProtocolPart)
                            {
                                System.IO.File.Delete(uri.LocationPart);
                            }
                        }

                        database.DeleteStudy(new Uid(item.StudyInstanceUID));
                        database.TeardownConnector();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
						Platform.Log(e, LogLevel.Error);
                        MessageBox.Show("Can't connect to data store", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private ApplicationConnectionString _connectionString = new ApplicationConnectionString();
    }
}
