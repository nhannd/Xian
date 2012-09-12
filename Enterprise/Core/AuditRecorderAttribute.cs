#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// When applied to a service operation, specifies that the operation should be
    /// recorded in the audit log using the specified recorder class.
    /// </summary>
    /// <remarks>
    /// The auditor class must implement <see cref="IServiceOperationRecorder"/> and have
    /// a default constructor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuditRecorderAttribute : Attribute
    {
    	/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recorderClass"></param>
        public AuditRecorderAttribute(Type recorderClass)
        {
            RecorderClass = recorderClass;
        }

    	/// <summary>
    	/// Gets the implementation of <see cref="IServiceOperationRecorder"/> that
    	/// will create the audit log entry.
    	/// </summary>
    	internal Type RecorderClass { get; private set; }
    }
}
