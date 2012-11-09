#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services.View
{
    /// <summary>
    /// Silverlight implementation of <see cref="IWorkspaceView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateWorkspaceView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding <see cref="SetTitle"/> to customize the display of the workspace title.
    /// </para>
    /// </remarks>
    public class WorkspaceView : DesktopObjectView, IWorkspaceView
    {
        private readonly DesktopWindowView _desktopView;
        private IApplicationComponentView _componentView;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="desktopView"></param>
        protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
        {
            _desktopView = desktopView;
            _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(workspace.Component.GetType());
            _componentView.SetComponent((IApplicationComponent)workspace.Component);
        }


        #region DesktopObjectView overrides

        /// <summary>
        /// Sets the title of the workspace.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
        }

        /// <summary>
        /// Opens the workspace, adding the tab to the tab group.
        /// </summary>
        public override void Open()
        {
            _desktopView.AddWorkspaceView(this);
        }

        /// <summary>
        /// Activates the workspace, making the tab the selected tab.
        /// </summary>
        public override void Activate()
        {
        }

        public IWorkspaceDialogBoxView CreateDialogBoxView(WorkspaceDialogBox dialogBox)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Show()
        {
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override void Hide()
        {
        }

        public override void SetModelObject(object modelObject)
        {
            throw new NotImplementedException();
        }

        public override void ProcessMessage(Message message)
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
        }

        protected override Entity CreateEntity()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity(Entity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            _desktopView.RemoveWorkspaceView(this);

            var componentView = _componentView as IDisposable;
            if (componentView == null)
                return;

            componentView.Dispose();
            _componentView = null;
        }

        #endregion
    }
}
