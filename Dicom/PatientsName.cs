namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulates the DICOM Patient's Name.
    /// </summary>
    public struct PatientsName
    {

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="patientsName">The Patient's Name as a string.</param>
        public PatientsName(string patientsName)
        {
            // validate the input
            if (null == patientsName)
                throw new System.ArgumentNullException("patientsName", SR.ExceptionGeneralPatientsNameNull);

            if (0 == patientsName.Length)
                throw new System.ArgumentOutOfRangeException("patientsName", SR.ExceptionGeneralPatientsNameZeroLength);

            _patientsName = patientsName;
        }

        /// <summary>
        /// Gets the Patient's Name as a string.
        /// </summary>
        /// <returns>A string representation of the Patient's Name.</returns>
        public override string  ToString()
        {
            return _patientsName;
        }

        private string _patientsName;
    }
}
