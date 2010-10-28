#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Data.Model
{
    [Serializable]
    public class WorkQueueInfo
    {
        private string _server;
        private int _itemCount;

        public string Server
        {
            get { return _server;}
            set { _server = value;}
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; }
        }

        public WorkQueueInfo(string server, int itemCount)
        {
            _server = server;
            _itemCount = itemCount;
        }
    }
}
