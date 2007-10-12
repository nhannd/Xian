#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using Crownwood.DotNetMagic.Docking;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IShelfView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateShelfView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding <see cref="SetTitle"/> to customize the display of the workspace title.
    /// </para>
    /// </remarks>
    public class ShelfView : DesktopObjectView, IShelfView
    {
        private DesktopWindowView _desktopView;
        private Content _content;
        private Shelf _shelf;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shelf"></param>
        /// <param name="desktopView"></param>
        protected internal ShelfView(Shelf shelf, DesktopWindowView desktopView)
        {
            _shelf = shelf;
            _desktopView = desktopView;
        }

        /// <summary>
        /// Gets the <see cref="Content"/> object that is hosted by the docking window.
        /// </summary>
        protected internal Content Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Gets the <see cref="ShelfDisplayHint"/> for this shelf.
        /// </summary>
        protected internal ShelfDisplayHint DisplayHint
        {
            get { return _shelf.DisplayHint; }
        }

        #region DesktopObjectView overrides

        /// <summary>
        /// Opens this shelf view.
        /// </summary>
        public override void Open()
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_shelf.Component.GetType());
            componentView.SetComponent((IApplicationComponent)_shelf.Component);
            _content = _desktopView.AddShelfView(this, (Control)componentView.GuiElement, _shelf.Title, _shelf.DisplayHint);
        }

        /// <summary>
        /// Sets the title of the shelf.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            if (_content != null)
            {
                _content.Title = title;
            }
        }

        /// <summary>
        /// Activates the shelf.
        /// </summary>
        public override void Activate()
        {
            _desktopView.ActivateShelfView(this);
        }

        /// <summary>
        /// Shows the shelf.
        /// </summary>
        public override void Show()
        {
            _desktopView.ShowShelfView(this);
        }

        /// <summary>
        /// Hides the shelf.
        /// </summary>
        public override void Hide()
        {
            _desktopView.HideShelfView(this);
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _content != null)
            {
                _desktopView.RemoveShelfView(this);

                // make sure to dispose of the control now (dotnetmagic doesn't do it automatically)
                if (!_content.Control.IsDisposed)
                {
                    _content.Control.Dispose();
                }
                _content = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
