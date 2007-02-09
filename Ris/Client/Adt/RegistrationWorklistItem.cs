using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistItem
    {
        private EntityRef<PatientProfile> _patientProfile;
        private ISet _queryResults;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private HealthcardNumber _healthcardNumber;
        private DateTime _dateOfBirth;
        private Sex _sex;

        public RegistrationWorklistItem(RegistrationWorklistQueryResult queryResult)
        {
            _queryResults = new HybridSet();

            _mrn = queryResult.Mrn;
            _patientName = queryResult.PatientName;
            _healthcardNumber = queryResult.HealthcardNumber;
            _dateOfBirth = queryResult.DateOfBirth;
            _sex = queryResult.Sex;

            _patientProfile = queryResult.PatientProfile;
            _queryResults.Add(queryResult);
        }

        public void AddQueryResults(RegistrationWorklistQueryResult queryResult)
        {
            _queryResults.Add(queryResult);
        }

        public bool HasStatus(ActivityStatus status)
        {
            foreach (RegistrationWorklistQueryResult queryResult in _queryResults)
            {
                if (queryResult.Status == status)
                    return true;
            }

            return false;
        }

        #region Public Properties

        public EntityRef<PatientProfile> PatientProfile
        {
            get { return _patientProfile; }
        }

        public ISet QueryResults
        {
            get { return _queryResults; }
        }

        public CompositeIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public HealthcardNumber HealthcardNumber
        {
            get { return _healthcardNumber; }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
        }

        public Sex Sex
        {
            get { return _sex; }
        }

        #endregion
    }
}
