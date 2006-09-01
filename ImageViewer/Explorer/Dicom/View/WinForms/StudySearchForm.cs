using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class StudySearchForm : UserControl
	{
		public StudySearchForm()
		{
			InitializeComponent();
		}

		public TextBox PatientId
		{
			get { return _patientID; }
		}

		public TextBox LastName
		{
			get { return _lastName; }
		}

		public TextBox FirstName
		{
			get { return _firstName; }
		}

		public TextBox AccessionNumber
		{
			get { return _accessionNumber; }
		}

		public TextBox StudyDescription
		{
			get { return _studyDescription; }
		}

		public event EventHandler SearchClicked
		{
			add { _searchButton.Click += value; }
			remove { _searchButton.Click -= value; }
		}
	}
}
