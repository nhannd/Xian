namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Wrapper class for UIDs used in the framework.
    /// </summary>
    public struct Uid
    {
        private string _uid;
        public Uid(string constructionUid)
        {
            // validate the input
            if (null == constructionUid)
                throw new System.ArgumentNullException("constructionUid", SR.ExceptionGeneralUidNull);

            if (0 == constructionUid.Length)
                throw new System.ArgumentOutOfRangeException("constructionUid", SR.ExceptionGeneralUidZeroLength);

            _uid = constructionUid;
        }

        public override string ToString()
        {
            return _uid;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="aet">The AETitle object to be casted.</param>
        /// <returns>A String representation of the AE Title object.</returns>
        public static implicit operator String(Uid uid)
        {
            return uid.ToString();
        }
    }
}
