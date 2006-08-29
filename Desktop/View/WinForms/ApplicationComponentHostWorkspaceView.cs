using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ApplicationComponentHostWorkspaceViewExtensionPoint))]
    public class ApplicationComponentHostWorkspaceView : WinFormsView, IWorkspaceView
    {
        private ApplicationComponentHostWorkspace _workspace;
        private IApplicationComponentView _componentView;

        public ApplicationComponentHostWorkspaceView()
        {
        }

        #region IWorkspaceView Members

        public void SetWorkspace(IWorkspace workspace)
        {
            _workspace = (ApplicationComponentHostWorkspace)workspace;
        }

        #endregion

        public override object GuiElement
        {
            get { return this.ComponentView.GuiElement; }
        }

        protected IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_workspace.Component.GetType());
                    _componentView.SetComponent(_workspace.Component);
                }
                return _componentView;
            }
        }
    }
}
