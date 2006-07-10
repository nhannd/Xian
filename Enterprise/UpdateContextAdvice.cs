using System;
using System.Collections.Generic;
using System.Text;

using Spring.Aop;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise
{
    public class UpdateContextAdvice : PersistenceContextAdvice
    {
        public UpdateContextAdvice(Session session)
            : base(session)
        {
        }

        public override object Invoke(IMethodInvocation invocation)
        {
            ServiceLayer serviceLayer = (ServiceLayer)invocation.This;
            using(IUpdateContext uctx = this.Session.GetUpdateContext())
            {
                object retval = null;
                try
                {
                    // set the current context of the service layer
                    serviceLayer.CurrentContext = uctx;
                    retval = invocation.Proceed();
                    
                    // commit transaction
                    uctx.Commit();

                    // forward the change set to the transaction monitor
                    this.Session.TransactionMonitor.Queue(uctx.EntityChangeSet);

                    return retval;
                }
                catch(Exception e)
                {
                    // rollback transaction
                    uctx.Rollback();

                    // rethrow
                    throw e;
                }
                finally
                {
                    // be sure to remove the context from the service layer
                    serviceLayer.CurrentContext = null;
                }
            }
        }
    }
}
