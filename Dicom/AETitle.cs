using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public struct AETitle
    {
        private string _aeTitle;

        public AETitle(string aeTitle)
        {
            // validate the input
            if (null == aeTitle)
                throw new System.ArgumentNullException("aeTitle", SR.ExceptionDicomAETitleNull);

            if (0 == aeTitle.Length)
                throw new System.ArgumentOutOfRangeException("aeTitle", SR.ExceptionDicomAETitleZeroLength);

            _aeTitle = aeTitle;
        }

        public override string ToString()
        {
            return _aeTitle;
        }
    }
}
