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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Mouse.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Mouse
{
    [ExtensionOf(typeof(EntityChangeSetListenerExtensionPoint))]
    public class WorklistIndexChangeSetListener : IEntityChangeSetListener
    {
        #region IEntityChangeSetListener Members

        public void PreCommit(EntityChangeSetPreCommitArgs args)
        {
            // not used
        }

        public void PostCommit(EntityChangeSetPostCommitArgs args)
        {
            UpdateIndex(args.ChangeSet);
        }

        #endregion

        private void UpdateIndex(EntityChangeSet changeSet)
        {
            List<EntityChange> relevantChanges = CollectionUtils.Select(changeSet.Changes,
                delegate(EntityChange change)
                {
                    Type entityClass = Type.GetType(change.EntityClassName);
                    return entityClass != null && entityClass.IsSubclassOf(typeof(ProcedureStep));
                });

            if (relevantChanges.Count > 0)
            {
                using (PersistenceScope pscope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
                {
                    IUpdateContext uctxt = (IUpdateContext)pscope.Context;
                    uctxt.ChangeSetRecorder = null;	// disable auditing

                    foreach (EntityChange change in relevantChanges)
                    {
                        ProcessChange(change, uctxt);
                    }

                    pscope.Complete();
                }
            }
        }

        private void ProcessChange(EntityChange change, IUpdateContext context)
        {
            if (change.ChangeType == EntityChangeType.Delete)
                return;

            ProcedureStep ps = context.Load<ProcedureStep>(change.EntityRef, EntityLoadFlags.Proxy);
			IWorklistViewItemBroker broker = context.GetBroker<IWorklistViewItemBroker>();

            // if this is an update, check for an existing entry
            WorklistViewItem existingItem = null;
            if (change.ChangeType == EntityChangeType.Update)
            {
				WorklistViewItemSearchCriteria where = new WorklistViewItemSearchCriteria();
                where.ProcedureStep.Instance.EqualTo(ps);

                existingItem = CollectionUtils.FirstElement(broker.Find(where));
            }

            // update existing entry, or create a new entry
            if (existingItem != null)
            {
                existingItem.SetProcedureStepInfo(ps);
            }
            else
            {
                WorklistViewItem item = broker.GetViewItemFromSource(ps);
                context.Lock(item, DirtyState.New);
            }
        }
    }
}
