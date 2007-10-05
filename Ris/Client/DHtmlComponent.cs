using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class DHtmlComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

    [AssociateView(typeof(DHtmlComponentViewExtensionPoint))]
    public class DHtmlComponent : ApplicationComponent
    {

        #region DHTML script callback class

        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.
        /// </summary>
        [ComVisible(true)]  // must be COM-visible
        public class DHtmlScriptCallback
        {
            protected DHtmlComponent _component;
            private ActionModelRenderer _renderer;

            public DHtmlScriptCallback(DHtmlComponent component)
            {
                _component = component;
                _renderer = new ActionModelRenderer();
            }

            /// <summary>
            /// Surrogate for the browser's window.alert method.
            /// </summary>
            /// <param name="message"></param>
            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            /// <summary>
            /// Surrogate for the browser's window.confirm method.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool Confirm(string message, string type)
            {
                if (string.IsNullOrEmpty(type))
                    type = "okcancel";
                type = type.ToLower();

                if (type == MessageBoxActions.OkCancel.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.OkCancel) == DialogBoxAction.Ok;
                }
                else if (type == MessageBoxActions.YesNo.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.Yes;
                }
                else
                {
                    throw new NotSupportedException("Type must be YesNo or OkCancel");
                }
            }

            public string DateFormat
            {
                get { return Format.DateFormat; }
            }

            public string TimeFormat
            {
                get { return Format.TimeFormat; }
            }

            public string DateTimeFormat
            {
                get { return Format.DateTimeFormat; }
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

            public JsmlServiceProxy GetServiceProxy(string serviceContractName)
            {
                return new JsmlServiceProxy(serviceContractName);
            }

            public string GetActionHtml(string labelSearch, string actionLabel)
            {
                return _renderer.GetHTML(_component.GetActionModel(), labelSearch, actionLabel);
            }

            public string GetWorklistItem()
            {
                return JsmlSerializer.Serialize(_component.GetWorklistItem(), "worklistItem");
            }

            public string GetData(string tag)
            {
                return _component.GetTagData(tag);
            }

            public void SetData(string tag, string data)
            {
                _component.SetTagData(tag, data);
            }

            public string ResolveStaffName(string search)
            {
                StaffSummary staff = null;
                if (StaffFinder.ResolveNameInteractive(search, _component.Host.DesktopWindow, out staff))
                {
                    return JsmlSerializer.Serialize(staff, "staff");
                }
                return null;
            }
        }

        #endregion


        private DHtmlScriptCallback _scriptCallback;
        private Uri _htmlPageUrl;
        private event EventHandler _dataSaving;

        /// <summary>
        /// Constructor
        /// </summary>
        public DHtmlComponent()
        {
        }

        public DHtmlComponent(string url)
        {
            SetUrl(url);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region PresentationModel

        public Uri HtmlPageUrl
        {
            get { return _htmlPageUrl; }
            protected set
            {
                if (!object.Equals(_htmlPageUrl, value))
                {
                    _htmlPageUrl = value;
                    NotifyPropertyChanged("HtmlPageUrl");
                }
            }
        }

        public DHtmlScriptCallback ScriptObject
        {
            get
            {
                if (_scriptCallback == null)
                {
                    _scriptCallback = CreateScriptCallback();
                }
                return _scriptCallback;
            }
        }

        public void InvokeAction(string path)
        {
            ActionModelNode embeddedActionModel = GetActionModel();
            if (embeddedActionModel != null)
            {
                // need to find the action in the model that matches the uri path
                // TODO clean this up - this is a bit of hack right now
                ActionPath uriPath = new ActionPath(path, null);
                foreach (ActionModelNode child in embeddedActionModel.ChildNodes)
                {
                    if (child.Action.Path.LastSegment.ResourceKey == uriPath.LastSegment.ResourceKey)
                    {
                        ((IClickAction)child.Action).Click();
                        break;
                    }
                }
            }
        }

        public event EventHandler DataSaving
        {
            add { _dataSaving += value; }
            remove { _dataSaving -= value; }
        }

        #endregion

        protected virtual ActionModelNode GetActionModel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual object GetWorklistItem()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual string GetTagData(string tag)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void SetTagData(string tag, string data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Factory method to create script callback.  Override to provide custom implementation.
        /// </summary>
        /// <returns></returns>
        protected virtual DHtmlScriptCallback CreateScriptCallback()
        {
            return new DHtmlScriptCallback(this);
        }

        protected void SetUrl(string url)
        {
            this.HtmlPageUrl = url == null ? null : new Uri(url);
        }

    }
}
