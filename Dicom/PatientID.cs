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
        public PatientId()
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
		/// NHibernate Property
		/// </summary>
		public virtual string InternalPatientId
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
    }
}
