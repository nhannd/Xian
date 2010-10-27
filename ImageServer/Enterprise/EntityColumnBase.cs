#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    /// Abstract base class for doing inserts and updates with the
    /// <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    public abstract class EntityColumnBase
    {
        #region Protected Members

        protected string _fieldName;
        protected object _value;

        #endregion  Protected Members

        #region Public Properties

        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _fieldName; }
        }

        /// <summary>
        /// Gets the value of the parameter/field.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fieldName">The column name.</param>
        protected EntityColumnBase(String fieldName)
        {
            _fieldName = fieldName;
        }

        #endregion Constructors
    }
}
