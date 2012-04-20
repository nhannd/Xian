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
using System.Linq;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    public class DicomServiceNodeList : List<IDicomServiceNode>
    {
        public DicomServiceNodeList()
        {
        }

        public DicomServiceNodeList(IEnumerable<IDicomServiceNode> toDicomServiceNodes)
            : base(toDicomServiceNodes)
        {
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

        public bool NoneSupport<T>() where T : class
        {
            return !this.Any(s => s.IsSupported<T>());
        }

        public bool HasAnyNonStreamingServers()
        {
            return this.Any(s => s.StreamingParameters == null);
        }

        public bool HasAnyStreamingServers()
        {
            return this.Any(s => s.StreamingParameters != null);
        }

        public bool IsOnlyNonStreamingServers()
        {
            return this.All(s => s.StreamingParameters != null);
        }

        public bool IsOnlyStreamingServers()
        {
            return this.All(s => s.StreamingParameters == null);
        }
    }
}