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

        /// <summary>
        /// Constructor
        /// </summary>
        public Folder()
        {
            // establish default resource resolver on this assembly (not the assembly of the derived class)
            _resourceResolver = new ResourceResolver(typeof(Folder).Assembly);
        }


        #region IFolder Members

        /// <summary>
        /// Gets the text that should be displayed for the folder
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// Allows the folder to notify that it's text has changed
        /// </summary>
        public virtual event EventHandler TextChanged
        {
            add { _textChanged += value; }
            remove { _textChanged -= value; }
        }

        /// <summary>
        /// Asks the folder to refresh its contents.  The implementation may be asynchronous.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Asks the folder to refresh the count of its contents, without actually refreshing the contents.
        /// The implementation may be asynchronous.
        /// </summary>
        public abstract void RefreshCount();

        /// <summary>
        /// Opens the folder (i.e. instructs the folder to show its "open" state icon).
        /// </summary>
        public virtual void OpenFolder() 
        {
            _isOpen = true;
            Refresh();
        }

        /// <summary>
        /// Closes the folder (i.e. instructs the folder to show its "closed" state icon).
        /// </summary>
        public virtual void CloseFolder()
        {
            _isOpen = false;
        }

        /// <summary>
        /// Indicates if the folder should be initially expanded
        /// </summary>
        public virtual bool StartExpanded
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the iconset that should be displayed for the folder
        /// </summary>
        public IconSet IconSet
        {
            get { return _iconSet; }
            protected set
            {
                _iconSet = value;
                EventsHelper.Fire(_iconChanged, this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Allows the folder to nofity that it's icon has changed
        /// </summary>
        public virtual event EventHandler IconChanged
        {
            add { _iconChanged += value; }
            remove { _iconChanged -= value; }
        }

        /// <summary>
        /// Gets the resource resolver that is used to resolve the Icon
        /// </summary>
        public IResourceResolver ResourceResolver
        {
            get { return _resourceResolver; }
            set { _resourceResolver = value; }
        }

        /// <summary>
        /// Gets the tooltip that should be displayed for the folder
        /// </summary>
        public virtual string Tooltip
        {
            get { return null; }
        }

        /// <summary>
        /// Allows the folder to notify that it's tooltip has changed
        /// </summary>
        public event EventHandler TooltipChanged
        {
            add { _tooltipChanged += value; }
            remove { _tooltipChanged -= value; }
        }

        /// <summary>
        /// Occurs when refresh is about to begin
        /// </summary>
        public event EventHandler RefreshBegin
        {
            add { _refreshBegin += value; }
            remove { _refreshBegin -= value; }
        }

        /// <summary>
        /// Occurs when refresh is about to finish
        /// </summary>
        public event EventHandler RefreshFinish
        {
            add { _refreshFinish += value; }
            remove { _refreshFinish -= value; }
        }

        /// <summary>
        /// Gets the menu model for the context menu that should be displayed when the user right-clicks on the folder.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            protected set { _menuModel = value; }
        }

        /// <summary>
        /// Gets the open/close state of the current folder
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            protected set { _isOpen = value; }
        }

        /// <summary>
        /// Asks the folder if it can accept a drop of the specified items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public virtual DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        /// <summary>
        /// Instructs the folder to accept the specified items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="kind"></param>
        public virtual DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        /// <summary>
        /// Informs the folder that the specified items were dragged from it.  It is up to the implementation
        /// of the folder to determine the appropriate response (e.g. whether the items should be removed or not).
        /// </summary>
        /// <param name="items"></param>
        /// <param name="result">The result of the drag drop operation</param>
        public virtual void DragComplete(object[] items, DragDropKind result)
        {
        }

        /// <summary>
        /// Gets a table of the items that are contained in this folder
        /// </summary>
        public abstract ITable ItemsTable
        {
            get;
        }

        #endregion

        #region Protected members

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

        #endregion

    }
}
