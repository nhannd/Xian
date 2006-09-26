using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.Common.Scripting;
using System.Reflection;

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
                }
                return _control;
            }
        }

        protected void Refresh()
        {
            this.Control.ReloadPage();
        }
    }
}
