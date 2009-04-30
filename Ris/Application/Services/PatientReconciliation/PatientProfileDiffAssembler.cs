#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.PatientReconciliation
{
    public class PatientProfileDiffAssembler
    {
        public PatientProfileDiff CreatePatientProfileDiff(PatientProfile left, PatientProfile right, IList<DiscrepancyTestResult> results)
        {
            PatientProfileDiff diff = new PatientProfileDiff();
            diff.LeftProfileAssigningAuthority = left.Mrn.AssigningAuthority.Code;
            diff.RightProfileAssigningAuthority = right.Mrn.AssigningAuthority.Code;

            diff.DateOfBirth = CreatePropertyDiff(PatientProfileDiscrepancy.DateOfBirth, results);
            diff.FamilyName = CreatePropertyDiff(PatientProfileDiscrepancy.FamilyName, results);
            diff.GivenName = CreatePropertyDiff(PatientProfileDiscrepancy.GivenName, results);
            diff.Healthcard = CreatePropertyDiff(PatientProfileDiscrepancy.Healthcard, results);
            diff.HomeAddress = CreatePropertyDiff(PatientProfileDiscrepancy.HomeAddress, results);
            diff.HomePhone = CreatePropertyDiff(PatientProfileDiscrepancy.HomePhone, results);
            diff.MiddleName = CreatePropertyDiff(PatientProfileDiscrepancy.MiddleName, results);
            diff.Sex = CreatePropertyDiff(PatientProfileDiscrepancy.Sex, results);
            diff.WorkAddress = CreatePropertyDiff(PatientProfileDiscrepancy.WorkAddress, results);
            diff.WorkPhone = CreatePropertyDiff(PatientProfileDiscrepancy.WorkPhone, results);

            return diff;
        }

        private PropertyDiff CreatePropertyDiff(PatientProfileDiscrepancy discrepancy, IList<DiscrepancyTestResult> results)
        {
            DiscrepancyTestResult result = CollectionUtils.SelectFirst<DiscrepancyTestResult>(results,
                delegate(DiscrepancyTestResult r) { return r.Discrepancy == discrepancy; });

            if (result != null)
            {
                PropertyDiff propDiff = new PropertyDiff();
                propDiff.IsDiscrepant = result.IsDiscrepant;
                propDiff.AlignedLeftValue = result.Diff.AlignedLeft;
                propDiff.AlignedRightValue = result.Diff.AlignedRight;
                propDiff.DiffMask = result.Diff.DiffMask;
                return propDiff;
            }
            return null;
        }
    }
}
