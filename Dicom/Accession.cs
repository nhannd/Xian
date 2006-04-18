
namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulation of the Accession Number DICOM tag.
    /// </summary>
    public struct Accession
    {

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="accession">The Accession Number as a string.</param>
        public Accession(string accession)
        {
            // validate the input
            if (null == accession)
                throw new System.ArgumentNullException("accession", SR.ExceptionDicomAETitleNull);

            //if (0 == accession.Length)
            //    throw new System.ArgumentOutOfRangeException("accession", SR.ExceptionDicomAETitleZeroLength);

            _accession = accession;
        }

        /// <summary>
        /// Retrieves the Accession Number.
        /// </summary>
        /// <returns>Accession Number as a string.</returns>
        public override string ToString()
        {
            return _accession;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="acc">The Accession object to be casted.</param>
        /// <returns>A String representation of the Accession object.</returns>
        public static implicit operator String(Accession acc)
        {
            return acc.ToString();
        }

        private string _accession;
    }
}
