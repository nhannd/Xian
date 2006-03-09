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

            // parse out the first and last names
            string[] names = _patientsName.Split('^');

            _lastName = (names.GetUpperBound(0) >= 0) ? (names[0]) : "";
            _firstName = (names.GetUpperBound(0) >= 1) ? (names[1]) : "";
        }

        public String LastName
        {
            get { return _lastName; }
        }

        public String FirstName
        {
            get { return _firstName; }
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
        private string _lastName;
        private string _firstName;
    }
}
