using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls
{
	public partial class StudySearchForm : UserControl
	{
		public StudySearchForm()
		{
			InitializeComponent();
		}

		public string PatientID
		{
			get { return _patientID.Text; }
			set { _patientID.Text = value; }
		}

		public string LastName
		{
			get { return _lastName.Text; }
			set { _lastName.Text = value; }
		}

		public string FirstName
		{
			get { return _firstName.Text; }
			set { _firstName.Text = value; }
		}

		public string AccessionNumber
		{
			get { return _accessionNumber.Text; }
			set { _accessionNumber.Text = value; }
		}

		public string StudyDescription
		{
			get { return _studyDescription.Text; }
			set { _studyDescription.Text = value; }
		}

		public event EventHandler SearchClickedEvent
		{
			add { _searchButton.Click += value; }
			remove { _searchButton.Click -= value; }
		}
	}
}
