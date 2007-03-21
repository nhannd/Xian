using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    /// <summary>
    /// Represents the result of evaluating a single discrepancy test on a pair of patient profiles
    /// </summary>
    public class DiscrepancyTestResult
    {
        private PatientProfileDiscrepancy _discrepancy;
        private bool _discrepant;
        private StringDiff _diff;

        public DiscrepancyTestResult(PatientProfileDiscrepancy discrepancy, bool discrepant, StringDiff diff)
        {
            _discrepancy = discrepancy;
            _discrepant = discrepant;
            _diff = diff;
        }

        public PatientProfileDiscrepancy Discrepancy
        {
            get { return _discrepancy; }
        }

        public bool IsDiscrepant
        {
            get { return _discrepant; }
        }

        public StringDiff Diff
        {
            get { return _diff; }
        }
    }
    
    /// <summary>
    /// Utility class for comparing a set of <see cref="PatientProfile"/> objects for discrepancies.
    /// </summary>
    public class PatientProfileDiscrepancyTest
    {
        delegate bool DiscrepancyTestDelegate(PatientProfile x, PatientProfile y);


        /// <summary>
        /// Returns a value that is a bitmask of <see cref="PatientProfileDiscrepancy"/> values indicating
        /// which discrepancies were found among the specified set of profiles.  Only the discrepancies specified in
        /// <paramref name="testableDiscrepancies"/> will be tested.
        /// </summary>
        /// <param name="profiles">The set of profiles to test</param>
        /// <param name="testableFields">A bitmask indicating the set of discrepancies to test for</param>
        /// <returns>A bitmask indicating the discrepancies found</returns>
        public static IList<DiscrepancyTestResult> GetDiscrepancies(PatientProfile x, PatientProfile y, PatientProfileDiscrepancy testableDiscrepancies)
        {
            List<DiscrepancyTestResult> results = new List<DiscrepancyTestResult>();

            // Healthcard
            if ((testableDiscrepancies & PatientProfileDiscrepancy.Healthcard) == PatientProfileDiscrepancy.Healthcard)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.Healthcard,
                    !x.Healthcard.IsEquivalentTo(y.Healthcard),
                    StringDiff.Compute(x.Healthcard.ToString(), y.Healthcard.ToString())));
            }

            // FamilyName
            if ((testableDiscrepancies & PatientProfileDiscrepancy.FamilyName) == PatientProfileDiscrepancy.FamilyName)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.FamilyName,
                    !x.Name.FamilyName.Equals(y.Name.FamilyName, StringComparison.CurrentCultureIgnoreCase),
                    StringDiff.Compute(x.Name.FamilyName.ToString(), y.Name.FamilyName.ToString())));
            }

            // GivenName
            if ((testableDiscrepancies & PatientProfileDiscrepancy.GivenName) == PatientProfileDiscrepancy.GivenName)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.GivenName,
                    !x.Name.GivenName.Equals(y.Name.GivenName, StringComparison.CurrentCultureIgnoreCase),
                    StringDiff.Compute(x.Name.GivenName.ToString(), y.Name.GivenName.ToString())));
            }

            // MiddleName
            if ((testableDiscrepancies & PatientProfileDiscrepancy.MiddleName) == PatientProfileDiscrepancy.MiddleName)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.MiddleName,
                    !(x.Name.MiddleName != null ? x.Name.MiddleName.Equals(y.Name.MiddleName, StringComparison.CurrentCultureIgnoreCase) : y.Name.MiddleName == null),
                    StringDiff.Compute(x.Name.MiddleName.ToString(), y.Name.MiddleName.ToString())));
            }

            // DateOfBirth
            if ((testableDiscrepancies & PatientProfileDiscrepancy.DateOfBirth) == PatientProfileDiscrepancy.DateOfBirth)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.DateOfBirth,
                    !(x.DateOfBirth == y.DateOfBirth),
                    StringDiff.Compute(x.DateOfBirth.ToShortDateString(), y.DateOfBirth.ToShortDateString())));
            }

            // Sex
            if ((testableDiscrepancies & PatientProfileDiscrepancy.Sex) == PatientProfileDiscrepancy.Sex)
            {
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.Sex,
                    !(x.Sex == y.Sex),
                    StringDiff.Compute(x.Sex.ToString(), y.Sex.ToString())));
            }

            // HomePhone
            if ((testableDiscrepancies & PatientProfileDiscrepancy.HomePhone) == PatientProfileDiscrepancy.HomePhone)
            {
                TelephoneNumber tx = x.CurrentHomePhone;
                TelephoneNumber ty = y.CurrentHomePhone;
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.HomePhone,
                    !((tx == null) ? (ty == null) : tx.IsEquivalentTo(ty)),
                    StringDiff.Compute(tx.ToString(), ty.ToString())));
            }

            // HomeAddress
            if ((testableDiscrepancies & PatientProfileDiscrepancy.HomeAddress) == PatientProfileDiscrepancy.HomeAddress)
            {
                Address tx = x.CurrentHomeAddress;
                Address ty = y.CurrentHomeAddress;
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.HomeAddress,
                    !((tx == null) ? (ty == null) : tx.IsEquivalentTo(ty)),
                    StringDiff.Compute(tx.ToString(), ty.ToString())));
            }

            // WorkPhone
            if ((testableDiscrepancies & PatientProfileDiscrepancy.WorkPhone) == PatientProfileDiscrepancy.WorkPhone)
            {
                TelephoneNumber tx = x.CurrentWorkPhone;
                TelephoneNumber ty = y.CurrentWorkPhone;
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.WorkPhone,
                    !((tx == null) ? (ty == null) : tx.IsEquivalentTo(ty)),
                    StringDiff.Compute(tx.ToString(), ty.ToString())));
            }

            // WorkAddress
            if ((testableDiscrepancies & PatientProfileDiscrepancy.WorkAddress) == PatientProfileDiscrepancy.WorkAddress)
            {
                Address tx = x.CurrentWorkAddress;
                Address ty = y.CurrentWorkAddress;
                results.Add(new DiscrepancyTestResult(PatientProfileDiscrepancy.WorkAddress,
                    !((tx == null) ? (ty == null) : tx.IsEquivalentTo(ty)),
                    StringDiff.Compute(tx.ToString(), ty.ToString())));
            }

            return results;
        }
    }
}
