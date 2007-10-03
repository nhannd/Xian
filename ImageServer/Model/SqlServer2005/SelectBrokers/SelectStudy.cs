using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.SelectBrokers
{
    /// <summary>
    /// Broker implementation for <see cref="ISelectStudy"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class SelectStudy : SelectBroker<StudySelectCriteria, Study>, ISelectStudy
    {
        public SelectStudy()
            : base("Study")
        {}
    }
}