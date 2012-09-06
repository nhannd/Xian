#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines the possible service interception sites.
	/// </summary>
	public enum ServiceInterceptSite
	{
		/// <summary>
		/// Server-side interception.
		/// </summary>
		Server,

		/// <summary>
		/// Client-side interception.
		/// </summary>
		Client
	}

	/// <summary>
	/// Defines an interface to an object that provides additional service interceptors.
	/// </summary>
	public interface IAdditionalServiceInterceptorProvider
	{
		/// <summary>
		/// Obtains a new instance of each additional interceptor.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		/// <remarks>
		/// Note that implementors have absolutely no control over where the additional interceptors
		/// are placed within the interception chain.  Therefore, additional interceptors must not
		/// rely in any way on being inserted before or after any of the default interceptors.
		/// </remarks>
		IList<IInterceptor> GetInterceptors(ServiceInterceptSite site);
	}

	/// <summary>
	/// Defines an extension point for providing additional service interceptors.
	/// </summary>
	[ExtensionPoint]
	public class AdditionalServiceInterceptorProviderExtensionPoint : ExtensionPoint<IAdditionalServiceInterceptorProvider>
	{
	}

	/// <summary>
	/// Utility class for instantiating additional service interceptors.
	/// </summary>
	public static class AdditionalServiceInterceptorProvider
	{
		/// <summary>
		/// Obtains a new instance of each additional interceptor.
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public static IList<IInterceptor> GetInterceptors(ServiceInterceptSite site)
		{
			return new AdditionalServiceInterceptorProviderExtensionPoint().CreateExtensions()
				.Cast<IAdditionalServiceInterceptorProvider>()
				.SelectMany(p => p.GetInterceptors(site))
				.ToList();
		}
	}
}
