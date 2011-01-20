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
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class PersistentWorkflow : IWorkflow
    {
        private readonly IPersistenceContext _context;

        public PersistentWorkflow(IPersistenceContext context)
        {
            _context = context;
        }

        #region IWorkflow Members

        public void AddEntity(Entity entity)
        {
            _context.Lock(entity, DirtyState.New);
        }

        public TBroker GetBroker<TBroker>() where TBroker : IPersistenceBroker
        {
            return _context.GetBroker<TBroker>();
        }

        #endregion
    }
}
