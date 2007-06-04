using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class ExceptionPolicyFactory
    {
        private static readonly Object _policyLock = new Object();
        private static IDictionary<Type, IExceptionPolicy> _policies;
        private static readonly Type _noMatchingPolicyKey = typeof(NoMatchingPolicyKey);

        private sealed class NoMatchingPolicyKey
        {
        }

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

                            //Type faultWrappedExceptionType = typeof(FaultException<>);
                            //faultWrappedExceptionType = faultWrappedExceptionType.MakeGenericType(attr.ExceptionType);

                            //_policies.Add(new KeyValuePair<Type, IExceptionPolicy>(faultWrappedExceptionType, policy));
                        }
                    }
                }

            }
            finally
            {
                _policies.Add(new KeyValuePair<Type, IExceptionPolicy>(_noMatchingPolicyKey, new DefaultExceptionPolicy()));
            }
        }

        public static IExceptionPolicy GetPolicy(Type exceptionType)
        {
            IExceptionPolicy policy = null;

            try
            {
                policy = Policies[exceptionType];
            }
            catch (KeyNotFoundException e)
            {
                policy = Policies[_noMatchingPolicyKey];
            }

            return policy;
        }
    }

    internal sealed class DefaultExceptionPolicy : ExceptionPolicyBase
    {
    }
}
