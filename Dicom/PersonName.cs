namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulates the DICOM Person's Name.
    /// </summary>
    public class PersonName
    {
        /// <summary>
        /// Constructor for NHibernate.
        /// </summary>
        public PersonName()
        {
        }

        protected virtual string InternalPersonName
        {
            get { return _personsName; }
            set
            {
                _personsName = value;
                BreakApartFirstAndLastName();
            }
        }
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
		/// <param name="personsName">The Person's Name as a string.</param>
        public PersonName(string personsName)
        {
            // validate the input
            if (null == personsName)
				throw new System.ArgumentNullException("personsName", SR.ExceptionGeneralPersonsNameNull);

            _personsName = personsName;
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
        /// Gets the Person's Name as a string.
        /// </summary>
        /// <returns>A string representation of the Person's Name.</returns>
        public override string ToString()
        {
            return _personsName;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(PersonName pn)
        {
            return pn.ToString();
        }

        protected void BreakApartFirstAndLastName()
        {
            // parse out the first and last names
            string[] names = this.InternalPersonName.Split('^');

            _lastName = (names.GetUpperBound(0) >= 0) ? (names[0]) : "";
            _firstName = (names.GetUpperBound(0) >= 1) ? (names[1]) : "";
		}

		#region Private Members

		private string _personsName;
        private string _lastName;
        private string _firstName;

		#endregion
	}
}
