using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public interface IAttachStudyToPatient : IProcedureUpdateBroker<AttachStudyToPatientParamaters>
    {
    }
}
