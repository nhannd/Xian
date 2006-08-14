namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulates the DICOM Patient's Name.
    /// </summary>
    public class PatientsName
    {
        /// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        public PatientsName()
        {
        }

        protected virtual string InternalPatientsName
        {
            get { return _patientsName; }
            set
            {
                _patientsName = value;
                BreakApartFirstAndLastName();
            }
        }
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="patientsName">The Patient's Name as a string.</param>
        public PatientsName(string patientsName)
        {
            // validate the input
            if (null == patientsName)
                throw new System.ArgumentNullException("patientsName", SR.ExceptionGeneralPatientsNameNull);

            //if (0 == patientsName.Length)
            //    throw new System.ArgumentOutOfRangeException("patientsName", SR.ExceptionGeneralPatientsNameZeroLength);

            _patientsName = patientsName;
            BreakApartFirstAndLastName();
        }

        public virtual String LastName
        {
            get { return _lastName; }
        }

        public virtual String FirstName
        {
            get { return _firstName; }
        }

        /// <summary>
        /// Gets the Patient's Name as a string.
        /// </summary>
        /// <returns>A string representation of the Patient's Name.</returns>
        public override string ToString()
        {
            return _patientsName;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="pn">The AETitle object to be casted.</param>
        /// <returns>A String representation of the AE Title object.</returns>
        public static implicit operator String(PatientsName pn)
        {
            return pn.ToString();
        }

        protected void BreakApartFirstAndLastName()
        {
            // parse out the first and last names
            string[] names = this.InternalPatientsName.Split('^');

            _lastName = (names.GetUpperBound(0) >= 0) ? (names[0]) : "";
            _firstName = (names.GetUpperBound(0) >= 1) ? (names[1]) : "";
        }

        private string _patientsName;
        private string _lastName;
        private string _firstName;
    }
}
