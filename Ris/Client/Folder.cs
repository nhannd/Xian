using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public abstract class Folder : IFolder
    {
        private event EventHandler _textChanged;
        private event EventHandler _iconChanged;
        private event EventHandler _tooltipChanged;
        private event EventHandler _refreshBegin;
        private event EventHandler _refreshFinish;

        private ActionModelNode _menuModel;

        private IconSet _iconSet;
        private IResourceResolver _resourceResolver;
        private bool _isOpen;

        protected void NotifyTextChanged()
        {
            EventsHelper.Fire(_textChanged, this, EventArgs.Empty);
        }

        protected void NotifyRefreshBegin()
        {
            EventsHelper.Fire(_refreshBegin, this, EventArgs.Empty);
        }

        protected void NotifyRefreshFinish()
        {
            EventsHelper.Fire(_refreshFinish, this, EventArgs.Empty);
        }

        #region IFolder Members

        public abstract string Text { get; }

        public void Refresh()
        {
            OnRefreshDelegate(null);
        }

        public void RefreshCount()
        {
            OnRefreshCountDelegate(null);
        }

        protected abstract void OnRefreshDelegate(object stateObject);
        protected abstract void OnRefreshCountDelegate(object stateObject);

        public virtual void OpenFolder() 
        {
            _isOpen = true;
            Refresh();
        }

        public virtual void CloseFolder() 
        {
            _isOpen = false;
        }

        public virtual event EventHandler TextChanged
        {
            add { _textChanged += value; }
            remove { _textChanged -= value; }
        }

        public virtual IconSet IconSet
        {
            get { return _iconSet; }
            set
            {
                _iconSet = value;
                EventsHelper.Fire(_iconChanged, this, EventArgs.Empty);
            }
        }

        public virtual IResourceResolver ResourceResolver
        {
            get { return _resourceResolver; }
            set { _resourceResolver = value; }
        }

        public virtual event EventHandler IconChanged
        {
            add { _iconChanged += value; }
            remove { _iconChanged -= value; }
        }

        public virtual string Tooltip
        {
            get { return null; }
        }

        public virtual event EventHandler TooltipChanged
        {
            add { _tooltipChanged += value; }
            remove { _tooltipChanged -= value; }
        }

        public virtual event EventHandler RefreshBegin
        {
            add { _refreshBegin += value; }
            remove { _refreshBegin -= value; }
        }

        public virtual event EventHandler RefreshFinish
        {
            add { _refreshFinish += value; }
            remove { _refreshFinish -= value; }
        }

        public virtual ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set { _menuModel = value; }
        }

        public virtual bool IsOpen
        {
            get { return _isOpen; }
        }

        public virtual DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        public virtual DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        public virtual void DragComplete(object[] items, DragDropKind kind)
        {
        }

        public abstract ITable ItemsTable
        {
            get;
        }

        #endregion
    }
}
