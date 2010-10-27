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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
    public abstract class AlertBase<TEntity> : IAlert<TEntity>
    {
        #region IAlert<TEntity> Members

		public abstract string Id { get; }

        public abstract AlertNotification Test(TEntity entity, IPersistenceContext context);

        #endregion
    }
}
