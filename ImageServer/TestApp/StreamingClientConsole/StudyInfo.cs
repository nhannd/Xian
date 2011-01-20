#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
