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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Specifies the inteface to the transaction monitor.  This is very preliminary and will likely
    /// change.
    /// </summary>
    public interface ITransactionNotifier
    {
        /// <summary>
        /// Allows a client to subscribe for change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to unsubscribe from change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to queue a set of changes for notification to other clients.
        /// </summary>
        /// <param name="changeSet"></param>
        void Queue(ICollection<EntityChange> changeSet);
    }
}
