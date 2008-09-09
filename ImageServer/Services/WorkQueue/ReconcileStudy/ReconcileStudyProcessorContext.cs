using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    class ReconcileStudyProcessorContext
    {
        private Model.WorkQueue _item;
        private ReconcileStudyWorkQueueData _data;
        private ServerPartition _partition;

        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }


        public ReconcileStudyWorkQueueData Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }

}
