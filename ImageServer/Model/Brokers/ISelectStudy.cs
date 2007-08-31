using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    /// <summary>
    /// Broker for dynamic queries against the <see cref="Study"/> table.
    /// </summary>
    public interface ISelectStudy : ISelectBroker<StudySelectCriteria,Study>
    {
    }
}
