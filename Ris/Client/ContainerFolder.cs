using System;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public interface IContainerFolder : IFolder
    {
        Type SubfolderType { get; }
        IList<IFolder> Subfolders { get; }
        void AddFolder(IFolder subFolder);
        bool RemoveFolder(IFolder subFolder);
    }

    public abstract class ContainerFolder : Folder, IContainerFolder//, IDisposable
    {
        private IList<IFolder> _subfolders;
        private string _text;
        private Type _subfolderType;

        private IconSet _openIconSet;
        private IconSet _closedIconSet;

        public ContainerFolder(string text, Type subfolderType)
        {
            _subfolders = new List<IFolder>();
            _text = text;
            _subfolderType = subfolderType;
            _openIconSet = new IconSet(IconScheme.Colour, "ContainerFolderOpenSmall.png", "ContainerFolderOpenMedium.png", "ContainerFolderOpenMedium.png");
            _closedIconSet = new IconSet(IconScheme.Colour, "ContainerFolderClosedSmall.png", "ContainerFolderClosedMedium.png", "ContainerFolderClosedMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(typeof(ContainerFolder).Assembly);
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
            //get { return string.Format("{0} ({1} Subfolder[s])", _text, _subfolders.Count); }
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

        public Type SubfolderType
        {
            get { return _subfolderType; }
        }

        public IList<IFolder> Subfolders
        {
            get { return _subfolders; }
        }

        public void AddFolder(IFolder subFolder)
        {
            _subfolders.Add(subFolder);
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
