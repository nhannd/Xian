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
            add { Core.TransactionNotifier.Subscribe(typeof(ModalityProcedureStep), value); }
            remove { Core.TransactionNotifier.Unsubscribe(typeof(ModalityProcedureStep), value); }
        }

    }
}
