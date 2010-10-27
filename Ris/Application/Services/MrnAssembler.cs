#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
