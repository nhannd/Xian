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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    public interface IContainerFolder : IFolder
    {
        IList<Type> SubfolderTypes { get; }
        void AddSubfolderType(Type type);
        IList<IFolder> Subfolders { get; }
        void AddFolder(IFolder subFolder);
        bool RemoveFolder(IFolder subFolder);
    }

    public abstract class ContainerFolder : Folder, IContainerFolder//, IDisposable
    {
        private readonly IList<IFolder> _subfolders;
        private readonly string _text;
        private readonly IList<Type> _subfolderTypes;

        private readonly IconSet _openIconSet;
        private readonly IconSet _closedIconSet;

        public ContainerFolder(string text)
            : this(text, new List<Type>())
        {
        }

        public ContainerFolder(string text, Type subfolderType)
            : this(text)
        {
            _subfolderTypes.Add(subfolderType);
        }

        public ContainerFolder(string text, IList<Type> subfolderTypes)
        {
            _subfolders = new List<IFolder>();
            _text = text;
            _subfolderTypes = subfolderTypes;

            _openIconSet = new IconSet(IconScheme.Colour, "ContainerFolderOpenSmall.png", "ContainerFolderOpenMedium.png", "ContainerFolderOpenMedium.png");
            _closedIconSet = new IconSet(IconScheme.Colour, "ContainerFolderClosedSmall.png", "ContainerFolderClosedMedium.png", "ContainerFolderClosedMedium.png");
            _iconSet = _closedIconSet;
            _resourceResolver = new ResourceResolver(typeof(ContainerFolder).Assembly);
        }

        /// <summary>
        /// Not to be used by subclasses
        /// </summary>
        private ContainerFolder()
        {
        }

        #region Folder overrides

        public override string Text
        {
            get { return _text; }
        }

        public override void Refresh()
        {
        }

        public override void RefreshCount()
        {
        }

        public override ClearCanvas.Desktop.Tables.ITable ItemsTable
        {
            get { return null; }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            // All subfolders are of the same type, so just let the first subfolder determine if it can handle the drop if subfolder exist
            if (_subfolders.Count == 0)
            {
                return DragDropKind.None;
            }
            else
            {
                return _subfolders[0].CanAcceptDrop(items, kind);
            }
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            // All subfolders are of the same type, so just let the first subfolder handle the drop if subfolder exist
            if (_subfolders.Count == 0)
            {
                return DragDropKind.None;
            }
            else
            {
                return _subfolders[0].AcceptDrop(items, kind);
            }
        }

        public override void OpenFolder()
        {
            if (_openIconSet != null)
                this.IconSet = _openIconSet;

            base.OpenFolder();
        }

        public override void CloseFolder()
        {
            if (_closedIconSet != null)
                this.IconSet = _closedIconSet;

            base.CloseFolder();
        }


        #endregion

        #region IContainerFolder Members

        public IList<Type> SubfolderTypes
        {
            get { return _subfolderTypes; }
        }

        public void AddSubfolderType(Type type)
        {
            if (false == CollectionUtils.Contains<Type>(_subfolderTypes, delegate(Type existingType) { return type == existingType; }))
                _subfolderTypes.Add(type);
        }

        public IList<IFolder> Subfolders
        {
            get { return _subfolders; }
        }

        public void AddFolder(IFolder subFolder)
        {
            if (CollectionUtils.Contains<Type>(_subfolderTypes, delegate(Type type) { return type == subFolder.GetType(); }))
                _subfolders.Add(subFolder);
            else
                throw new Exception("Bad subfolder");
        }

        public bool RemoveFolder(IFolder subFolder)
        {
            return _subfolders.Remove(subFolder);
        }

        #endregion

        #region IDisposable Members

        //TODO

        //public void Dispose()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        #endregion
    }
}
