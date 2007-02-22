using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    public class WorklistItem : IWorklistItem
    {
        private EntityRef<PatientProfile> _patientProfile;
        private string _workClassName;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private HealthcardNumber _healthcardNumber;
        private DateTime _dateOfBirth;
        private Sex _sex;

        public WorklistItem(string workClassName, WorklistQueryResult queryResult)
        {
            _workClassName = workClassName;
            _mrn = queryResult.Mrn;
            _patientName = queryResult.PatientName;
            _healthcardNumber = queryResult.HealthcardNumber;
            _dateOfBirth = queryResult.DateOfBirth;
            _sex = queryResult.Sex;

            _patientProfile = queryResult.PatientProfile;
        }

        public WorklistItem(string workClassName, PatientProfile profile)
        {
            _workClassName = workClassName;

            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _healthcardNumber = profile.Healthcard;
            _dateOfBirth = profile.DateOfBirth;
            _sex = profile.Sex;

            _patientProfile = new EntityRef<PatientProfile>(profile);
        }

        #region Public Properties

        public EntityRef<PatientProfile> PatientProfile
        {
            get { return _patientProfile; }
        }

        public string WorkClassName
        {
            get { return _workClassName; }
            set { _workClassName = value; }
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
