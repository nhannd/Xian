using System;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;

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

        public ContainerFolder(string text, Type subfolderType)
        {
            _subfolders = new List<IFolder>();
            _text = text;
            _subfolderType = subfolderType;
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
