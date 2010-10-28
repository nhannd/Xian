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
