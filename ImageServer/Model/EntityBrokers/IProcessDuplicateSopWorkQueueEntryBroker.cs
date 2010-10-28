#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public interface IWorkQueueProcessDuplicateSopBroker : IEntityBroker<WorkQueueProcessDuplicateSop, WorkQueueProcessDuplicateSopSelectCriteria, WorkQueueProcessDuplicateSopUpdateColumns>
    { }
}
