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

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Class representing the primary key of a <see cref="ServerEntity"/> object.
    /// </summary>
    [Serializable]
    public class ServerEntityKey
    {
        #region Private Members

        private readonly object _key;
        private readonly String _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the <see cref="ServerEntity"/>.</param>
        /// <param name="entityKey">The primary key object itself.</param>
        public ServerEntityKey(String name, object entityKey)
        {
            _name = name;
            _key = entityKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The primary key itself.
        /// </summary>
        public object Key
        {
            get { return _key; }
        }

        /// <summary>
        /// The name of the <see cref="ServerEntity"/>.
        /// </summary>
        public String EntityName
        {
            get { return _name; }
        }

        #endregion

        #region Public Overrides

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ServerEntityKey key = obj as ServerEntityKey;
            if (key == null) return false;

            return _key.Equals(key.Key);
        }

        public override string ToString()
        {
            return _key.ToString();
        }

        #endregion
    }
}
