#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class DicomExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (DicomExplorerConfigurationComponentViewExtensionPoint))]
	public class DicomExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private SearchResultColumnOptionCollection _resultColumns = new SearchResultColumnOptionCollection();
		private bool _selectDefaultServerOnStartup = false;

		protected SearchResultColumnOptionCollection ResultColumns
		{
			get { return _resultColumns; }
		}

		public bool SelectDefaultServerOnStartup
		{
			get { return _selectDefaultServerOnStartup; }
			set
			{
				if (SelectDefaultServerOnStartup == value)
					return;

				_selectDefaultServerOnStartup = value;
				NotifyPropertyChanged("SelectDefaultServerOnStartup");
				Modified = true;
			}
		}

		public bool ShowPhoneticIdeographicNames
		{
			get { return ShowPhoneticName || ShowIdeographicName; }
			set
			{
				if (ShowPhoneticIdeographicNames == value)
					return;

				ShowPhoneticName = ShowIdeographicName = value;
				Modified = true;
			}
		}

		public bool ShowPhoneticName
		{
			get { return _resultColumns[StudyTable.ColumnNamePhoneticName].Visible; }
			set
			{
				if (ShowPhoneticName == value)
					return;

				_resultColumns[StudyTable.ColumnNamePhoneticName].Visible = value;
				NotifyPropertyChanged("ShowPhoneticName");
				NotifyPropertyChanged("ShowPhoneticIdeographicNames");
				Modified = true;
			}
		}

		public bool ShowIdeographicName
		{
            get { return _resultColumns[StudyTable.ColumnNameIdeographicName].Visible; }
			set
			{
				if (ShowIdeographicName == value)
					return;

                _resultColumns[StudyTable.ColumnNameIdeographicName].Visible = value;
				NotifyPropertyChanged("ShowIdeographicName");
				NotifyPropertyChanged("ShowPhoneticIdeographicNames");
				Modified = true;
			}
		}

		public bool ShowNumberOfImagesInStudy
		{
            get { return _resultColumns[StudyTable.ColumnNameNumberOfInstances].Visible; }
			set
			{
				if (ShowNumberOfImagesInStudy == value)
					return;

                _resultColumns[StudyTable.ColumnNameNumberOfInstances].Visible = value;
				NotifyPropertyChanged("ShowNumberOfImagesInStudy");
				Modified = true;
			}
		}

		public override void Start()
		{
			_selectDefaultServerOnStartup = DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup;
			_resultColumns = new SearchResultColumnOptionCollection(DicomExplorerConfigurationSettings.Default.ResultColumns);
			base.Start();
		}

		public override void Save()
		{
			DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup = SelectDefaultServerOnStartup;
			DicomExplorerConfigurationSettings.Default.ResultColumns = _resultColumns;
			DicomExplorerConfigurationSettings.Default.Save();
		}
	}
}