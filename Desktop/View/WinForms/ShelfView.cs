using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Docking;
using System.ComponentModel;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ShelfView : DesktopObjectView, IShelfView
    {
        private DesktopWindowView _desktopView;
        private Content _content;
        private Shelf _shelf;


        protected internal ShelfView(Shelf shelf, DesktopWindowView desktopView)
        {
            _shelf = shelf;
            _desktopView = desktopView;
        }

        protected internal Content Content
        {
            get { return _content; }
        }

        protected internal ShelfDisplayHint DisplayHint
        {
            get { return _shelf.DisplayHint; }
        }

        #region DesktopObjectView overrides

        public override void Open()
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_shelf.Component.GetType());
            componentView.SetComponent(_shelf.Component);
            _content = _desktopView.AddShelfView(this, (Control)componentView.GuiElement, _shelf.Title, _shelf.DisplayHint);
        }

        public override void SetTitle(string title)
        {
            if (_content != null)
            {
                _content.Title = title;
            }
        }

        public override void Activate()
        {
            _desktopView.ActivateShelfView(this);
        }

        public override void Show()
        {
            _desktopView.ShowShelfView(this);
        }

        public override void Hide()
        {
            _desktopView.HideShelfView(this);
        }

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
