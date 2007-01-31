using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare
{
    public partial interface IHealthcareServiceLayer
    {
        /// <summary>
        /// Provides notification when a <see cref="ModalityProcedureStep"/> entity changes.  This
        /// is actually just a convenience method that allows a client to hook into notifications
        /// from the transaction monitor associated with the session.  If you subscribe to this event,
        /// it is extremely important to explicitly unsubscribe.
        /// </summary>
        event EventHandler<EntityChangeEventArgs> ModalityProcedureStepChanged;
    }
}
