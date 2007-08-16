using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Database
{
    public abstract class ServerEnum : ServerEntity
    {
        #region Constructors
        public ServerEnum(String name)
            : base(name)
        {
        }
        #endregion

        #region Private Members
        private short _typeEnum;
        private string _lookup;
        private string _description;
        private string _longDescription;
        #endregion

        #region Public Properties
        public short Enum
        {
            get { return _typeEnum; }
            set { _typeEnum = value; }
        }
        public string Lookup
        {
            get { return _lookup; }
            set { _lookup = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set { _longDescription = value; }
        }
        #endregion

        public abstract void SetEnum(short val);

    }
}
