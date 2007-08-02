using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public abstract class ServerEntity : Entity
    {
        private ServerEntityKey _key;
        private String _name;

        public ServerEntity(String name)
            : base()
        {
            _name = name;
        }

        public String Name
        {
            get { return _name; }
        }

        public void SetKey(ServerEntityKey key)
        {
            _key = key;
        }

        public ServerEntityKey GetKey()
        {
            if (_key == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return _key;
        }

        /// <summary>
        /// Not supported by ServerEntity objects
        /// </summary>
        /// <returns></returns>
        public override EntityRef GetRef()
        {
            throw new InvalidOperationException("Not supported by ServerEntity");
        }
    }
}
