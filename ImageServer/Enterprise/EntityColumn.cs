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
    /// Generic base class for update parameter classes used in a non-procedural update broker implementing the <see cref="IUpdateBroker"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the field to be updated</typeparam>
    public class EntityUpdateColumn<T> : EntityColumnBase
    {      
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldName">The update column name.</param>
        /// <param name="value">The value to update.</param>
        public EntityUpdateColumn(String fieldName, T value)
            : base(fieldName)
        {
            _value = value;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// The value of the column to update.
        /// </summary>
        public new T Value
        {
            get { return (T) _value; }
        }

        #endregion Public properties
    }
}
