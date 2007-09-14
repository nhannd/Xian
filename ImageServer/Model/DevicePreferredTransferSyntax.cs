using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model
{
    public class DevicePreferredTransferSyntax : ServerEntity
    {
        #region Constructors
        public DevicePreferredTransferSyntax()
            : base("DevicePreferredTransferSyntax")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _deviceKey;
        private ServerEntityKey _serverSopClassKey;
        private ServerEntityKey _serverTransferSyntaxKey;
        #endregion

        public ServerEntityKey DeviceKey
        {
            get { return _deviceKey; }
            set { _deviceKey = value; }
        }

        public ServerEntityKey ServerSopClassKey
        {
            get { return _serverSopClassKey; }
            set { _serverSopClassKey = value; }
        }

        public ServerEntityKey ServerTransferSyntaxKey
        {
            get { return _serverTransferSyntaxKey; }
            set { _serverTransferSyntaxKey = value; }
        }

        public ServerSopClass GetServerSopClass()
        {
            return ServerSopClass.GetServerSopClass(_serverSopClassKey);
        }

        public ServerTransferSyntax GetServerTransferSyntax()
        {
            return ServerTransferSyntax.GetServerTransferSyntax(_serverTransferSyntaxKey);
        }
    }
}
