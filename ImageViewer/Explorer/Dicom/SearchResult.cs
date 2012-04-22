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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public partial class SearchResult
	{
		private string _serverGroupName;
		private bool _isLocalServer;
		private int _numberOfChildServers;

	    private readonly StudyTable _studyTable;
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
            _studyTable = new StudyTable();
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
				UpdateColumnVisibility();
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
                UpdateColumnVisibility();
			}
		}

		public StudyTable StudyTable
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

		public void Initialize()
		{
            _studyTable.Initialize();
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

        internal void UpdateColumnVisibility()
        {
            var hide = _isLocalServer || _numberOfChildServers == 1;
            _studyTable.SetServerColumnsVisibility(!hide);

            _studyTable.SetColumnVisibility(StudyTable.ColumnNameDeleteOn, _isLocalServer);
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
