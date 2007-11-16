using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services
{
    public class MrnAssembler
    {
        public CompositeIdentifierDetail CreateMrnDetail(PatientIdentifier mrn)
        {
            if (mrn == null)
                return new CompositeIdentifierDetail();

            return new CompositeIdentifierDetail(
                mrn.Id,
                EnumUtils.GetEnumValueInfo(mrn.AssigningAuthority));
        }
    }
}
