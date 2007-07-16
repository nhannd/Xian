using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class DesktopObjectView : WinFormsView, IDesktopObjectView
    {
        private bool _active;
        private bool _visible;
        private event EventHandler _activeChanged;
        private event EventHandler _visibleChanged;
        private event EventHandler _closeRequested;

        protected DesktopObjectView()
        {
        }
        
        #region IDesktopObjectView Members

        public event EventHandler VisibleChanged
        {
            add { _visibleChanged += value; }
            remove { _visibleChanged -= value; }
        }

        public event EventHandler ActiveChanged
        {
            add { _activeChanged += value; }
            remove { _activeChanged -= value; }
        }

        public event EventHandler CloseRequested
        {
            add { _closeRequested += value; }
            remove { _closeRequested -= value; }
        }

        public abstract void SetTitle(string title);

        public abstract void Open();

        public abstract void Show();

        public abstract void Hide();

        public abstract void Activate();

        public bool Visible
        {
            get { return _visible; }
        }

        public bool Active
        {
            get { return _active; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        #endregion

        #region Helpers

        protected internal void SetVisibleStatus(bool visible)
        {
            if (_visible != visible)
            {
                _visible = visible;
                OnVisibleChanged(EventArgs.Empty);
            }
        }

        protected internal void SetActiveStatus(bool active)
        {
            if (_active != active)
            {
                _active = active;
                OnActiveChanged(EventArgs.Empty);
            }
        }

        protected internal void RaiseCloseRequested()
        {
            OnCloseRequested(EventArgs.Empty);
        }

        #endregion

        #region Protected overridables

        protected virtual void Dispose(bool disposing)
        {
            // nothing to dispose of
        }

        protected virtual void OnCloseRequested(EventArgs e)
        {
            EventsHelper.Fire(_closeRequested, this, e);
        }

        protected virtual void OnActiveChanged(EventArgs e)
        {
            EventsHelper.Fire(_activeChanged, this, e);
        }

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            EventsHelper.Fire(_visibleChanged, this, e);
        }

        #endregion

        #region WinFormsView overrides

        public override object GuiElement
        {
            // not used
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
