#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class to store a collection of update columns to be used in in a 
    /// non-procedural broker implementing the 
    /// <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    /// <remark>
    /// Each updatable field in the parameter collection is an element of <see cref="SubParameters"/>.
    /// </remark>
    abstract public class EntityUpdateColumns
    {
        #region Private Members

        private readonly string _entityName;       
        private readonly Dictionary<string, EntityColumnBase> _subParameters = new Dictionary<string, EntityColumnBase>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityName">The name of the <see cref="ServerEntity"/>.</param>
        public EntityUpdateColumns(string entityName)
        {
            _entityName = entityName;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Returns the list of sub-parameters
        /// </summary>
        public IDictionary<string, EntityColumnBase> SubParameters
        {
            get { return _subParameters; }
        }
        
        public virtual bool IsEmpty
        {
            get
            {
                if (_subParameters.Values.Count > 0)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _entityName; }
        }

        #endregion
    }
}
