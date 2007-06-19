using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Wraps a domain object for presentation and selection in a table
    /// </summary>
    public class TechnologistDocumentationTableItem
    {
        private bool _selected = false;
        private bool _canSelect = true;
        private readonly ProcedureStepDetail _procedureStep;

        public TechnologistDocumentationTableItem()
            :this(new ProcedureStepDetail())
        {                  
        }

        public TechnologistDocumentationTableItem (ProcedureStepDetail procedureStepDocumentationItem)
        {
            _procedureStep = procedureStepDocumentationItem ?? new ProcedureStepDetail();
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public bool CanSelect
        {
            get { return _canSelect; }
            set { _canSelect = value; }
        }

        public ProcedureStepDetail ProcedureStep
        {
            get { return _procedureStep; }
        }
    }
}