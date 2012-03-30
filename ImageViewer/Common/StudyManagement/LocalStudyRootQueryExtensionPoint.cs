#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    //This is temporary until we can fix the server tree.  Really need to do that soon.
    public sealed class LocalStudyRootQueryExtensionPoint : ExtensionPoint<IStudyRootQuery>
    {
    }
}
