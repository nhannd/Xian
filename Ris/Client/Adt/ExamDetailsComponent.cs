using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class ExamDetailsComponent : DHtmlComponent
    {
        private ProcedurePlanSummary _procedurePlanSummary;
        private HtmlFormSelector _detailsFormSelector;

        public ExamDetailsComponent(string urlSelectorScript, ProcedurePlanSummary procedurePlanSummary)
        {
            _procedurePlanSummary = procedurePlanSummary;

            _detailsFormSelector = new HtmlFormSelector(urlSelectorScript, new string[] {"procedurePlan"});
            SetUrl(_detailsFormSelector.SelectForm(_procedurePlanSummary));
        }
    }
}
