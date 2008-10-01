using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingClientConsole
{
    class InstanceInfo
    {
        private string _SopUid;

        public string SopUid
        {
            get { return _SopUid; }
            set { _SopUid = value; }
        }
    }

    class SeriesInfo
    {
        private string _SeriesUid;
        private List<InstanceInfo> _instances = new List<InstanceInfo>();
        public string SeriesUid
        {
            get { return _SeriesUid; }
            set { _SeriesUid = value; }
        }

        public List<InstanceInfo> Instances
        {
            get { return _instances; }
            set { _instances = value; }
        }
    }

    class StudyInfo
    {
        private string _StudyUid;
        private List<SeriesInfo> _series = new List<SeriesInfo>();
        public string StudyUid
        {
            get { return _StudyUid; }
            set { _StudyUid = value; }
        }

        public List<SeriesInfo> Series
        {
            get { return _series; }
            set { _series = value; }
        }
    }
}
