namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct PatientID
    {
        private string _patientID;

        public PatientID(string patientID)
        {
            // validate the input
            if (null == patientID)
                throw new System.ArgumentNullException("patientID", "The patientID cannot be set to null");

            if (0 == patientID.Length)
                throw new System.ArgumentOutOfRangeException("patientID", "The patientID cannot be zero-length");

            _patientID = patientID;
        }

        public override string ToString()
        {
            return _patientID;
        }
    }
}
