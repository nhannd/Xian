using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	/// <summary>
	/// Extension point for views onto <see cref="SearchPanelComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class SearchPanelComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}
	
	/// <summary>
	/// SearchPanelComponent class
	/// </summary>
	[AssociateView(typeof(SearchPanelComponentViewExtensionPoint))]
	public class SearchPanelComponent : ApplicationComponent, INotifyPropertyChanged
	{
		private StudyBrowserComponent _studyBrowserComponent;

		private string _title;

		private string _lastName = "";
		private string _firstName = "";
		private string _patientID = "";
		private string _accessionNumber = "";
		private string _studyDescription = "";

		private bool _useStudyDateFrom = false;
		private bool _useStudyDateTo = false;
		private DateTime _studyDateFrom;
		private DateTime _studyDateTo;

		private List<string> _searchModalities;
		private ICollection<string> _availableModalities;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public SearchPanelComponent(StudyBrowserComponent studyBrowserComponent)
		{
			Platform.CheckForNullReference(studyBrowserComponent, "studyBrowserComponent");
			_studyBrowserComponent = studyBrowserComponent;
			_studyBrowserComponent.SearchPanelComponent = this;

			_searchModalities = new List<string>();
			_availableModalities = StandardModalities.Modalities;

			InternalClearDates();
		}

		public IDesktopWindow DesktopWindow
		{
			get { return this.Host.DesktopWindow; }
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyPropertyChanged("Title");
			}
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set
			{
				_accessionNumber = value;
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		public string PatientID
		{
			get { return _patientID; }
			set
			{
				_patientID = value;
				NotifyPropertyChanged("PatientID");
			}
		}

		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				NotifyPropertyChanged("FirstName");
			}
		}

		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				NotifyPropertyChanged("LastName");
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set
			{
				_studyDescription = value;
				NotifyPropertyChanged("StudyDescription");
			}
		}

		public DateTime? StudyDateFrom
		{
			get
			{
				if (_useStudyDateFrom)
					return _studyDateFrom;
				else
					return null;
			}
			set
			{
				if (value == null || (object)value == System.DBNull.Value)
				{
					_useStudyDateFrom = false;
				}
				else
				{
					_studyDateFrom = this.MinimumDate((DateTime)value, this.MaximumStudyDateFrom);
					_useStudyDateFrom = true;
				}

				NotifyPropertyChanged("StudyDateFrom");
			}
		}

		public DateTime? StudyDateTo
		{
			get
			{
				if (_useStudyDateTo)
					return _studyDateTo;
				else 
					return null;
			}
			set
			{
				if (value == null || (object)value == System.DBNull.Value)
				{
					_useStudyDateTo = false;
				}
				else
				{
					_studyDateTo = MinimumDate((DateTime)value, this.MaximumStudyDateTo);
					_useStudyDateTo = true;
				}

				NotifyPropertyChanged("StudyDateTo");

				//always keep the fromdate less that the todate
				if (this.StudyDateFrom != null)
				{
					//try re-setting the fromdate, it will be automatically changed if it's not valid.
					this.StudyDateFrom = this.StudyDateFrom;
				}
			}
		}

		public DateTime MaximumStudyDateFrom
		{
			get { return this.StudyDateTo ?? DateTime.Today; }
		}

		public DateTime MaximumStudyDateTo
		{
			get { return DateTime.Today; }
		}

		public ICollection<string> AvailableSearchModalities
		{
			get { return _availableModalities; }
		}

		public IList<string> SearchModalities
		{
			get { return _searchModalities; }
			set
			{
				_searchModalities.Clear();

				if (value != null)
					_searchModalities.AddRange(value);

				NotifyPropertyChanged("SearchModalities");
			}
		}

		public override void Start()
		{
			this.Title = SR.TitleSearch;

			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public void Clear()
		{
			this.PatientID = "";
			this.FirstName = "";
			this.LastName = "";
			this.AccessionNumber = "";
			this.StudyDescription = "";
			this.SearchModalities = new List<string>(); //clear the checked modalities.

			InternalClearDates();
		}

		public void Search()
		{
			BlockingOperation.Run(_studyBrowserComponent.Search);
		}
		
		public void SearchToday()
        {
			InternalSearchLastXDays(0);
        }

		public void SearchLastWeek()
		{
			InternalSearchLastXDays(7);
		}

		public void SearchLastXDays(int numberOfDays)
		{
			InternalSearchLastXDays(numberOfDays);
		}

		private void InternalSearchLastXDays(int numberOfDays)
		{
			this.StudyDateTo = DateTime.Today;
			this.StudyDateFrom = DateTime.Today - TimeSpan.FromDays((double)numberOfDays);

			Search();
		}

		private DateTime MinimumDate(params DateTime[] dates)
		{
			DateTime minimumDate = dates[0];
			for (int i = 1; i < dates.Length; ++i)
			{
				if (dates[i] < minimumDate)
					minimumDate = dates[i];
			}

			return minimumDate;
		}

		private void InternalClearDates()
		{
			this.StudyDateTo = DateTime.Today;
			this.StudyDateFrom = DateTime.Today;
			this.StudyDateTo = null;
			this.StudyDateFrom = null;
		}
    }
}
