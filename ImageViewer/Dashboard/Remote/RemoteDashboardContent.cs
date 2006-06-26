namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using ClearCanvas.Desktop.Dashboard;
    using ClearCanvas.Controls.WinForms;
    using ClearCanvas.ImageViewer.StudyManagement;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;
    using ClearCanvas.DataStore;
    using ClearCanvas.Common;
    using ClearCanvas.Desktop;
    using System.Threading;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.Dashboard.DashboardContentExtensionPoint))]
    public class RemoteDashboardContent : DashboardContent
	{
		private MasterViewControl _masterView = new MasterViewControl();
		private StudySearchPanel _detailView = new StudySearchPanel();

		public RemoteDashboardContent()
		{
			_detailView.HeaderText = "Search Remote Server";
            _detailView.StudySearchForm.SearchClicked += OnSearch;

            ToolStripButton button = new ToolStripButton("Retrieve");
            button.ToolTipText = "Retrieve study from selected server";
            button.Click += OnRetrieve;
            _detailView.StudyGridView.ToolStrip.Items.Add(button);

            // put in a separator to space out the buttons
            _detailView.StudyGridView.ToolStrip.Items.Add(new ToolStripSeparator());

            ToolStripLabel label = new ToolStripLabel("Storage Folder: " + _storagePath, null, false, null, "_storagePathLabel");
            label.ToolTipText = "Specify where retrieved images and objects will be stored on your local storage";
            _detailView.StudyGridView.ToolStrip.Items.Add(label);

            ToolStripButton browseButton = new ToolStripButton("...");
            browseButton.ToolTipText = "Use a file browser dialog to pick the storage location";
            browseButton.Click += new EventHandler(OnBrowseStorageFolder);
            _detailView.StudyGridView.ToolStrip.Items.Add(browseButton);

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
			get { return "My PACS Network"; }
		}

        private void OnBrowseStorageFolder(Object source, EventArgs args)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Choose the folder where images will be stored";
            folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _storagePath.Set(folderDialog.SelectedPath);
                _storagePath.Save();
                _detailView.StudyGridView.ToolStrip.Items["_storagePathLabel"].Text = "Storage Folder: " + folderDialog.SelectedPath;
            }
        }

        private void OnSearch(object sender, EventArgs e)
        {
            using (new CursorManager(_detailView, Cursors.WaitCursor))
            {
                _detailView.StudyGridView.DataGridView.Rows.Clear();

                IStudyFinder remoteStudyFinder = ImageWorkspace.StudyManager.StudyFinders["My PACS Network"];

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

                StudyItemList studyItemList = remoteStudyFinder.Query<Server>(_masterView.SelectedServer, queryParams);
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

		public override void OnSelected()
		{
			base.MasterView = _masterView;
			base.DetailView = _detailView;
		}

        private void OnRetrieve(object sender, EventArgs e)
        {
            if ("" == _storagePath)
            {
                Platform.ShowMessageBox("Storage path must be set in order to perform retrieves.");
                return;
            }

            // get updated connection string first
            _connectionString.Load();

            DataGridViewSelectedRowCollection selectedRows = _detailView.StudyGridView.DataGridView.SelectedRows;

            DataGridViewRow row = selectedRows[0];
            RetrieveStudy(row.Tag as StudyItem);
        }

        private void RetrieveStudy(StudyItem item)
        {
            using (new CursorManager(_detailView, Cursors.WaitCursor))
            {
                // check first to make sure that we can connect to the database
                using (DatabaseConnector connector = new DatabaseConnector(_connectionString))
                {
                    try
                    {
                        connector.SetupConnector();
                        connector.TeardownConnector();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        Platform.ShowMessageBox("Can't connect to data store: " + e.ToString());
                        return;
                    }
                }

                DicomClient client = new DicomClient(new ApplicationEntity(new HostName("localhost"), new AETitle("CCWORKSTN"), new ListeningPort(4000)));

                // see if we have NumberOfStudyRelatedInstances, if not, we have to query for it
                if (0 == item.NumberOfStudyRelatedInstances)
                {
                    ReadOnlyQueryResultCollection results = client.QuerySopInstance(item.Server, new Uid(item.StudyInstanceUID));
                    item.NumberOfStudyRelatedInstances = (UInt32) results.Count;
                }

                RetrieveProgressForm progressForm = new RetrieveProgressForm(item.NumberOfStudyRelatedInstances);
                progressForm.DisplayText = item.FirstName + " " + item.LastName + " / " + item.StudyDescription + " (" + item.StudyDate + ")";
                RetrieveThreadObject rto = new RetrieveThreadObject(client, item, _storagePath, progressForm, _connectionString);

                Thread t = new Thread(new ThreadStart(rto.WorkMethod));
                t.IsBackground = true;
                t.Start();

                progressForm.ShowDialog();
            }
        }

        private class RetrieveThreadObject
        {
            public RetrieveThreadObject(DicomClient client, StudyItem item, String path, RetrieveProgressForm progressForm, ApplicationConnectionString connectionString)
            {
                _client = client;
                _item = item;
                _storagePath = path;
                _progressForm = progressForm;
                _connectionString = connectionString;
            }

            public void WorkMethod()
            {
                _client.SopInstanceReceived += new EventHandler<SopInstanceReceivedEventArgs>(NewImageEventHandler);

                try
                {
                    _client.Retrieve(_item.Server, new Uid(_item.StudyInstanceUID), _storagePath);
                    _client.Dispose();
                }
                catch (NetworkDicomException e)
                {
					Platform.Log(e, LogLevel.Error);
				}

                if (null == _progressForm || !_progressForm.Created)
                    return;

                _progressForm.BeginInvoke((ThreadStart)delegate()
                {
                    _progressForm.Close();
                });
            }

            private void NewImageEventHandler(object sender, SopInstanceReceivedEventArgs args)
            {
                _connectionString.Load();
                using (DatabaseConnector connector = new DatabaseConnector(_connectionString))
                {
                    try
                    {
                        connector.SetupConnector();
                        connector.InsertSopInstance(args.SopFileName);
                        connector.TeardownConnector();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
						Platform.Log(e, LogLevel.Error);
						return;
                    }
                }

                if (null == _progressForm || !_progressForm.Created)
                    return;

                _progressForm.BeginInvoke((ThreadStart)delegate()
                {
                    _progressForm.StepProgress();
                });
            }

            private DicomClient _client;
            private StudyItem _item;
            private RetrieveProgressForm _progressForm;
            private String _storagePath;
            private ApplicationConnectionString _connectionString;
        }

        private ApplicationConnectionString _connectionString = new ApplicationConnectionString();
        private StoragePath _storagePath = new StoragePath();
	}
}
