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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides <see cref="IExceptionPolicy"/> objects via static <see cref="GetPolicy"/> method.
    ///</summary>
    internal class ExceptionPolicyFactory
    {
        private sealed class DefaultExceptionPolicy : IExceptionPolicy
        {
            public void Handle(Exception e, IExceptionHandlingContext context)
            {
                context.Log(LogLevel.Error, e);
                context.ShowMessageBox(e.Message, true);
            }
        }

    	private static readonly IDictionary<Type, IExceptionPolicy> _policies = CreatePolicies();
    	private static readonly IExceptionPolicy _defaultPolicy = new DefaultExceptionPolicy();

		private static IDictionary<Type, IExceptionPolicy> CreatePolicies()
        {
            var policies = new Dictionary<Type, IExceptionPolicy>();

        	try
        	{
				foreach (IExceptionPolicy policy in new ExceptionPolicyExtensionPoint().CreateExtensions())
        		{
        			foreach (ExceptionPolicyForAttribute attr in policy.GetType().GetCustomAttributes(typeof (ExceptionPolicyForAttribute), true))
        			{
        				if (!policies.ContainsKey(attr.ExceptionType))
        					policies[attr.ExceptionType] = policy;
        			}
        		}
        	}
			catch (NotSupportedException)
			{}
        	catch (Exception e)
        	{
        		Platform.Log(LogLevel.Debug, e);
        	}

			return policies;
        }

        ///<summary>
        /// Returns an <see cref="IExceptionPolicy"/> for a requested <see cref="Exception"/> type.
        ///</summary>
        ///<param name="exceptionType">An <see cref="Exception"/> derived type.</param>
        ///<returns>An <see cref="IExceptionPolicy"/> for the requested type if found or a <see cref="DefaultExceptionPolicy"/>.</returns>
        public static IExceptionPolicy GetPolicy(Type exceptionType)
        {
            IExceptionPolicy policy;
            if(!_policies.TryGetValue(exceptionType, out policy))
            	policy = _defaultPolicy;

            return policy;
        }
    }
}
