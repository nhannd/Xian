namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulates the DICOM Patient ID.
    /// </summary>
    public struct PatientId
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="patientId">A string representation of the Patient ID.</param>
        public PatientId(string patientId)
        {
            // validate the input
            if (null == patientId)
                throw new System.ArgumentNullException("patientId", SR.ExceptionGeneralPatientsNameNull);

            if (0 == patientId.Length)
                throw new System.ArgumentOutOfRangeException("patientId", SR.ExceptionGeneralPatientsNameZeroLength);

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

        private string _patientId;
    }
}
