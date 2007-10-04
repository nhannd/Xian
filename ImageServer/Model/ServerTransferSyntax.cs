using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model
{
    public class ServerTransferSyntax : ServerEntity
    {
        private static Dictionary<ServerEntityKey, ServerTransferSyntax> _dict = new Dictionary<ServerEntityKey, ServerTransferSyntax>();

        /// <summary>
        /// One-time load from the database of transfer syntaxes.
        /// </summary>
        static ServerTransferSyntax()
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IGetServerTransferSyntaxes broker = read.GetBroker<IGetServerTransferSyntaxes>();
            IList<ServerTransferSyntax> list = broker.Execute();
            read.Dispose();

            foreach (ServerTransferSyntax syntax in list)
            {
                _dict.Add(syntax.GetKey(), syntax);
            }
        }

        #region Constructors
        public ServerTransferSyntax()
            : base("ServerTransferSyntax")
        {
        }
        #endregion

        #region Private Members
        private String _uid;
        private String _description;
        private bool _enabled;
        #endregion

        public String Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public static ServerTransferSyntax Load(ServerEntityKey lookup)
        {
            if (!_dict.ContainsKey(lookup))
                throw new PersistenceException("Unknown ServerTransferSyntax: " + lookup, null);

            return _dict[lookup];
        }
    }
}
