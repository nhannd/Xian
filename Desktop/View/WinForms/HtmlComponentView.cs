using System.IO;
using System.Reflection;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class HtmlComponentView : WinFormsView, IApplicationComponentView
    {
        private IApplicationComponent _component;
        private HtmlComponentControl _control;
        private string _templateResource;
        private IResourceResolver _resourceResolver;

        public HtmlComponentView(string templateResource)
        {
            _templateResource = templateResource;
        }

        public HtmlComponentView(string templateResource, IResourceResolver resolver)
        {
            _templateResource = templateResource;
            _resourceResolver = resolver;
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                return this.Control;
            }
        }

        public IApplicationComponent Component
        {
            get { return _component; }
        }

        protected HtmlComponentControl Control
        {
            get
            {
                if (_control == null)
                {
                    // if a resource resolver was not established, create a smart default
                    if (_resourceResolver == null)
                    {
                        // assume the resource is in the component's assembly, or failing that, the view's assembly
                        _resourceResolver = new ResourceResolver(new Assembly[] { _component.GetType().Assembly, this.GetType().Assembly });
                    }

                    Stream resourceStream = _resourceResolver.OpenResource(_templateResource);
                    _control = new HtmlComponentControl(_component,
                        new ActiveTemplate(new StreamReader(resourceStream)));

                    _control.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(NavigatingEventHandler);
                }
                return _control;
            }
        }

        protected void Refresh()
        {
            this.Control.ReloadPage();
        }

        protected virtual ActionModelNode GetEmbeddedActionModel()
        {
            return null;
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
