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
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    public class AopInterceptorChain : IInterceptor
    {
        #region IntermediateInvocation

        class IntermediateInvocation : IInvocation
        {
            private readonly AopInterceptorChain _owner;
            private readonly IInvocation _rootInvocation;
            private readonly int _level;

            internal IntermediateInvocation(AopInterceptorChain owner, int level, IInvocation rootInvocation)
            {
                _owner = owner;
                _rootInvocation = rootInvocation;
                _level = level;
            }

            #region IInvocation Members

            public object InvocationTarget
            {
                get { return _rootInvocation.InvocationTarget; }
            }

            public Type TargetType
            {
                get { return _rootInvocation.TargetType; }
            }

            public object[] Arguments
            {
                get { return _rootInvocation.Arguments; }
            }

            public Type[] GenericArguments
            {
                get { return _rootInvocation.GenericArguments; }
            }

            public MethodInfo Method
            {
                get { return _rootInvocation.Method; }
            }

            public MethodInfo MethodInvocationTarget
            {
                get { return _rootInvocation.MethodInvocationTarget; }
            }

            public object ReturnValue
            {
                get { return _rootInvocation.ReturnValue; }
                set { _rootInvocation.ReturnValue = value; }
            }

            public void Proceed()
            {
                _owner.NextInterceptor(_level, _rootInvocation);
            }

            public void SetArgumentValue(int index, object value)
            {
                _rootInvocation.SetArgumentValue(index, value);
            }

            public object GetArgumentValue(int index)
            {
                return _rootInvocation.GetArgumentValue(index);
            }

            public MethodInfo GetConcreteMethod()
            {
                return _rootInvocation.GetConcreteMethod();
            }

            public MethodInfo GetConcreteMethodInvocationTarget()
            {
                return _rootInvocation.GetConcreteMethodInvocationTarget();
            }

            public object Proxy
            {
                get { return _rootInvocation.Proxy; }
            }

            #endregion
        }

        #endregion

        private readonly IList<IInterceptor> _interceptors;

        public AopInterceptorChain(IList<IInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }


        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            NextInterceptor(0, invocation);
        }

        #endregion

        private void NextInterceptor(int level, IInvocation rootInvocation)
        {
            if(level == _interceptors.Count)
            {
                // Proceed normally
                rootInvocation.Proceed();
            }
            else
            {
                IInvocation ii = new IntermediateInvocation(this, level+1, rootInvocation);
                _interceptors[level].Intercept(ii);
            }
        }

    }
}
