#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Common.ServerTree.LegacyXml
{
    [Serializable]
    public class Server
    {
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String NameOfServer { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String Location { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String Host { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public String AETitle { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public bool IsStreaming { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int HeaderServicePort { get; set; }
        /// <summary>
        /// Public field for serialization only; do not modify directly.
        /// </summary>
        public int WadoServicePort { get; set; }
    }
}