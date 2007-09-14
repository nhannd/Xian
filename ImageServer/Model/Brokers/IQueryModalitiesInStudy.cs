using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    /// <summary>
    /// Broker for selecting the value of Modality based on a partition and Study Instance UID.
    /// </summary>
    public interface IQueryModalitiesInStudy : IProcedureQueryBroker<ModalitiesInStudyQueryParameters, Series>
    {
    }
}
