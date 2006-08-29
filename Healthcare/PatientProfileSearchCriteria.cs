using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare
{
    public partial class PatientProfileSearchCriteria : SearchCriteria
    {
        private PatientIdentifierSearchCriteria _identifiers = new PatientIdentifierSearchCriteria();

        /// <summary>
        /// Allows criteria to be specified on patient identifiers related to this patient
        /// </summary>
        public PatientIdentifierSearchCriteria Identifiers
        {
            get { return _identifiers; }
        }

        public override bool IsEmpty
        {
            get { return base.IsEmpty && _identifiers.IsEmpty; }
        }
    }
}
