using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Tests
{
    public class ServiceLayerTestHelper
    {
        /// <summary>
        /// This method exposes the internal ServicLayer.CurrentContext setter.  This method may be used when a ServiceLayer-derived object is manually instantiated 
        /// outside of the normal framework mechanism (eg when testing via NUnit) to manually set the PersistenceContext.  Allows for a mock PersistenceContext to be 
        /// used, which in turn allows mock EntityBroker objects to be used to return test data.
        /// </summary>
        /// <example>
        /// <code>
        ///    _mocks = new Mockery();
        ///    _mockPersistanceContext = _mocks.NewMock&lt;IPersistenceContext&gt;();
        ///    _service= new ServiceLayer();
        ///    ServiceLayerTestHelper.SetServiceLayerPersistenceContext(
        ///        _service, _mockPersistanceContext);
        /// </code>
        /// </example>
        /// <see>ClearCanvas.Ris.Services.Tests.AdtServiceTest</see>
        /// <param name="serviceLayer"></param>
        /// <param name="context"></param>
        public static void SetServiceLayerPersistenceContext(ServiceLayer serviceLayer, IPersistenceContext context)
        {
            serviceLayer.CurrentContext = context;
        }
    }
}
