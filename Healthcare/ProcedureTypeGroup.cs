#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ProcedureTypeGroup entity
    /// </summary>
	public partial class ProcedureTypeGroup : ClearCanvas.Enterprise.Core.Entity
	{
        /// <summary>
        /// Returns all concrete subclasses of this class.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IList<Type> ListSubClasses(IPersistenceContext context)
        {
            return CollectionUtils.Select(context.GetBroker<IMetadataBroker>().ListEntityClasses(),
                delegate(Type t) { return !t.IsAbstract && t.IsSubclassOf(typeof(ProcedureTypeGroup)); });
        }

        /// <summary>
        /// Gets the concrete subclass matching the specified name, which need not be fully qualified.
        /// </summary>
        /// <param name="subclassName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Type GetSubClass(string subclassName, IPersistenceContext context)
        {
            return CollectionUtils.SelectFirst(ListSubClasses(context),
				delegate(Type t) { return t.FullName.EndsWith(subclassName); });
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}