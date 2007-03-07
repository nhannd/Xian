using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Provides data for <see cref="PatientCollection"/> events.
	/// </summary>
	public class PatientEventArgs : CollectionEventArgs<Patient>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PatientEventArgs"/>.
		/// </summary>
		public PatientEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="PatientEventArgs"/> with
		/// a specified <see cref="Patient"/>.
		/// </summary>
		/// <param name="patient"></param>
		public PatientEventArgs(Patient patient)
		{
			base.Item  = patient;
		}

		/// <summary>
		/// Gets the <see cref="Patient"/>.
		/// </summary>
		public Patient Patient { get { return base.Item; } }

	}
}
