#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    [DataContract]
    public class SessionToken : IEquatable<SessionToken>
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

        public override bool Equals(object obj)
        {
            SessionToken other = obj as SessionToken;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() ^ _expiryTime.GetHashCode();
        }

        #region IEquatable<SessionToken> Members

        public bool Equals(SessionToken other)
        {
            if (other == null)
                return false;
            return _id == other._id
                && _expiryTime == other._expiryTime;
        }

        #endregion
    }
}