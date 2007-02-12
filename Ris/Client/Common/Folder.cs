using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Common
{
    public abstract class Folder : IFolder
    {
        private event EventHandler _textChanged;
        private event EventHandler _iconChanged;
        private event EventHandler _tooltipChanged;
        private ActionModelNode _menuModel;


        protected void NotifyTextChanged()
        {
            EventsHelper.Fire(_textChanged, this, EventArgs.Empty);
        }

        #region IFolder Members

        public abstract string Text { get; }
        public abstract void Refresh();

        public virtual event EventHandler TextChanged
        {
            add { _textChanged += value; }
            remove { _textChanged -= value; }
        }

        public virtual Image Icon
        {
            get { return null; }
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

        public virtual ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set { _menuModel = value; }
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
