using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="RegistrationPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(RegistrationPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponentView : WinFormsView, IApplicationComponentView
    {
        private RegistrationPreviewComponent _component;
        private RegistrationPreviewComponentControl _control;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (RegistrationPreviewComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new RegistrationPreviewComponentControl(_component);
                    _control.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(NavigatingEventHandler);
                }
                return _control;
            }
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
                if (embeddedActionModel != null)
                {
                    // need to find the action in the model that matches the uri path
                    // TODO clean this up - this is a bit of hack right now
                    ActionPath uriPath = new ActionPath(e.Url.LocalPath, null);
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
        }
    }
}
