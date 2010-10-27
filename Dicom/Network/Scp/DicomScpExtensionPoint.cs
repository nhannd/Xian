#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Network.Scp
{
    /// <summary>
    /// Extension point for plugins to the <see cref="DicomScp"/> class.
    /// </summary>
    [ExtensionPoint()]
    public class DicomScpExtensionPoint<TContext> : ExtensionPoint<IDicomScp<TContext>>
    {
    }
}
