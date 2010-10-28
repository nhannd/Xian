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
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ServiceOperationAttribute : Attribute
    {
        private PersistenceScopeOption _scopeOption;
        private bool _changeSetAuditable = true;
        
        public ServiceOperationAttribute()
        {
            // a persistence context is required, by default
            _scopeOption = PersistenceScopeOption.Required;
        }

		/// <summary>
		/// Gets or sets a value indicating whether change-set auditing is applied to this operation.
		/// Does not affect other levels of auditing.
		/// </summary>
        public bool ChangeSetAuditable
        {
            get { return _changeSetAuditable; }
            set { _changeSetAuditable = value; }
        }

		/// <summary>
		/// Gets or sets the <see cref="PersistenceScopeOption"/> to apply to this operation.
		/// </summary>
        public PersistenceScopeOption PersistenceScopeOption
        {
            get { return _scopeOption; }
            set { _scopeOption = value; }
        }

        public abstract PersistenceScope CreatePersistenceScope();
   }
}
