using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public partial interface IPatientProfileBroker : IEntityBroker<PatientProfile, PatientProfileSearchCriteria>
    {
    }
}
