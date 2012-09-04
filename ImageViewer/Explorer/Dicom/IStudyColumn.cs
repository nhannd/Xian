#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ExtensionPoint()]
    public sealed class StudyColumnExtensionPoint : ExtensionPoint<IStudyColumn>
    {
    }

    public interface IStudyColumn
    {
        string Name { get; }

        float WidthFactor { get; }

        object GetValue(StudyTableItem item);

        event EventHandler<ItemEventArgs<StudyTableItem>> ColumnValueChanged;
    }
}
