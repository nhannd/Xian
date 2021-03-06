#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.SqlServer2005.EntityBrokers
{
    using ClearCanvas.Common;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise.SqlServer2005;

    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ReconcileQueueUidBroker : EntityBroker<ReconcileQueueUid, ReconcileQueueUidSelectCriteria, ReconcileQueueUidUpdateColumns>, IReconcileQueueUidEntityBroker
    {
        public ReconcileQueueUidBroker() : base("ReconcileQueueUid")
        { }
    }
}
