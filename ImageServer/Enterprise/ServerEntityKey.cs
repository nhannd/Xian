#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
