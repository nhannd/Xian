namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct PatientsName
    {
        private string _patientsName;

        public PatientsName(string patientsName)
        {
            // validate the input
            if (null == patientsName)
                throw new System.ArgumentNullException("patientsName", SR.ExceptionGeneralPatientsNameNull);

            if (0 == patientsName.Length)
                throw new System.ArgumentOutOfRangeException("patientsName", SR.ExceptionGeneralPatientsNameZeroLength);

            _patientsName = patientsName;
        }

        public override string  ToString()
        {
            return _patientsName;
        }
    }
}
