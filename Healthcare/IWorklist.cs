#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Healthcare
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public interface IWorklist
    {
        IList GetWorklistItems(IWorklistQueryContext wqc);
        int GetWorklistItemCount(IWorklistQueryContext wqc);
    }
}