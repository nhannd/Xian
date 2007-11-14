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

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

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

        protected IconSet _iconSet;
        protected IResourceResolver _resourceResolver;
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
        public abstract void Refresh();
        public abstract void RefreshCount();

        public virtual void OpenFolder() 
        {
            _isOpen = true;
            Refresh();
        }

        public virtual bool StartExpanded
        {
            get { return false; }
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
