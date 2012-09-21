#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Core.Tests
{
	/// <summary>
	/// Class that mimicks the NH entity proxy mechanism.
	/// </summary>
	static class EntityProxyFactory
	{
		internal class EntityProxyInterceptor : IInterceptor
		{
			private readonly object _rawInstance;

			internal EntityProxyInterceptor(object rawInstance)
			{
				_rawInstance = rawInstance;
			}

			public bool Intercepted { get; private set; }

			void IInterceptor.Intercept(IInvocation invocation)
			{
				// record that an invocation was intercepted
				this.Intercepted = true;

				// forward the call
				invocation.ReturnValue = invocation.Method.Invoke(_rawInstance, invocation.Arguments);
			}
		}

		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		internal static TEntity CreateProxy<TEntity>(TEntity raw, out EntityProxyInterceptor interceptor)
		{
			interceptor = new EntityProxyInterceptor(raw);
			return (TEntity)_generator.CreateClassProxy(typeof(TEntity), interceptor);
		}

		internal static TEntity CreateProxy<TEntity>(TEntity raw)
		{
			EntityProxyInterceptor interceptor;
			return CreateProxy(raw, out interceptor);
		}
	}
}
