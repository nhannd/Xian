namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulates the DICOM Patient ID.
    /// </summary>
    public class PatientId
    {
        /// <summary>
        /// Mandatory NHibernate constructor.
        /// </summary>
        public PatientId()
        {
        }


        public virtual string InternalPatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="patientId">A string representation of the Patient ID.</param>
        public PatientId(string patientId)
        {
            // validate the input
            if (null == patientId)
                throw new System.ArgumentNullException("patientId", SR.ExceptionGeneralPatientsNameNull);

            _patientId = patientId;
        }

        /// <summary>
        /// Gets a string representation of the Patient ID.
        /// </summary>
        /// <returns>String representation of Patient ID.</returns>
        public override string ToString()
        {
            return _patientId;
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

        private string _patientId;
    }
}
