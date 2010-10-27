#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public abstract class ServerEntity : Entity
    {
        #region Private Members
        private readonly object _syncRoot = new object();
        private ServerEntityKey _key;
        private readonly String _name;

        #endregion

        #region Constructors

        public ServerEntity(String name)
        {
            _name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the ServerEntity object.
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        public ServerEntityKey Key
        {
            get { return _key; }
        }

        protected object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the primary key of the ServerEntity object.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(ServerEntityKey key)
        {
            _key = key;
        }

        /// <summary>
        /// Get the primary key of the ServerEntity object.
        /// </summary>
        /// <returns>A <see cref="ServerEntityKey"/> object representating the primary key.</returns>
        public ServerEntityKey GetKey()
        {
            if (_key == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return _key;
        }

        /// <summary>
        /// Not supported by ServerEntity objects
        /// </summary>
        /// <returns></returns>
        public override EntityRef GetRef()
        {
            throw new InvalidOperationException("Not supported by ServerEntity");
        }

        #endregion
    }
}
