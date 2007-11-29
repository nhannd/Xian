#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Encapsulates the DICOM Patient ID.
    /// </summary>
    public class PatientId : IEquatable<PatientId>
    {
		private string _patientId;

        /// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        private PatientId()
        {
        }

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		/// <param name="patientId">A string representation of the Patient ID.</param>
		public PatientId(string patientId)
		{
			SetPatientId(patientId);
		}
		
		/// <summary>
		/// NHibernate Property.
		/// </summary>
		protected virtual string InternalPatientId
        {
            get { return _patientId; }
			set { SetPatientId(value); }
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="pid">The AETitle object to be casted.</param>
        /// <returns>A String representation of the AE Title object.</returns>
        public static implicit operator string(PatientId pid)
        {
            return pid.ToString();
        }

		/// <summary>
		/// Gets a string representation of the Patient ID.
		/// </summary>
		/// <returns>String representation of Patient ID.</returns>
		public override string ToString()
		{
			return _patientId;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PatientId)
				return this.Equals((PatientId) obj);

			return false;
		}

		#region IEquatable<PatientId> Members

		public bool Equals(PatientId other)
		{
			return InternalPatientId == other.InternalPatientId;
		}

		#endregion

		private void SetPatientId(string patientId)
		{
			_patientId = patientId ?? "";
		}

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
