#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

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

        object GetValue(StudyItem item);

        event EventHandler<ItemEventArgs<StudyItem>> ColumnValueChanged;
    }
}
