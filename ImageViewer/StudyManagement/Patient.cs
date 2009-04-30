#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM patient.
	/// </summary>
	public class Patient
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

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
		{
			get { return _sop.PatientsBirthDate; }
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
			string str = String.Format("{0} | {1}", this.PatientsName, this.PatientId);
			return str;
		}

		internal void SetSop(Sop sop)
		{
			_sop = sop;
		}
	}
}
