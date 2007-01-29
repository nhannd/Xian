using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Utility class for comparing a set of <see cref="PatientProfile"/> objects for discrepancies.
    /// </summary>
    public class PatientProfileDiscrepancyTest
    {
        /// <summary>
        /// Returns a value that is a bitmask of <see cref="PatientProfileDiscrepancy"/> values indicating
        /// which discrepancies were found among the specified set of profiles.  Only the discrepancies specified in
        /// <paramref name="testableDiscrepancies"/> will be tested.
        /// </summary>
        /// <param name="profiles">The set of profiles to test</param>
        /// <param name="testableFields">A bitmask indicating the set of discrepancies to test for</param>
        /// <returns>A bitmask indicating the discrepancies found</returns>
        public static PatientProfileDiscrepancy GetDiscrepancies(IEnumerable profiles, PatientProfileDiscrepancy testableDiscrepancies)
        {
            PatientProfileDiscrepancy result = PatientProfileDiscrepancy.None;

            DiscrepancyTester tester = new DiscrepancyTester(profiles, testableDiscrepancies);

            // Healthcard
            result |= tester.TestEqual(PatientProfileDiscrepancy.Healthcard,
                delegate(PatientProfile x, PatientProfile y) { return x.Healthcard.IsEquivalentTo(y.Healthcard); });

            // FamilyName
            result |= tester.TestEqual(PatientProfileDiscrepancy.FamilyName,
                delegate(PatientProfile x, PatientProfile y) { return x.Name.FamilyName.Equals(y.Name.FamilyName, StringComparison.CurrentCultureIgnoreCase); });

            // GivenName
            result |= tester.TestEqual(PatientProfileDiscrepancy.GivenName,
                delegate(PatientProfile x, PatientProfile y) { return x.Name.GivenName.Equals(y.Name.GivenName, StringComparison.CurrentCultureIgnoreCase); });

            // MiddleName
            result |= tester.TestEqual(PatientProfileDiscrepancy.MiddleName,
                delegate(PatientProfile x, PatientProfile y) { return x.Name.MiddleName != null ? x.Name.MiddleName.Equals(y.Name.MiddleName, StringComparison.CurrentCultureIgnoreCase) : y.Name.MiddleName == null; });

            // DateOfBirth
            result |= tester.TestEqual(PatientProfileDiscrepancy.DateOfBirth,
                delegate(PatientProfile x, PatientProfile y) { return x.DateOfBirth == y.DateOfBirth; });

            // Sex
            result |= tester.TestEqual(PatientProfileDiscrepancy.Sex,
                delegate(PatientProfile x, PatientProfile y) { return x.Sex == y.Sex; });

            // HomePhone
            result |= tester.TestEqual(PatientProfileDiscrepancy.HomePhone,
                delegate(PatientProfile x, PatientProfile y)
                {
                    TelephoneNumber tx = x.CurrentHomePhone;
                    TelephoneNumber ty = y.CurrentHomePhone;

                    return (tx == null) ? (ty == null) : tx.IsEquivalentTo(ty);
                });

            // HomeAddress
            result |= tester.TestEqual(PatientProfileDiscrepancy.HomeAddress,
                delegate(PatientProfile x, PatientProfile y)
                {
                    Address tx = x.CurrentHomeAddress;
                    Address ty = y.CurrentHomeAddress;

                    return (tx == null) ? (ty == null) : tx.IsEquivalentTo(ty);
                });

            // WorkPhone
            result |= tester.TestEqual(PatientProfileDiscrepancy.WorkPhone,
               delegate(PatientProfile x, PatientProfile y)
               {
                   TelephoneNumber tx = x.CurrentWorkPhone;
                   TelephoneNumber ty = y.CurrentWorkPhone;

                   return (tx == null) ? (ty == null) : tx.IsEquivalentTo(ty);
               });

            // WorkAddress
            result |= tester.TestEqual(PatientProfileDiscrepancy.WorkAddress,
                delegate(PatientProfile x, PatientProfile y)
                {
                    Address tx = x.CurrentWorkAddress;
                    Address ty = y.CurrentWorkAddress;

                    return (tx == null) ? (ty == null) : tx.IsEquivalentTo(ty);
                });

            return result;
        }

        delegate bool TestEqualDelegate(PatientProfile x, PatientProfile y);

        class DiscrepancyTester
        {
            private IEnumerable _profiles;
            private PatientProfileDiscrepancy _testables;

            public DiscrepancyTester(IEnumerable profiles, PatientProfileDiscrepancy testables)
            {
                _profiles = profiles;
                _testables = testables;
            }

            public PatientProfileDiscrepancy TestEqual(PatientProfileDiscrepancy testable, TestEqualDelegate testCallback)
            {
                if ((_testables & testable) != 0)
                {
                    PatientProfile reference = null;
                    foreach (PatientProfile profile in _profiles)
                    {
                        if (reference != null)
                        {
                            if (!testCallback(profile, reference))
                                return testable;
                        }
                        reference = profile;
                    }
                }
                return PatientProfileDiscrepancy.None;
            }
        }
    }
}
