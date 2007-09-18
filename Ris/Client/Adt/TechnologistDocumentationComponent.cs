using System;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistDocumentationComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistDocumentationComponentViewExtensionPoint))]
    public class TechnologistDocumentationComponent : ApplicationComponent
    {
        [ComVisible(true)]
        public class ScriptCallback
        {
            private readonly TechnologistDocumentationComponent _component;

            public ScriptCallback(TechnologistDocumentationComponent component)
            {
                this._component = component;
            }

            public string GetData(string tag)
            {
                return _component.GetData(tag);
            }

            public string FormatDate(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.Date(dt);
            }

            public string FormatTime(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.Time(dt);
            }

            public string FormatDateTime(string isoDateString)
            {
                DateTime? dt = JsmlSerializer.ParseIsoDateTime(isoDateString);
                return dt == null ? "" : Format.DateTime(dt);
            }

            public string FormatHealthcard(string jsml)
            {
                HealthcardDetail detail = JsmlSerializer.Deserialize<HealthcardDetail>(jsml);
                return detail == null ? "" : HealthcardFormat.Format(detail);
            }

            public string FormatMrn(string jsml)
            {
                MrnDetail detail = JsmlSerializer.Deserialize<MrnDetail>(jsml);
                return detail == null ? "" : MrnFormat.Format(detail);
            }

            public string FormatPersonName(string jsml)
            {
                PersonNameDetail detail = JsmlSerializer.Deserialize<PersonNameDetail>(jsml);
                return detail == null ? "" : PersonNameFormat.Format(detail);
            }

            public JsmlServiceProxy GetServiceProxy(string serviceContractName)
            {
                return new JsmlServiceProxy(serviceContractName);
            }

            public string GetWorklistItem()
            {
                return JsmlSerializer.Serialize(_component.GetWorklistItem(), "worklistItem");
            }
        }

        #region Private Members

        private readonly ScriptCallback _scriptCallback;
        private readonly ModalityWorklistItem _worklistItem;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _scriptCallback = new ScriptCallback(this);

            _worklistItem = item;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        public string OrderSummaryUrl
        {
            get { return TechnologistDocumentationComponentSettings.Default.OrderSummaryUrl; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public string GetData(string tag)
        {
            return _worklistItem.AccessionNumber;
        }

        public object GetWorklistItem()
        {
            return _worklistItem;
        }
    }
}
