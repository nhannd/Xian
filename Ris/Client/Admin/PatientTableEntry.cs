using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// 
    /// </summary>
    public class PatientTableEntry
    {
        private Patient _patient;

        public PatientTableEntry(Patient patient)
        {
            _patient = patient;
        }

        public Patient Patient
        {
            get { return _patient; }
        }

        [TableColumn("MRN")]
        public string Mrn
        {
            get { return _patient.GetMrn().Id; }
        }

        [TableColumn("Name")]
        public string Name
        {
            get { return _patient.Name.Format(); }
        }
    }
}
