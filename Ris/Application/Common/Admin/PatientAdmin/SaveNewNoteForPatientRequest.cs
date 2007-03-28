using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class SaveNewNoteForPatientRequest : DataContractBase
    {
        public SaveNewNoteForPatientRequest(EntityRef patientProfileRef, NoteDetail detail)
        {
            this.PatientProfileRef = patientProfileRef;
            this.Note = detail;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public NoteDetail Note;
    }
}
