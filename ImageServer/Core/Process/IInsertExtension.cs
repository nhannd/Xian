#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Process
{
    public interface IInsertExtension
    {
        void InsertExtension(InsertInstanceParameters parameters, DicomFile theFile);
    }

    public class ProcessorInsertExtensionPoint : ExtensionPoint<IInsertExtension>
    { }
}
