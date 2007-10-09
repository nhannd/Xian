using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class ExamDetailsComponent : DHtmlComponent, IDocumentationPage
    {
        private ProcedurePlanSummary _procedurePlanSummary;
        private HtmlFormSelector _detailsFormSelector;
        private string _title;
        private Dictionary<string, string> _orderExtendedProperties;

        public ExamDetailsComponent(string title, string urlSelectorScript, ProcedurePlanSummary procedurePlanSummary, Dictionary<string, string> orderExtendedProperties)
        {
            _title = title;
            _procedurePlanSummary = procedurePlanSummary;
            _orderExtendedProperties = orderExtendedProperties;

            _detailsFormSelector = new HtmlFormSelector(urlSelectorScript, new string[] {"procedurePlan"});
            SetUrl(_detailsFormSelector.SelectForm(_procedurePlanSummary));
        }

        protected override string GetTagData(string tag)
        {
            string value;
            _orderExtendedProperties.TryGetValue(tag, out value);

            return value;
        }

        protected override void SetTagData(string tag, string data)
        {
            _orderExtendedProperties[tag] = data;
        }

        #region IDocumentationPage Members

        string IDocumentationPage.Title
        {
            get { return _title; }
        }

        ClearCanvas.Desktop.IApplicationComponent IDocumentationPage.Component
        {
            get { return this; }
        }

        #endregion
    }
}
