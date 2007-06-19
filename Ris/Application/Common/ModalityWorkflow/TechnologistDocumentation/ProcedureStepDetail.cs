using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    /// <summary>
    /// Dummy MPS/PS object
    /// </summary>
    [DataContract]
    public class ProcedureStepDetail
    {
        public ProcedureStepDetail()
            : this("Scheduled", new DocumentationPageDetail("about:blank"))
        {
        }

        public ProcedureStepDetail(string status, DocumentationPageDetail documentationPage)
            : this(null, status, documentationPage)
        {
        }

        public ProcedureStepDetail(string name, string status, DocumentationPageDetail documentationPage)
        {
            this.Name = name;
            this.Status = status;
            this.DocumentationPage = documentationPage;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Status;

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