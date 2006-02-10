namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct PatientId
    {
        private string _patientId;

        public PatientId(string patientId)
        {
            // validate the input
            if (null == patientId)
                throw new System.ArgumentNullException("patientId", SR.ExceptionGeneralPatientsNameNull);

            if (0 == patientId.Length)
                throw new System.ArgumentOutOfRangeException("patientId", SR.ExceptionGeneralPatientsNameZeroLength);

            _patientId = patientId;
        }

        public override string ToString()
        {
            return _patientId;
        }
    }
}
