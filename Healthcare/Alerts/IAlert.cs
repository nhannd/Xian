#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
    public interface IAlert<TEntity>
    {
		/// <summary>
		/// Identifies this type of alert.
		/// </summary>
		string Id { get; }

        /// <summary>
        /// Test the entity for any alert conditions.  This method must be thread-safe
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns>NULL if the test does not trigger an alert </returns>
        AlertNotification Test(TEntity entity, IPersistenceContext context);
    }
}
