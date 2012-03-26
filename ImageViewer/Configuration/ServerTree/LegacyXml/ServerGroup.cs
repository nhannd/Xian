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

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.LegacyXml
{
    [Serializable]
    public class ServerGroup
    {
        protected ServerGroup()
        {
        }

        public ServerGroup(string name)
        {
            NameOfGroup = name;
            ChildGroups = new List<ServerGroup>();
            ChildServers = new List<Server>();
        }

        #region Public Properties/Fields

        public String NameOfGroup { get; set; }

        /// <summary>
        /// Public field for serialization only.
        /// </summary>
        public List<ServerGroup> ChildGroups;
        /// <summary>
        /// Public field for serialization only.
        /// </summary>
        public List<Server> ChildServers;
		
        #endregion
    }
}