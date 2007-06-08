using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides <see cref="IExceptionPolicy"/> objects via static <see cref="GetPolicy"/> method
    ///</summary>
    public class ExceptionPolicyFactory
    {

        private sealed class DefaultExceptionPolicyKey
        {
        }

        internal sealed class DefaultExceptionPolicy : IExceptionPolicy
        {
            public void Handle(Exception e, IExceptionHandlingContext context)
            {
                context.Log(e);
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
        /// Returns an <see cref="IExceptionPolicy"/> for a requested <see cref="Exception"/> type
        ///</summary>
        ///<param name="exceptionType">An <see cref="Exception"/> derived type</param>
        ///<returns>An <see cref="IExceptionPolicy"/> for the requested type if found or a <see cref="DefaultExceptionPolicy"/></returns>
        public static IExceptionPolicy GetPolicy(Type exceptionType)
        {
            IExceptionPolicy policy;

            try
            {
                policy = Policies[exceptionType];
            }
            catch (KeyNotFoundException)
            {
                policy = Policies[_defaultExceptionPolicyKey];
            }

            return policy;
        }
    }
}
