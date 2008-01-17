using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    [DataContract]
    public class SessionToken
    {
        private string _id;
        private DateTime _expiryTime;

        /// <summary>
        /// Creates a session token with no expiry time.
        /// </summary>
        /// <param name="id"></param>
        public SessionToken(string id)
        {
            _id = id;
        }

        /// <summary>
        /// Creates a session token with the specified expiry time.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expiryTime"></param>
        public SessionToken(string id, DateTime expiryTime)
        {
            _id = id;
            _expiryTime = expiryTime;
        }

        [DataMember]
        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        [DataMember]
        public DateTime ExpiryTime
        {
            get { return _expiryTime; }
            private set { _expiryTime = value; }
        }
    }
}