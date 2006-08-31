using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent
	{
		private IStudyFinder _studyFinder;
		private IStudyLoader _studyLoader;

		private string _title;

		private string _lastName;
		private string _firstName;
		private string _patientID;
		private string _accessionNumber;

		public IStudyFinder StudyFinder
		{
			get { return _studyFinder; }
			set { _studyFinder = value; }
		}

		public IStudyLoader StudyLoader
		{
			get { return _studyLoader; }
			set { _studyLoader = value; }
		}
		
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value; }
		}


		public string PatientID
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public void Search()
		{
		}

		public void Clear()
		{
		}
	}
}
