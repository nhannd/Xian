using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public class DuplicateSopReceivedQueue : StudyIntegrityQueue
    {

    }

    public interface IInsertDuplicateSopReceivedQueue : IProcedureQueryBroker<InsertDuplicateSopReceivedQueueParameters, DuplicateSopReceivedQueue>
    {
    }
}
