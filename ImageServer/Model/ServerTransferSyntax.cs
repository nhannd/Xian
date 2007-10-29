#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
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
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IGetServerTransferSyntaxes broker = read.GetBroker<IGetServerTransferSyntaxes>();
                IList<ServerTransferSyntax> list = broker.Execute();
                foreach (ServerTransferSyntax syntax in list)
                {
                    _dict.Add(syntax.GetKey(), syntax);
                }
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
