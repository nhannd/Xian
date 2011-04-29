#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Client-side advice to throttle repeated attempts at contacting an unreachable endpoint.
	/// </summary>
	/// <remarks>
	/// If the invocation fails due to a <see cref="EndpointNotFoundException"/>, this interceptor
	/// will block further calls to the same host/port for a period of time and throw another
	/// <see cref="EndpointNotFoundException"/>.  The idea is to avoid repeatedly calling an
	/// unreachable endpoint and having to endure waiting for the call to time-out each time.
	/// </remarks>
	class FailedEndpointThrottleClientAdvice : IInterceptor
	{
		#region Helper classes

		class EndpointKey : IEquatable<EndpointKey>
		{
			private readonly string _host;
			private readonly string _scheme;
			private readonly int _port;

			public EndpointKey(Uri uri)
			{
				_scheme = uri.Scheme;
				_host = uri.Host;
				_port = uri.Port;
			}

			public bool Equals(EndpointKey other)
			{
				if (ReferenceEquals(null, other))
					return false;
				return Equals(other._host, _host) && Equals(other._scheme, _scheme) && other._port == _port;
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as EndpointKey);
			}

			public override int GetHashCode()
			{
				return _host.GetHashCode() ^ _scheme.GetHashCode() ^ _port.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("{0}://{1}:{2}/", _scheme, _host, _port);
			}
		}

		class FailureInfo
		{
			public DateTime TimeOfLastFailedAttempt { get; set;}
		}

		#endregion


		private readonly Dictionary<EndpointKey, FailureInfo> _failures = new Dictionary<EndpointKey, FailureInfo>();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="throttleTime"></param>
		internal FailedEndpointThrottleClientAdvice(TimeSpan throttleTime)
		{
			this.ThrottleTime = throttleTime;
		}

		/// <summary>
		/// Gets or sets the throttle time, which is the minimum time that must elapse before attempting
		/// to contact a previously failed endpoint.
		/// </summary>
		public TimeSpan ThrottleTime { get; private set; }

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			var channel = (IClientChannel)invocation.InvocationTarget;
			var endpointKey = new EndpointKey(channel.RemoteAddress.Uri);

			// important to lock, because multiple threads may be using this instance
			lock (_failures)
			{
				// check if we've had a recent failure attempting to contact this endpoint
				// if so, just throw an exception before attempting to contact it
				FailureInfo failureInfo;
				if(_failures.TryGetValue(endpointKey, out failureInfo)
				 && (DateTime.Now - failureInfo.TimeOfLastFailedAttempt < ThrottleTime))
				{
					// TODO: this exception message is not exactly accurate... not sure what it should say really
					var msg = string.Format("Could not connect to {0}.", endpointKey);
					throw new EndpointNotFoundException(msg);
				}
			}

			try
			{
				invocation.Proceed();
			}
			catch (EndpointNotFoundException e)
			{
				Platform.Log(LogLevel.Debug, e, "Service unreachable");

				lock (_failures)
				{
					_failures[endpointKey] = new FailureInfo {TimeOfLastFailedAttempt = DateTime.Now};
				}

				throw;
			}
		}

		#endregion
	}
}
