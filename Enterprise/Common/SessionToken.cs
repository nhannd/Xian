#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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