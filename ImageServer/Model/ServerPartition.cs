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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Model
{
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public class ServerPartition : ServerEntity
    {
        #region Constructors
        public ServerPartition()
            : base("ServerPartition")
        {
        }
        #endregion

        #region Private Members
        private bool _enabled;
        private String _description;
        private string _aeTitle;
        private int _port;
        private string _partitionFolder;
        ServerEntityKey _serverPartitionKey;
        #endregion

        #region Public Properties
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public String AeTitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public String PartitionFolder
        {
            get { return _partitionFolder; }
            set { _partitionFolder = value; }
        }
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        #endregion


        #region Static Methods
        static public ServerPartition Load(ServerEntityKey key)
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            ISelectServerPartition broker = read.GetBroker<ISelectServerPartition>();
            ServerPartition entity = broker.Load(key);
            read.Dispose();
            return entity;
        }
        #endregion


    }
}
