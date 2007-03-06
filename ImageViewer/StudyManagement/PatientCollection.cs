using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Patient"/> objects.
	/// </summary>
	public class PatientCollection : ObservableDictionary<string, Patient, PatientEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PatientCollection"/>.
		/// </summary>
		public PatientCollection()
		{

		}
	}
}
