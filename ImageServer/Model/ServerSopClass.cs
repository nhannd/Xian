using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model
{
    public class ServerSopClass : ServerEntity
    {
        private static Dictionary<ServerEntityKey, ServerSopClass> _dict = new Dictionary<ServerEntityKey, ServerSopClass>();

        /// <summary>
        /// One-time load from the database of sop classes.
        /// </summary>
        static ServerSopClass()
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IGetServerSopClasses broker = read.GetBroker<IGetServerSopClasses>();
            IList<ServerSopClass> list = broker.Execute();
            read.Dispose();

            foreach (ServerSopClass sop in list)
            {
                _dict.Add(sop.GetKey(), sop);
            }
        }

        #region Constructors
        public ServerSopClass()
            : base("ServerSopClass")
        {
        }
        #endregion

        #region Private Members
        private String _sopClassUid;
        private String _description;
        private bool _nonImage;
        #endregion

        public String SopClassUid
        {
            get { return _sopClassUid; }
            set { _sopClassUid = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public bool NonImage
        {
            get { return _nonImage; }
            set { _nonImage = value; }
        }

        public static ServerSopClass GetServerSopClass(ServerEntityKey lookup)
        {
            if (!_dict.ContainsKey(lookup))
                throw new PersistenceException("Unknown ServerSopClass: " + lookup, null);

            return _dict[lookup];
        }
    }
}
