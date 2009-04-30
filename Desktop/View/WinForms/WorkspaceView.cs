#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IWorkspaceView"/>. 
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
        private Crownwood.DotNetMagic.Controls.TabPage _tabPage;
        private DesktopWindowView _desktopView;
        private Control _control;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="desktopView"></param>
        protected internal WorkspaceView(Workspace workspace, DesktopWindowView desktopView)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(workspace.Component.GetType());
            componentView.SetComponent((IApplicationComponent)workspace.Component);

            _tabPage = new Crownwood.DotNetMagic.Controls.TabPage();
            _tabPage.Control = _control = componentView.GuiElement as Control;
            _tabPage.Tag = this;

            _desktopView = desktopView;
        }

        /// <summary>
        /// Gets the tab page that hosts this workspace view.
        /// </summary>
        protected internal Crownwood.DotNetMagic.Controls.TabPage TabPage
        {
            get { return _tabPage; }
        }

        #region DesktopObjectView overrides

        /// <summary>
        /// Sets the title of the workspace.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _tabPage.Title = title;
            _tabPage.ToolTip = title;
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
            _tabPage.Selected = true;
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

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tabPage != null)
                {
                    // Remove the tab
                    _desktopView.RemoveWorkspaceView(this);

                    _control.Dispose();
                    _control = null;
                    _tabPage.Dispose();
                    _tabPage = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}
