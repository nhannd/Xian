using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare
{
    public abstract partial class HealthcareServiceLayer
    {
        public event EventHandler<EntityChangeEventArgs> ModalityProcedureStepChanged
        {
            add { this.Session.TransactionNotifier.Subscribe(typeof(ModalityProcedureStep), value); }
            remove { this.Session.TransactionNotifier.Unsubscribe(typeof(ModalityProcedureStep), value); }
        }

    }
}
