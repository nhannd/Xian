#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

        private sealed class DefaultExceptionPolicyKey
        {
        }

        internal sealed class DefaultExceptionPolicy : IExceptionPolicy
        {
            public void Handle(Exception e, IExceptionHandlingContext context)
            {
                context.Log(LogLevel.Error, e);
                context.ShowMessageBox(e.Message, true);
            }
        }

        private static readonly Object _policyLock = new Object();
        private static IDictionary<Type, IExceptionPolicy> _policies;
        private static readonly Type _defaultExceptionPolicyKey = typeof(DefaultExceptionPolicyKey);

        // Initialise policies once
        private static IDictionary<Type, IExceptionPolicy> Policies
        {
            get
            {
                if (_policies == null)
                {
                    lock (_policyLock)
                    {
                        if (_policies == null)
                        {
                            InitialisePolicies();
                        }
                    }
                }
                return _policies;
            }
        }

        // Add all ExceptionPolicyExtensionPoint extensions as well as a default policy
        private static void InitialisePolicies()
        {
            _policies = new Dictionary<Type, IExceptionPolicy>();

            try
            {
                ExceptionPolicyExtensionPoint xp = new ExceptionPolicyExtensionPoint();
                Object[] extensions = xp.CreateExtensions();
                foreach (object extension in extensions)
                {
                    IExceptionPolicy policy = extension as IExceptionPolicy;
                    if (policy != null)
                    {
                        foreach (ExceptionPolicyForAttribute attr in policy.GetType().GetCustomAttributes(typeof(ExceptionPolicyForAttribute), true))
                        {
                            _policies.Add(new KeyValuePair<Type, IExceptionPolicy>(attr.ExceptionType, policy));
                        }
                    }
                }
            }
            finally
            {
                _policies.Add(new KeyValuePair<Type, IExceptionPolicy>(_defaultExceptionPolicyKey, new DefaultExceptionPolicy()));
            }
        }

        ///<summary>
        /// Returns an <see cref="IExceptionPolicy"/> for a requested <see cref="Exception"/> type.
        ///</summary>
        ///<param name="exceptionType">An <see cref="Exception"/> derived type.</param>
        ///<returns>An <see cref="IExceptionPolicy"/> for the requested type if found or a <see cref="DefaultExceptionPolicy"/>.</returns>
        public static IExceptionPolicy GetPolicy(Type exceptionType)
        {
            IExceptionPolicy policy;

            if(Policies.TryGetValue(exceptionType, out policy) == false)
            {
                policy = Policies[_defaultExceptionPolicyKey];
            }

            return policy;
        }
    }
}
