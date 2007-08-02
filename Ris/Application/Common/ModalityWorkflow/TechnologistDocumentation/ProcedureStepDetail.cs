using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    /// <summary>
    /// Dummy MPS/PS object
    /// </summary>
    [DataContract]
    public class ProcedureStepDetail
    {
        public ProcedureStepDetail()
            : this(new EnumValueInfo("SC", "Scheduled"), new DocumentationPageDetail("about:blank"))
        {
        }

        public ProcedureStepDetail(EnumValueInfo status, DocumentationPageDetail documentationPage)
            : this(null, status, documentationPage)
        {
        }

        public ProcedureStepDetail(string name, EnumValueInfo status, DocumentationPageDetail documentationPage)
        {
            this.Name = name;
            this.Status = status;
            this.DocumentationPage = documentationPage;
            this.Dirty = false;
        }

        [DataMember] 
        public EntityRef EntityRef;

        [DataMember] 
        public bool Dirty;

        [DataMember]
        public string Name;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public DocumentationPageDetail DocumentationPage;

        [DataMember]
        public PerformedProcedureStepDetail PerformedProcedureStep;

        public bool CanDocumentWith(ProcedureStepDetail that)
        {
            if (that == null) return false;

            return this.DocumentationPage.Equals(that.DocumentationPage)
                && this.Status.Equals(that.Status)
                && ((this.PerformedProcedureStep == null && that.PerformedProcedureStep == null)
                    || (this.PerformedProcedureStep != null && this.PerformedProcedureStep.Equals(that.PerformedProcedureStep)));
        }
    }
}