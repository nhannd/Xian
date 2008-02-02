#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class StaffAssembler
    {
        public StaffSummary CreateStaffSummary(Staff staff, IPersistenceContext context)
        {
            if (staff == null)
                return null;

            return new StaffSummary(staff.GetRef(), staff.Id,
                EnumUtils.GetEnumValueInfo(staff.Type, context),
                new PersonNameAssembler().CreatePersonNameDetail(staff.Name));
        }

        public StaffDetail CreateStaffDetail(Staff staff, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();

            return new StaffDetail(staff.Id, EnumUtils.GetEnumValueInfo(staff.Type, context),
                assembler.CreatePersonNameDetail(staff.Name), staff.LicenseNumber, staff.BillingNumber,
                    new Dictionary<string, string>(staff.ExtendedProperties));
        }

        public void UpdateStaff(StaffDetail detail, Staff staff)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();

            staff.Id = detail.StaffId;
            staff.Type = EnumUtils.GetEnumValue<StaffType>(detail.StaffType);
            assembler.UpdatePersonName(detail.Name, staff.Name);
            staff.LicenseNumber = detail.LicenseNumber;
            staff.BillingNumber = detail.BillingNumber;

            // explicitly copy each pair, so that we don't remove any properties that the client may have removed
            foreach (KeyValuePair<string, string> pair in detail.ExtendedProperties)
            {
                staff.ExtendedProperties[pair.Key] = pair.Value;
            }
        }
    }
}
