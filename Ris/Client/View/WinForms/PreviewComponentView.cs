using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public abstract class PreviewComponentView : WinFormsView, IApplicationComponentView
    {
        private PreviewApplicationComponent _component;
        private PreviewComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PreviewApplicationComponent) component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PreviewComponentControl(_component);
                    _control.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(NavigatingEventHandler);
                }
                return _control;
            }
        }

        public PreviewApplicationComponent Component
        {
            get { return _component; }
        }

        protected virtual ActionModelNode GetEmbeddedActionModel()
        {
            return _component.ActionModel;
        }

        private void NavigatingEventHandler(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            // default page - allow navigation to proceed
            if (e.Url.OriginalString == "about:blank")
                return;

            if (e.Url.OriginalString.StartsWith("action:"))
            {
                e.Cancel = true;    // cancel the webbrowser navigation

                ActionModelNode embeddedActionModel = GetEmbeddedActionModel();
                if(embeddedActionModel != null)
                {
                    // need to find the action in the model that matches the uri path
                    // TODO clean this up - this is a bit of hack right now
                    ActionPath uriPath = new ActionPath(e.Url.LocalPath, null);
                    foreach(ActionModelNode child in embeddedActionModel.ChildNodes)
                    {
                        if(child.Action.Path.LastSegment.ResourceKey == uriPath.LastSegment.ResourceKey)
                        {
                            ((IClickAction)child.Action).Click();
                            break;
                        }
                    }
                }
            }
        }
    }
}
