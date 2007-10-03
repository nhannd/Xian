using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SelectBrokers
{
    /// <summary>
    /// Broker for dynamic queries against the <see cref="Study"/> table.
    /// </summary>
    public interface ISelectStudy : ISelectBroker<StudySelectCriteria,Study>
    {
    }
}