using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Encapsulation of the AE Title DICOM tag.
    /// </summary>
    [SerializableAttribute]
    public struct AETitle
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="aeTitle">The AE Title as a string.</param>
        public AETitle(string aeTitle)
        {
            // validate the input
            if (null == aeTitle)
                throw new System.ArgumentNullException("aeTitle", SR.ExceptionDicomAETitleNull);

            if (0 == aeTitle.Length)
                throw new System.ArgumentOutOfRangeException("aeTitle", SR.ExceptionDicomAETitleZeroLength);

            _aeTitle = aeTitle;
        }

        /// <summary>
        /// Retrieves the AE Title.
        /// </summary>
        /// <returns>The AE Title as a string.</returns>
        public override string ToString()
        {
            return _aeTitle;
        }

        public String StringForm
        {
            get { return this.ToString(); }
        }

        private string _aeTitle;
    }
}
