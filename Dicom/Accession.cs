
namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Accession
    {
        private string _accession;

        public Accession(string accession)
        {
            // validate the input
            if (null == accession)
                throw new System.ArgumentNullException("accession", SR.ExceptionDicomAETitleNull);

            if (0 == accession.Length)
                throw new System.ArgumentOutOfRangeException("accession", SR.ExceptionDicomAETitleZeroLength);

            _accession = accession;
        }

        public override string ToString()
        {
            return _accession;
        }
    }
}
