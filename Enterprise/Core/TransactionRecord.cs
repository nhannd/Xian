using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    public class TransactionRecord : Entity
    {
        private DateTime _timestamp;
        private string _user;
        private string _transaction;
        private string _details;

        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        private TransactionRecord()
        {

        }

        public TransactionRecord(string transaction, string details)
        {
            _user = Thread.CurrentPrincipal.Identity.Name;
            _timestamp = Platform.Time;
            _transaction = transaction;
            _details = details;
        }

        public DateTime TimeStamp
        {
            get { return _timestamp; }
            private set { _timestamp = value; }
        }

        public string User
        {
            get { return _user; }
            private set { _user = value; }
        }

        public string Transaction
        {
            get { return _transaction; }
            private set { _transaction = value; }
        }

        public string Details
        {
            get { return _details; }
            private set { _details = value; }
        }
    }
}
