#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public class DicomServiceNodeList : IList<IDicomServiceNode>
    {
        private readonly ReadOnlyCollection<IDicomServiceNode> _serviceNodes;

        public DicomServiceNodeList()
        {
            _serviceNodes = new ReadOnlyCollection<IDicomServiceNode>(new List<IDicomServiceNode>());
        }

        public DicomServiceNodeList(IEnumerable<IDicomServiceNode> toDicomServiceNodes)
        {
            _serviceNodes = new ReadOnlyCollection<IDicomServiceNode>(toDicomServiceNodes.ToList());
        }

        public string Name { get; set; }

        public string Id { get; set; }

        [Obsolete("Use IsLocalServer instead.")]
        public bool IsLocalDatastore
        {
            get { return IsLocalServer; }
        }

        public bool IsLocalServer
        {
            get
            {
                return Count > 0 && this[0].IsLocal;
            }
        }

        public bool AnySupport<T>() where T : class
        {
            return this.Any(s => s.IsSupported<T>());
        }

        public bool AllSupport<T>() where T : class
        {
            return this.All(s => s.IsSupported<T>());
        }

        #region IList<IDicomServiceNode> Members

        public int IndexOf(IDicomServiceNode item)
        {
            return _serviceNodes.IndexOf(item);
        }

        void IList<IDicomServiceNode>.Insert(int index, IDicomServiceNode item)
        {
            throw new NotSupportedException();
        }

        void IList<IDicomServiceNode>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public IDicomServiceNode this[int index]
        {
            get { return _serviceNodes[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region ICollection<IDicomServiceNode> Members

        void ICollection<IDicomServiceNode>.Add(IDicomServiceNode item)
        {
            throw new NotSupportedException();
        }

        void ICollection<IDicomServiceNode>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(IDicomServiceNode item)
        {
            return _serviceNodes.Contains(item);
        }

        public void CopyTo(IDicomServiceNode[] array, int arrayIndex)
        {
            _serviceNodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _serviceNodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<IDicomServiceNode>.Remove(IDicomServiceNode item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<IDicomServiceNode> Members

        public IEnumerator<IDicomServiceNode> GetEnumerator()
        {
            return _serviceNodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _serviceNodes.GetEnumerator();
        }

        #endregion
    }
}