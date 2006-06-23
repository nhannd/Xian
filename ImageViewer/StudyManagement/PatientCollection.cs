using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class PatientCollection : ObservableDictionary<string, Patient, PatientEventArgs>
	{
		public PatientCollection()
		{

		}
	}
}
