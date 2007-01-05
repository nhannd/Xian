using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public class ActivityScheduling
    {
        private ActivityPerformer _performer;
        private DateTime? _startTime;
        private DateTime? _endTime;

        public ActivityPerformer Performer
        {
            get { return _performer; }
            set { _performer = value; }
        }

        public DateTime? StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
    }
}
