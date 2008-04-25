using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class OrderNoteboxItem
    {
        private readonly EntityRef _orderNoteRef;
        private readonly EntityRef _orderRef;
        private readonly EntityRef _patientRef;

        private readonly PatientIdentifier _mrn;
        private readonly PersonName _patientName;
        private readonly DateTime? _dateOfBirth;
        private readonly string _accessionNumber;
        private readonly string _diagnosticServiceName;
        private readonly string _category;
        private readonly DateTime? _postTime;
        private readonly Staff _author;
        private readonly bool _isAcknowledged;


        public OrderNoteboxItem(
            OrderNote orderNote,
            Order order,
            Patient patient,
            PatientIdentifier mrn,
            PersonName patientName,
            DateTime? dateOfBirth,
            string accessionNumber,
            string diagnosticServiceName,
            string category,
            DateTime? postTime,
            Staff author,
            bool isAcknowledged)
        {
            this._orderNoteRef = orderNote.GetRef();
            this._orderRef = order.GetRef();
            this._patientRef = patient.GetRef();
            this._mrn = mrn;
            this._patientName = patientName;
            this._dateOfBirth = dateOfBirth;
            this._accessionNumber = accessionNumber;
            this._diagnosticServiceName = diagnosticServiceName;
            this._category = category;
            this._postTime = postTime;
            this._author = author;
            this._isAcknowledged = isAcknowledged;
        }


        /// <summary>
        /// Constructor for sent item.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="order"></param>
        /// <param name="patient"></param>
        /// <param name="patientProfile"></param>
        public OrderNoteboxItem(OrderNote note, Order order, Patient patient, PatientProfile patientProfile)
            :this(note, order, patient, patientProfile, note.IsFullyAcknowledged)
        {
        }


        /// <summary>
        /// Constructor for inbox item.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="order"></param>
        /// <param name="patient"></param>
        /// <param name="patientProfile"></param>
        /// <param name="isAcknowledged"></param>
        public OrderNoteboxItem(OrderNote note, Order order, Patient patient, PatientProfile patientProfile, bool isAcknowledged)
        {
            _orderNoteRef = note.GetRef();
            _orderRef = order.GetRef();
            _patientRef = patient.GetRef();
            _mrn = patientProfile.Mrn;
            _patientName = patientProfile.Name;
            _dateOfBirth = patientProfile.DateOfBirth;
            _accessionNumber = order.AccessionNumber;
            _diagnosticServiceName = order.DiagnosticService.Name;
            _category = note.Category;
            _postTime = note.PostTime;
            _author = note.Author;
            _isAcknowledged = isAcknowledged;
        }

        public EntityRef OrderNoteRef
        {
            get { return _orderNoteRef; }
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
        }

        public EntityRef PatientRef
        {
            get { return _patientRef; }
        }

        public PatientIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }
        }

        public string Category
        {
            get { return _category; }
        }

        public DateTime? PostTime
        {
            get { return _postTime; }
        }

        public Staff Author
        {
            get { return _author; }
        }

        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
        }
    }
}
