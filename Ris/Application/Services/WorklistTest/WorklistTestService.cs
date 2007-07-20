using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.WorklistTest;
using ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Application.Services.WorklistTest
{
    [ServiceImplementsContract(typeof(IWorklistTestService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class WorklistTestService : ApplicationServiceBase, IWorklistTestService
    {
        #region IWorklistTestService Members

        [ReadOperation]
        public GetAWorklistResponse GetAWorklist(GetAWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.GetBroker<IWorklistBroker>().FindOne(new WorklistSearchCriteria());

            WorklistAssembler assembler = new WorklistAssembler();
            return new GetAWorklistResponse(assembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [ReadOperation]
        public DoWorklistTestResponse DoWorklistTest(DoWorklistTestRequest request)
        {
            Worklist aWorklist = this.PersistenceContext.Load<Worklist>(request.WorklistRef);

            int count = aWorklist.GetWorklistCount(this.CurrentUserStaff, this.PersistenceContext);

            return new DoWorklistTestResponse(string.Format("Name: {0}\r\nCount: {1}", aWorklist.Name, count));
        }

        #endregion
    }
}
