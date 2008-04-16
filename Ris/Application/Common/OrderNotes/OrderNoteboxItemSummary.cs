using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class OrderNoteboxItemSummary : DataContractBase
    {
        /// <summary>
        /// Gets a reference to the order note.
        /// </summary>
        [DataMember]
        public EntityRef OrderNoteRef;

        /// <summary>
        /// Gets a reference to the order.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

        /// <summary>
        /// Gets a reference to the patient.
        /// </summary>
        [DataMember]
        public EntityRef PatientRef;

        /// <summary>
        /// Gets the patient MRN.
        /// </summary>
        [DataMember]
        public CompositeIdentifierDetail Mrn;

        /// <summary>
        /// Gets the patient name.
        /// </summary>
        [DataMember]
        public PersonNameDetail PatientName;

        /// <summary>
        /// Gets the patient date of birth.
        /// </summary>
        [DataMember]
        public DateTime? DateOfBirth;

        /// <summary>
        /// Gets the order accession number.
        /// </summary>
        [DataMember]
        public string AccessionNumber;

        /// <summary>
        /// Gets the diagnostic service name.
        /// </summary>
        [DataMember]
        public string DiagnosticServiceName;

        /// <summary>
        /// Gets the note category.
        /// </summary>
        [DataMember]
        public string Category;
        
        /// <summary>
        /// Gets the time the note was sent.
        /// </summary>
        [DataMember]
        public DateTime? SentTime;

        /// <summary>
        /// Gets the note author.
        /// </summary>
        [DataMember]
        public StaffSummary Author;

        /// <summary>
        /// Gets a value indicating whether the note has been acknowledged.
        /// For "Sent Items", this value is only true if the note has been acknowledged by all recipients.
        /// </summary>
        [DataMember]
        public bool IsAcknowledged;
    }
}
