using System;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class ProcedurePlanChangedEventArgs : EventArgs
    {
        private readonly ProcedurePlanSummary _procedurePlanSummary;

        public ProcedurePlanChangedEventArgs(ProcedurePlanSummary procedurePlanSummary)
        {
            _procedurePlanSummary = procedurePlanSummary;
        }

        public ProcedurePlanSummary ProcedurePlanSummary
        {
            get { return _procedurePlanSummary; }
        }
    }

    public interface IDocumentationPage
    {
        void Save();
        void Validate();
        event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged;
    }
}