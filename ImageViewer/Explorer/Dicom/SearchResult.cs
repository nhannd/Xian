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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public class SearchResult
	{
		private string _serverGroupName;
		private bool _isLocalDataStore;
		private int _numberOfChildServers;

		private readonly Table<StudyItem> _studyTable;
		private readonly List<StudyItem> _hiddenItems;

		private bool _filterDuplicates;
		private bool _everSearched;

		public SearchResult()
		{
			_everSearched = false;
			HasDuplicates = false;

			_serverGroupName = "";
			_isLocalDataStore = false;
			_numberOfChildServers = 1;

			_hiddenItems = new List<StudyItem>();
			_studyTable = new Table<StudyItem>();
		}

		#region Properties

		public string ServerGroupName
		{
			get { return _serverGroupName; }
			set { _serverGroupName = value; }
		}

		public bool IsLocalDataStore
		{
			get { return _isLocalDataStore; }
			set
			{
				_isLocalDataStore = value;
				UpdateServerColumnsVisibility();
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

		public Table<StudyItem> StudyTable
		{
			get { return _studyTable; }
		}

		public string ResultsTitle
		{
			get
			{
				if (!_everSearched)
					return _serverGroupName;
				else
					return String.Format(SR.FormatStudiesFound, _studyTable.Items.Count, _serverGroupName);
			}
		}

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
			}
		}
		#endregion

		#region Methods

		public void Initialize()
		{
			InitializeTable();
		}

		public void Refresh(StudyItemList studies, bool filterDuplicates)
		{
			_everSearched = true;
			_filterDuplicates = filterDuplicates;

			_hiddenItems.Clear();
			IList<StudyItem> filteredStudies = new List<StudyItem>(studies);
			RemoveDuplicates(filteredStudies, _hiddenItems);
			HasDuplicates = _hiddenItems.Count > 0;

			if (!_filterDuplicates)
			{
				_hiddenItems.Clear();
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(studies);
			}
			else
			{
				_studyTable.Items.Clear();
				_studyTable.Items.AddRange(filteredStudies);
			}

			StudyTable.Sort();
		}

		private static void RemoveDuplicates(IList<StudyItem> allStudies, List<StudyItem> removed)
		{
			removed.Clear();

			Dictionary<string, StudyItem> uniqueStudies = new Dictionary<string, StudyItem>();
			foreach (StudyItem study in allStudies)
			{
				StudyItem existing;
				if (uniqueStudies.TryGetValue(study.StudyInstanceUid, out existing))
				{
					ApplicationEntity server = study.Server as ApplicationEntity;
					//we will only replace an existing entry if this study's server is streaming.
					if (server != null && server.IsStreaming)
					{
						//only replace existing entry if it is on a non-streaming server.
						server = existing.Server as ApplicationEntity;
						if (server == null || !server.IsStreaming)
						{
							removed.Add(existing);
							uniqueStudies[study.StudyInstanceUid] = study;
							continue;
						}
					}

					//this study is a duplicate.
					removed.Add(study);
				}
				else
				{
					uniqueStudies[study.StudyInstanceUid] = study;
				}
			}

			foreach (StudyItem study in removed)
				allStudies.Remove(study);
		}

		protected virtual void InitializeTable()
		{
			_studyTable.Columns.AddRange(CreateExtensionColumns());
			_studyTable.Columns.AddRange(CreateDefaultColumns());
			_studyTable.Columns.AddRange(CreateInstanceCountColumns());
			_studyTable.Columns.AddRange(CreateServerColumns());

			// Default: Sort by last name
			var column = FindColumn(SR.ColumnHeadingLastName);
			_studyTable.Sort(new TableSortParams(column, true));
		}

		protected IEnumerable<TableColumnBase<StudyItem>> CreateExtensionColumns()
		{
			var columns = new List<TableColumnBase<StudyItem>>();
			try
			{
				// Create and add any extension columns
				StudyColumnExtensionPoint xp = new StudyColumnExtensionPoint();
				foreach (object obj in xp.CreateExtensions())
				{
					IStudyColumn newColumn = (IStudyColumn)obj;

					var column = new TableColumn<StudyItem, string>(
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

		protected static IEnumerable<TableColumnBase<StudyItem>> CreateDefaultColumns()
		{
			var columns = new List<TableColumnBase<StudyItem>>();
			TableColumn<StudyItem, string> column;

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingPatientId,
				delegate(StudyItem item) { return item.PatientId; },
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingLastName,
				delegate(StudyItem item) { return item.PatientsName.LastName; },
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingFirstName,
				delegate(StudyItem item) { return item.PatientsName.FirstName; },
				0.5f);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingIdeographicName,
				delegate(StudyItem item) { return item.PatientsName.Ideographic; },
				0.5f) { Visible = false };

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingPhoneticName,
				delegate(StudyItem item) { return item.PatientsName.Phonetic; },
				0.5f) { Visible = false };

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingDateOfBirth,
				delegate(StudyItem item) { return FormatDicomDA(item.PatientsBirthDate); },
				null,
				0.4F,
				delegate(StudyItem one, StudyItem two) { return one.PatientsBirthDate.CompareTo(two.PatientsBirthDate); });

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingAccessionNumber,
				delegate(StudyItem item) { return item.AccessionNumber; },
				0.45F);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingStudyDate,
				delegate(StudyItem item) { return FormatDicomDA(item.StudyDate); },
				null,
				0.4F,
				delegate(StudyItem one, StudyItem two) { return one.StudyDate.CompareTo(two.StudyDate); });

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingStudyDescription,
				delegate(StudyItem item) { return item.StudyDescription; },
				0.75F);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingModality,
				delegate(StudyItem item) { return DicomStringHelper.GetDicomStringArray(SortModalities(item.ModalitiesInStudy)); },
				0.25f);

			columns.Add(column);

			var iconColumn = new TableColumn<StudyItem, IconSet>(
				SR.ColumnHeadingAttachments,
				GetAttachmentsIcon,
				0.25f)
								{
									ResourceResolver = new ApplicationThemeResourceResolver(typeof(SearchResult).Assembly),
									Comparison = (x, y) => x.HasAttachments().CompareTo(y.HasAttachments())
								};

			columns.Add(iconColumn);

			column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingReferringPhysician,
				delegate(StudyItem item)
				{
					if (item.ReferringPhysiciansName != null)
						return item.ReferringPhysiciansName.FormattedName;
					else
						return "";
				},
				0.6f);

			columns.Add(column);
			return columns;
		}

		protected static IEnumerable<TableColumnBase<StudyItem>> CreateInstanceCountColumns()
		{
			var column = new TableColumn<StudyItem, string>(
				SR.ColumnHeadingNumberOfInstances,
				delegate(StudyItem item)
				{
					if (item.NumberOfStudyRelatedInstances.HasValue)
						return item.NumberOfStudyRelatedInstances.ToString();
					else
						return "";
				},
				null,
				0.3f,
					delegate(StudyItem study1, StudyItem study2)
					{
						int? instances1 = study1.NumberOfStudyRelatedInstances;
						int? instances2 = study2.NumberOfStudyRelatedInstances;

						if (instances1 == null)
						{
							if (instances2 == null)
								return 0;
							else
								return 1;
						}
						else if (instances2 == null)
						{
							return -1;
						}

						return -instances1.Value.CompareTo(instances2.Value);
					});

			return new TableColumnBase<StudyItem>[] { column };
		}

		protected static IEnumerable<TableColumnBase<StudyItem>> CreateServerColumns()
		{
			var columns = new List<TableColumnBase<StudyItem>>();
			var column = new TableColumn<StudyItem, string>(SR.ColumnHeadingServer,
														delegate(StudyItem item)
														{
															return (item.Server == null) ? "" : item.Server.ToString();
														},
														0.3f);

			columns.Add(column);

			column = new TableColumn<StudyItem, string>(SR.ColumnHeadingAvailability,
														delegate(StudyItem item)
														{
															return item.InstanceAvailability ?? "";
														},
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
			TableColumnBase<StudyItem> column = FindColumn(SR.ColumnHeadingServer);
			if (column != null)
			{
				if (_isLocalDataStore || _numberOfChildServers == 1)
					column.Visible = false;
				else
					column.Visible = true;
			}

			column = FindColumn(SR.ColumnHeadingAvailability);
			if (column != null)
			{
				if (_isLocalDataStore)
					column.Visible = false;
				else
					column.Visible = true;
			}
		}

		protected TableColumnBase<StudyItem> FindColumn(string columnHeading)
		{
			foreach (TableColumnBase<StudyItem> column in StudyTable.Columns)
			{
				if (column.Name == columnHeading)
					return column;
			}

			return null;
		}

		protected void OnColumnValueChanged(object sender, ItemEventArgs<StudyItem> e)
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

		private static IconSet GetAttachmentsIcon(StudyItem item)
		{
			return item.HasAttachments() ? new IconSet("AttachmentsExtraSmall.png") : null;
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
