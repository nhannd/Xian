using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public class ServerEntityKey
    {
        private object _key;
        private String _name;

        public ServerEntityKey(String name, object entityKey)
        {
            _name = name;
            _key = entityKey;
        }

        public object Key
        {
            get { return _key; }
        }

        public String EntityName
        {
            get { return _name; }
        }
    }
}
