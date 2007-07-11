using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public abstract class PreviewApplicationComponent : ApplicationComponent
    {
        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private PreviewApplicationComponent _component;
            private ActionModelRenderer _renderer;

            public ScriptCallback(PreviewApplicationComponent component)
            {
                _component = component;
                _renderer = new ActionModelRenderer();
            }

            public string GetPreviewData(string requestJsml)
            {
                return _component.GetPreviewData(requestJsml);
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string GetActionHtml(string labelSearch, string actionLabel)
            {
                return _renderer.GetHTML(_component.ActionModel, labelSearch, actionLabel);
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

            public string FormatAddress(string jsml)
            {
                AddressDetail detail = JsmlSerializer.Deserialize<AddressDetail>(jsml);
                return detail == null ? "" : AddressFormat.Format(detail);
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

            public string FormatTelephone(string jsml)
            {
                TelephoneDetail detail = JsmlSerializer.Deserialize<TelephoneDetail>(jsml);
                return detail == null ? "" : TelephoneFormat.Format(detail);
            }
        }

        private ScriptCallback _scriptCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        public PreviewApplicationComponent()
        {
            _scriptCallback = new ScriptCallback(this);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Public Properties

        public abstract string GetPreviewData(string requestJsml);

        public abstract string DetailsPageUrl { get; }

        public virtual ActionModelNode ActionModel
        {
            get { return null; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        #endregion
    }
}
