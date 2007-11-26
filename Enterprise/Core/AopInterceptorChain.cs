using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    class AopInterceptorChain : IInterceptor
    {
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
                set { _rootInvocation.InvocationTarget = value; }
            }

            public System.Reflection.MethodInfo Method
            {
                get { return _rootInvocation.Method; }
            }

            public System.Reflection.MethodInfo MethodInvocationTarget
            {
                get { return _rootInvocation.MethodInvocationTarget; }
            }

            public object Proceed(params object[] args)
            {
                return _owner.NextInterceptor(_level, _rootInvocation, args);
            }

            public object Proxy
            {
                get { return _rootInvocation.Proxy; }
            }

            #endregion
        }


        private readonly IList<IInterceptor> _interceptors;

        public AopInterceptorChain(IList<IInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }


        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            return NextInterceptor(0, invocation, args);
        }

        #endregion

        private object NextInterceptor(int level, IInvocation rootInvocation, params object[] args)
        {
            if(level == _interceptors.Count)
            {
                return rootInvocation.Proceed(args);
            }
            else
            {
                IInvocation ii = new IntermediateInvocation(this, level+1, rootInvocation);
                return _interceptors[level].Intercept(ii, args);
            }
        }

    }
}
