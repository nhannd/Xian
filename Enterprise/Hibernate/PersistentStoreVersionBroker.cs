#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IPersistentStoreVersionBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
    public class PersistentStoreVersionBroker : EntityBroker<PersistentStoreVersion, PersistentStoreVersionSearchCriteria>, IPersistentStoreVersionBroker
    {
    }
}