using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public interface IWorkItemActivityCallback
    {
        [OperationContract(IsOneWay = true)]
        void WorkItemChanged(WorkItemData progressItem);
    }
}
