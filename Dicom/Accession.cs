
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

            if (0 == accession.Length)
                throw new System.ArgumentOutOfRangeException("accession", SR.ExceptionDicomAETitleZeroLength);

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

        private string _accession;
    }
}
