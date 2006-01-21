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
                throw new System.ArgumentNullException("patientsName", "The patientsName cannot be set to null");

            if (0 == patientsName.Length)
                throw new System.ArgumentOutOfRangeException("patientsName", "The patientsName cannot be zero-length");

            _patientsName = patientsName;
        }

        public override string  ToString()
        {
            return _patientsName;
        }
    }
}
