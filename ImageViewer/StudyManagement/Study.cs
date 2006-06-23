using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Study
	{
		private Patient _parentPatient;
		private string _studyInstanceUID;
		private SeriesCollection _series = new SeriesCollection();
		private int _referenceCount;

		internal Study(string studyInstanceUID, Patient parentPatient)
		{
			_studyInstanceUID = studyInstanceUID;
			_parentPatient = parentPatient;
		}

		public Patient ParentPatient
		{
			get { return _parentPatient; }
		}

		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
		}

		public SeriesCollection Series
		{
			get { return _series; }
		}

		internal int ReferenceCount
		{
			get { return _referenceCount; }
			set { _referenceCount = value; }
		}

		public override string ToString()
		{
			return this.StudyInstanceUID;
		}
	}
}
