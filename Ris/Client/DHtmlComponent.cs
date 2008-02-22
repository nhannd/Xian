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
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class DHtmlComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

    /// <summary>
    /// Base class for components that display an HTML page.
    /// </summary>
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
            private readonly HtmlActionModelRenderer _actionModelRenderer;

            public DHtmlScriptCallback(DHtmlComponent component)
            {
                _component = component;
                _actionModelRenderer = new HtmlActionModelRenderer();
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

            public bool Modified
            {
                get { return _component.Modified; }
                set { _component.Modified = value; }
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
                return _actionModelRenderer.GetHTML(_component.GetActionModel(), labelSearch, actionLabel);
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

        /// <summary>
        /// Gets the value associated with the specified tag.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method retrieves tags from the dictionary returned by the
        /// <see cref="TagData"/> property.
        /// In most cases this method should not be overridden - override the <see cref="TagData"/> property
        /// instead.  The only reason to override this method is to do special processing of a given tag
        /// (for example, to define a special tag that is not stored in the dictionary).
        /// </remarks>
        /// <param name="tag"></param>
        /// <returns></returns>
        protected virtual string GetTag(string tag)
        {
            // if component doesn't support tag data, just do the most lenient thing and return null
            // we could throw an exception, but that seems counter to the spirit of javascript
            if (this.TagData == null)
                return null;

            string value;
            this.TagData.TryGetValue(tag, out value);

            return value;
        }

        /// <summary>
        /// Gets the value associated with the specified tag.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method stores tags in the dictionary returned by
        /// the <see cref="TagData"/> property.
        /// In most cases this method should not be overridden - override the <see cref="TagData"/> property
        /// instead.  The only reason to override this method is to do special processing of a given tag
        /// (for example, to define a special tag that is not stored in the dictionary).
        /// </remarks>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        protected virtual void SetTag(string tag, string data)
        {
            // in this case, throwing an exception is probably warranted because there is no point
            // letting the page believe that it is successfully storing tags when in fact it isn't
            if (this.TagData == null)
                throw new NotSupportedException("This component does not support storage of tags.");

            this.TagData[tag] = data;
        }

        /// <summary>
        /// Gets the dictionary used for default storage of tag data.
        /// </summary>
        /// <remarks>
        /// The default implementations of <see cref="GetTag"/> and <see cref="SetTag"/> use the dictionary
        /// returned by this property to store tag data.  The default implementation of this property
        /// returns an empty dictionary.  Therefore this property must be overridden
        /// to support tag storage.  Alternatively, the <see cref="GetTag"/> and <see cref="SetTag"/> methods
        /// may be overridden directly, but in most cases this is not necessary.
        /// </remarks>
        protected virtual IDictionary<string, string> TagData
        {
            get
            {
                return new Dictionary<string, string>();
            }
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
