using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _profileRef;
        private EntityRef _orderRef;

        public WorklistItemKey(EntityRef profileRef, EntityRef orderRef)
        {
            _profileRef = profileRef;
            _orderRef = orderRef;
        }

        public EntityRef ProfileRef
        {
            get { return _profileRef; }
            set { _profileRef = value; }
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
            set { _orderRef = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        // PatientProfile data
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private HealthcardNumber _healthcardNumber;
        private DateTime? _dateOfBirth;
        private Sex _sex;

        // Order data
        private DateTime? _earliestScheduledTime;

        public WorklistItem(PatientProfile profile)
            : base(new WorklistItemKey(profile.GetRef(), null))
        {
            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _healthcardNumber = profile.Healthcard;
            _dateOfBirth = profile.DateOfBirth;
            _sex = profile.Sex;
        }

        public WorklistItem(Order order)
            : base(new WorklistItemKey(null, order.GetRef()))
        {
            PatientProfile profile = CollectionUtils.SelectFirst<Entity>(order.Patient.Profiles,
                delegate(Entity entity)
                {
                    PatientProfile pp = entity.As<PatientProfile>();
                    if (pp.Mrn.AssigningAuthority == order.Visit.VisitNumber.AssigningAuthority)
                        return true;

                    return false;
                }).Downcast<PatientProfile>();

            WorklistItemKey thisKey = (WorklistItemKey)this.Key;
            thisKey.ProfileRef = profile.GetRef();

            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _healthcardNumber = profile.Healthcard;
            _dateOfBirth = profile.DateOfBirth;
            _sex = profile.Sex;

            _earliestScheduledTime = order.EarliestScheduledDateTime;
        }

        public WorklistItem(EntityRef profileRef,
            EntityRef orderRef,
            CompositeIdentifier mrn,
            PersonName patientName,
            HealthcardNumber healthcard,
            DateTime? dateOfBirth,
            Sex sex,
            DateTime? earliestScheduledTime)
            : base(new WorklistItemKey(profileRef, orderRef))
        {
            _mrn = mrn;
            _patientName = patientName;
            _healthcardNumber = healthcard;
            _dateOfBirth = dateOfBirth;
            _sex = sex;

            _earliestScheduledTime = earliestScheduledTime;
        }

        #region Public Properties

        public EntityRef ProfileRef
        {
            get { return ((WorklistItemKey)this.Key).ProfileRef; }
        }

        public EntityRef OrderRef
        {
            get { return ((WorklistItemKey)this.Key).OrderRef; }
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

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
        }

        public Sex Sex
        {
            get { return _sex; }
        }

        public DateTime? EarliestScheduledTime
        {
            get { return _earliestScheduledTime; }
        }

        #endregion
    }
}
