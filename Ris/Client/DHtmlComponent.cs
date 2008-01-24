#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Client.Formatting;
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
            private HtmlActionModelRenderer _renderer;

            public DHtmlScriptCallback(DHtmlComponent component)
            {
                _component = component;
                _renderer = new HtmlActionModelRenderer();
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
                DateTime? dt = DateTimeUtils.ParseISO(isoDateString);
                return dt == null ? "" : Format.Date(dt);
            }

            public string FormatTime(string isoDateString)
            {
                DateTime? dt = DateTimeUtils.ParseISO(isoDateString);
                return dt == null ? "" : Format.Time(dt);
            }

            public string FormatDateTime(string isoDateString)
            {
                DateTime? dt = DateTimeUtils.ParseISO(isoDateString);
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
                CompositeIdentifierDetail detail = JsmlSerializer.Deserialize<CompositeIdentifierDetail>(jsml);
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

            public string ResolveStaffName(string search)
            {
                StaffSummary staff = null;
                StaffLookupHandler lookupHandler = new StaffLookupHandler(_component.Host.DesktopWindow);
                bool resolved = lookupHandler.ResolveName(search, out staff);
                if(!resolved)
                {
                    resolved = lookupHandler.ResolveNameInteractive(search, out staff);
                }
                return resolved ? JsmlSerializer.Serialize(staff, "staff") : null;
            }

            public string GetHealthcareContext()
            {
                return JsmlSerializer.Serialize(_component.GetHealthcareContext(), "context");
            }

            public string GetTag(string tag)
            {
                return _component.GetTag(tag);
            }

            public void SetTag(string tag, string data)
            {
                _component.SetTag(tag, data);
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

        public virtual void SaveData()
        {
            EventsHelper.Fire(_dataSaving, this, EventArgs.Empty);
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
                // Do not assume same url implies page should not be reloaded
                _htmlPageUrl = value;
                NotifyPropertyChanged("HtmlPageUrl");
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

        public virtual bool ScrollBarsEnabled
        {
            get { return true; }
        }

        #endregion

        protected virtual ActionModelNode GetActionModel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual DataContractBase GetHealthcareContext()
        {
            throw new NotSupportedException("Healthcare context not supported by this component.");
        }

        protected virtual string GetTag(string tag)
        {
            throw new NotSupportedException(string.Format("Tag {0} is not supported by this component.", tag));
        }

        protected virtual void SetTag(string tag, string data)
        {
            throw new NotSupportedException(string.Format("Tag {0} is not supported by this component.", tag));
        }

        /// <summary>
        /// Factory method to create script callback.  Override to provide custom implementation.
        /// </summary>
        /// <returns></returns>
        protected virtual DHtmlScriptCallback CreateScriptCallback()
        {
            return new DHtmlScriptCallback(this);
        }

        public void SetUrl(string url)
        {
            this.HtmlPageUrl = url == null ? null : new Uri(url);
        }


    }
}
