using System;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Desktop;

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
        string Title { get; }
        IApplicationComponent Component { get; }
    }
}