#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public partial class SearchResult
	{
		protected const string ColumnPatientId = @"Patient ID";
		protected const string ColumnLastName = @"Last Name";
		protected const string ColumnFirstName = @"First Name";
		protected internal const string ColumnIdeographicName = @"Ideographic Name";
		protected internal const string ColumnPhoneticName = @"Phonetic Name";
		protected const string ColumnDateOfBirth = @"DOB";
		protected const string ColumnAccessionNumber = @"Accession Number";
		protected const string ColumnStudyDate = @"Study Date";
		protected const string ColumnStudyDescription = @"Study Description";
		protected const string ColumnModality = @"Modality";
		protected const string ColumnAttachments = @"Attachments";
		protected const string ColumnReferringPhysician = @"Referring Physician";
		protected internal const string ColumnNumberOfInstances = @"Instances";
		protected const string ColumnServer = @"Server";
		protected const string ColumnAvailability = @"Availability";
        protected const string ColumnDeleteOn = @"Delete On";

		private string _serverGroupName;
		private bool _isLocalServer;
		private int _numberOfChildServers;

		private readonly Table<StudyTableItem> _studyTable;
        private readonly List<StudyTableItem> _hiddenItems;

		private bool _filterDuplicates;
		private bool _everSearched;
	    private string _resultsTitle;

	    public SearchResult()
		{
			_everSearched = false;
			HasDuplicates = false;

			_serverGroupName = "";
			_isLocalServer = false;
			_numberOfChildServers = 1;

            _hiddenItems = new List<StudyTableItem>();
            _studyTable = new Table<StudyTableItem>();
            _setChangedStudies = new Dictionary<string, string>();
		}

		#region Properties

		public string ServerGroupName
		{
			get { return _serverGroupName; }
			set { _serverGroupName = value; }
		}

		public bool IsLocalServer
		{
			get { return _isLocalServer; }
			set
			{
                if (Equals(_isLocalServer, value))
                    return;

				_isLocalServer = value;
				UpdateServerColumnsVisibility();
                if (_isLocalServer)
                    StartMonitoringStudies();
                else
                    StopMonitoringStudies();
			}
		}

		public int NumberOfChildServers
		{
			get { return _numberOfChildServers; }
			set
			{
				_numberOfChildServers = value;
				UpdateServerColumnsVisibility();
			}
		}

		public Table<StudyTableItem> StudyTable
		{
			get { return _studyTable; }
		}

		public string ResultsTitle
		{
            get { return _resultsTitle; }
            private set
            {
                if (Equals(_resultsTitle, value))
                    return;

                _resultsTitle = value;
                EventsHelper.Fire(ResultsTitleChanged, this, EventArgs.Empty);
            }
		}

	    public event EventHandler ResultsTitleChanged;

		public bool HasDuplicates { get; private set; }

		public bool FilterDuplicates
		{
			get { return _filterDuplicates; }
			set
			{
				_filterDuplicates = value;
				if (_filterDuplicates)
				{
					if (_hiddenItems.Count == 0)
					{
					    RemoveDuplicates(_studyTable.Items, _hiddenItems);
					}
				}
				else
				{
					if (_hiddenItems.Count > 0)
					{
						_studyTable.Items.AddRange(_hiddenItems);
						_hiddenItems.Clear();
					}
				}

				StudyTable.Sort();
                SetResultsTitle();
			}
		}
		#endregion

		#region Methods

		public void Initialize()
		{
			InitializeTable();
		    SetResultsTitle();
		}

        public void Refresh(List<StudyTableItem> tableItems, bool filterDuplicates)
		{
			_everSearched = true;
			_filterDuplicates = filterDuplicates;

            _setChangedStudies.Clear();

			_hiddenItems.Clear();
            var filteredItems = new List<StudyTableItem>(tableItems);
			RemoveDuplicates(filteredItems, _hiddenItems);
			HasDuplicates = _hiddenItems.Count > 0;

			if (!_filterDuplicates)
			{
				_hiddenItems.Clear();
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(tableItems);
			}
			else
			{
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(filteredItems);
			}

			StudyTable.Sort();
            SetResultsTitle();
        }

        private void SetResultsTitle()
        {
            if (!_everSearched)
            {
                ResultsTitle = Reindexing
                                   ? String.Format(SR.FormatNeverSearchedReindexing, _serverGroupName)
                                   : _serverGroupName;
            }
            else
            {
                ResultsTitle = Reindexing
                                   ? String.Format(SR.FormatStudiesFoundReindexing, _studyTable.Items.Count, _serverGroupName)
                                   : String.Format(SR.FormatStudiesFound, _studyTable.Items.Count, _serverGroupName);
            }
        }

        private static void RemoveDuplicates(IList<StudyTableItem> allItems, List<StudyTableItem> removedItems)
		{
			removedItems.Clear();

            var uniqueItems = new Dictionary<string, StudyTableItem>();
            foreach (StudyTableItem item in allItems)
			{
				StudyTableItem existing;
                if (uniqueItems.TryGetValue(item.StudyInstanceUid, out existing))
                {
                    var server = item.Server;
					//we will only replace an existing entry if this study's server is streaming.
					if (server != null && server.StreamingParameters != null)
					{
						//only replace existing entry if it is on a non-streaming server.
                        server = existing.Server;
                        if (server == null || server.StreamingParameters == null)
						{
							removedItems.Add(existing);
							uniqueItems[item.StudyInstanceUid] = item;
							continue;
						}
					}

					//this study is a duplicate.
					removedItems.Add(item);
				}
				else
				{
                    uniqueItems[item.StudyInstanceUid] = item;
				}
			}

			foreach (StudyTableItem removedItem in removedItems)
				allItems.Remove(removedItem);
		}

	    protected virtual void InitializeTable()
		{
			_studyTable.Columns.AddRange(CreateExtensionColumns());
			_studyTable.Columns.AddRange(CreateDefaultColumns());
			_studyTable.Columns.AddRange(CreateInstanceCountColumns());
			_studyTable.Columns.AddRange(CreateServerColumns());

			// Default: Sort by last name
			var column = FindColumn(ColumnLastName);
			_studyTable.Sort(new TableSortParams(column, true));
		}

		protected IEnumerable<TableColumnBase<StudyTableItem>> CreateExtensionColumns()
		{
			var columns = new List<TableColumnBase<StudyTableItem>>();
			try
			{
				// Create and add any extension columns
				var xp = new StudyColumnExtensionPoint();
                foreach (IStudyColumn extensionColumn in xp.CreateExtensions())
                {
                    IStudyColumn newColumn = extensionColumn;

					var column = new TableColumn<StudyTableItem, string>(
						newColumn.Name,
						item => (newColumn.GetValue(item) ?? "").ToString(),
						newColumn.WidthFactor);

					newColumn.ColumnValueChanged += OnColumnValueChanged;
					columns.Add(column);
				}
			}
			catch (NotSupportedException) { }
			return columns;
		}

		protected static IEnumerable<TableColumnBase<StudyTableItem>> CreateDefaultColumns()
		{
			var columns = new List<TableColumnBase<StudyTableItem>>();
			TableColumn<StudyTableItem, string> column;

			column = new TableColumn<StudyTableItem, string>(
				ColumnPatientId,
				SR.ColumnHeadingPatientId,
				item => item.PatientId,
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnLastName,
				SR.ColumnHeadingLastName,
				item => new PersonName(item.PatientsName).LastName,
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnFirstName,
				SR.ColumnHeadingFirstName,
                item => new PersonName(item.PatientsName).FirstName,
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnIdeographicName,
				SR.ColumnHeadingIdeographicName,
				item => new PersonName(item.PatientsName).Ideographic,
				0.5f) { Visible = false };

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnPhoneticName,
				SR.ColumnHeadingPhoneticName,
                item => new PersonName(item.PatientsName).Phonetic,
				0.5f) { Visible = false };

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnDateOfBirth,
				SR.ColumnHeadingDateOfBirth,
				item => FormatDicomDA(item.PatientsBirthDate),
				null,
				0.4F,
				//TODO (Marmot):
				(one, two) => one.PatientsBirthDate.CompareTo(two.PatientsBirthDate));

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnAccessionNumber,
				SR.ColumnHeadingAccessionNumber,
				item => item.AccessionNumber,
				0.45F);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnStudyDate,
				SR.ColumnHeadingStudyDate,
				item => FormatDicomDA(item.StudyDate),
				null,
				0.4F,
				(one, two) => one.StudyDate.CompareTo(two.StudyDate));

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnStudyDescription,
				SR.ColumnHeadingStudyDescription,
				item => item.StudyDescription,
				0.75F);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(
				ColumnModality,
				SR.ColumnHeadingModality,
				item => DicomStringHelper.GetDicomStringArray(SortModalities(item.ModalitiesInStudy)),
				0.25f);

			columns.Add(column);

			var iconColumn = new TableColumn<StudyTableItem, IconSet>(
				ColumnAttachments,
				SR.ColumnHeadingAttachments,
				GetAttachmentsIcon,
				0.25f)
								{
									ResourceResolver = new ApplicationThemeResourceResolver(typeof(SearchResult).Assembly),
                                    Comparison = (x, y) => x.HasAttachments().CompareTo(y.HasAttachments())
								};

			columns.Add(iconColumn);

            column = new TableColumn<StudyTableItem, string>(
                ColumnReferringPhysician,
                SR.ColumnHeadingReferringPhysician,
                delegate(StudyTableItem entry)
                    {
                        var name = new PersonName(entry.ReferringPhysiciansName ?? "");
                        return name.FormattedName;
                    },
                0.6f);

			columns.Add(column);
			return columns;
		}

		protected static IEnumerable<TableColumnBase<StudyTableItem>> CreateInstanceCountColumns()
		{
			var column = new TableColumn<StudyTableItem, string>(
				ColumnNumberOfInstances,
				SR.ColumnHeadingNumberOfInstances,
				item => item.NumberOfStudyRelatedInstances.HasValue ? item.NumberOfStudyRelatedInstances.ToString() : "",
				null,
				0.3f,
					delegate(StudyTableItem entry1, StudyTableItem entry2)
					{
						int? instances1 = entry1.NumberOfStudyRelatedInstances;
                        int? instances2 = entry2.NumberOfStudyRelatedInstances;

						if (instances1 == null)
						{
							if (instances2 == null)
								return 0;
							return 1;
						}
						if (instances2 == null)
						{
							return -1;
						}

						return -instances1.Value.CompareTo(instances2.Value);
					});

			return new TableColumnBase<StudyTableItem>[] { column };
		}

		protected static IEnumerable<TableColumnBase<StudyTableItem>> CreateServerColumns()
		{
			var columns = new List<TableColumnBase<StudyTableItem>>();
			var column = new TableColumn<StudyTableItem, string>(ColumnServer, SR.ColumnHeadingServer,
                                                             item => (item.Server == null) ? "" : item.Server.ToString(),
														0.3f);

			columns.Add(column);

			column = new TableColumn<StudyTableItem, string>(ColumnAvailability, SR.ColumnHeadingAvailability,
			                                             item => item.InstanceAvailability ?? "",
														0.3f);

			columns.Add(column);
			return columns;
		}

		internal void UpdateColumnVisibility()
		{
			UpdateServerColumnsVisibility();
		}

		private void UpdateServerColumnsVisibility()
		{
			TableColumnBase<StudyTableItem> column = FindColumn(ColumnServer);
			if (column != null)
			{
				if (_isLocalServer || _numberOfChildServers == 1)
					column.Visible = false;
				else
					column.Visible = true;
			}

			column = FindColumn(ColumnAvailability);
			if (column != null)
			{
				if (_isLocalServer)
					column.Visible = false;
				else
					column.Visible = true;
			}
		}

		protected TableColumnBase<StudyTableItem> FindColumn(string columnHeading)
		{
		    return StudyTable.Columns.FirstOrDefault(column => column.Name == columnHeading);
		}

	    protected void OnColumnValueChanged(object sender, ItemEventArgs<StudyTableItem> e)
		{
			this.StudyTable.Items.NotifyItemUpdated(e.Item);
		}

		#endregion

		protected static string FormatDicomDA(string dicomDate)
		{
			DateTime date;
			if (!DateParser.Parse(dicomDate, out date))
				return dicomDate;

			return date.ToString(Format.DateFormat);
		}

		private static IconSet GetAttachmentsIcon(StudyTableItem entry)
		{
			return entry.HasAttachments() ? new IconSet("AttachmentsExtraSmall.png") : null;
		}

		private static string[] SortModalities(IEnumerable<string> modalities)
		{
			var list = new List<string>(modalities);
			list.Remove(@"DOC"); // the DOC modality is a special case and handled via the attachments icon
			list.Sort((x, y) =>
			              {
			                  var result = GetModalityPriority(x).CompareTo(GetModalityPriority(y));
			                  if (result == 0)
			                      result = string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
			                  return result;
			              });
			return list.ToArray();
		}

		private static int GetModalityPriority(string modality)
		{
			const int imageModality = 0; // sort all known image modalities to top
			const int unknownModality = 1; // unknown modalities may be images or may simply be other documents - sort after known images, but before known ancillary documents
			const int srModality = 2;
			const int koModality = 3;
			const int prModality = 4;

			switch (modality)
			{
				case @"SR":
					return srModality;
				case @"KO":
					return koModality;
				case @"PR":
					return prModality;
				default:
					return StandardModalities.Modalities.Contains(modality) ? imageModality : unknownModality;
			}
		}

		public static SearchResultColumnOptionCollection ColumnOptions
		{
			get
			{
				try
				{
					return new SearchResultColumnOptionCollection(DicomExplorerConfigurationSettings.Default.ResultColumns);
				}
				catch (Exception)
				{
					return new SearchResultColumnOptionCollection();
				}
			}
			set
			{
				DicomExplorerConfigurationSettings.Default.ResultColumns = value;
				DicomExplorerConfigurationSettings.Default.Save();
			}
		}
	}
}
