#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM patient.
	/// </summary>
	public class Patient : IPatientData
	{
		private Sop _sop;
		private StudyCollection _studies;

		internal Patient()
		{
		}

		/// <summary>
		/// Gets the collection of <see cref="Study"/> objects that belong
		/// to this <see cref="Patient"/>.
		/// </summary>
		public StudyCollection Studies
		{
			get
			{
				if (_studies == null)
					_studies = new StudyCollection();

				return _studies;
			}
		}
		#region Patient Module

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _sop.PatientId; }
		}

		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public PersonName PatientsName
		{
			get { return _sop.PatientsName; }
		}

		string IPatientData.PatientsName
		{
			get { return _sop.PatientsName; }
		}

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
		{
			get { return _sop.PatientsBirthDate; }
		}

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthTime
		{
			get { return _sop.PatientsBirthTime; }
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public string PatientsSex
		{
			get { return _sop.PatientsSex; }
		}

		#endregion

		/// <summary>
		/// Returns the patient's name and patient ID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1}", this.PatientsName, this.PatientId);
		}

		internal void SetSop(Sop sop)
		{
			if (sop == null)
				_sop = null;
			else if (_sop == null)
				_sop = sop;
		}
	}
}
