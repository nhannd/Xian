#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class to store select criteria for a broker implementing
    /// the <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    public abstract class EntitySelectCriteria : SearchCriteria
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityName">The name of the <see cref="ServerEntity"/> the criteria selects against.</param>
        public EntitySelectCriteria(string entityName)
            : base(entityName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntitySelectCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected EntitySelectCriteria(EntitySelectCriteria other)
            : base(other)
        {
        }

    	/// <summary>
    	/// 
    	/// </summary>
    	/// <returns></returns>
    	public override abstract object Clone();
    }
}
