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
