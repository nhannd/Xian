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

		/// <summary>
		/// Constructor
		/// </summary>
		public SearchPanelComponent(StudyBrowserComponent studyBrowserComponent)
		{
			Platform.CheckForNullReference(studyBrowserComponent, "studyBrowserComponent");
			_studyBrowserComponent = studyBrowserComponent;
			_studyBrowserComponent.SearchPanelComponent = this;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
		
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set
			{
				_accessionNumber = value;
				OnPropertyChanged("AccessionNumber");
			}
		}

		public string PatientID
		{
			get { return _patientID; }
			set
			{
				_patientID = value;
				OnPropertyChanged("PatientID");
			}
		}

		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				OnPropertyChanged("FirstName");
			}
		}

		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				OnPropertyChanged("LastName");
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set
			{
				_studyDescription = value;
				OnPropertyChanged("StudyDescription");
			}
		}

		public override void Start()
		{
			this.Title = "Search";

			base.Start();
		}

		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
			base.Stop();
		}

		public void Search()
		{
			_studyBrowserComponent.Search();
		}

		public void Clear()
		{
			this.PatientID = "";
			this.FirstName = "";
			this.LastName = "";
			this.AccessionNumber = "";
			this.StudyDescription = "";
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(
				  this,
				  new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
