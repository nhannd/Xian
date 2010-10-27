#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
	/// Extension point for <see cref="IExceptionPolicy"/>s.
    ///</summary>
    [ExtensionPoint]
    public sealed class ExceptionPolicyExtensionPoint : ExtensionPoint<IExceptionPolicy> { }

    /// <summary>
    /// Provides Exception specific handling policies.
    /// </summary>
    /// <example>
    /// <code>
    /// [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    /// [ExceptionPolicyFor(typeof(FooException))]
    /// public class FooExceptionPolicy : IExceptionPolicy
    /// {
    ///     ...
    /// }
    /// </code>
    /// </example>
    public interface IExceptionPolicy
    {
        ///<summary>
        /// Handles the specified exception.
        ///</summary>
        void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext);
    }

    ///<summary>
    /// Specifies an exception type to which an <see cref="IExceptionPolicy"/> applies.
    ///</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExceptionPolicyForAttribute : Attribute
    {
        private readonly Type _exceptionType;

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="exceptionType">The type of exception the policy is for.</param>
        public ExceptionPolicyForAttribute(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        ///<summary>
        /// Gets the type of exception the policy is for.
        ///</summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
        }
    }
}

