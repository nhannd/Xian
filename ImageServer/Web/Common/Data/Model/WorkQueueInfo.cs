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
