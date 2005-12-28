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
                throw new System.ArgumentNullException("aeTitle", "The AE Title cannot be set to null");

            if (0 == aeTitle.Length)
                throw new System.ArgumentOutOfRangeException("aeTitle", "The AE Title cannot be zero-length");

            _aeTitle = aeTitle;
        }

        public override string ToString()
        {
            return _aeTitle;
        }
    }
}
