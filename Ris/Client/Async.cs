#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides a simple mechanism for application components to execute code asynchronously.
	/// </summary>
	public static class Async
	{
		#region ComponentTaskManager class

		class ComponentTaskManager : IDisposable
		{
			private readonly IApplicationComponent _component;
			private readonly List<AsyncTask> _asyncTasks;

			public ComponentTaskManager(IApplicationComponent component)
			{
				_component = component;
				_component.Stopped += ComponentStoppedEventHandler;
				_asyncTasks = new List<AsyncTask>();
			}

			public void Invoke(
				AsyncTask.Action asyncCode,
				AsyncTask.Action successHandler,
				Action<Exception> errorHandler)
			{
				var task = new AsyncTask();
				_asyncTasks.Add(task);

				task.Run(asyncCode,
					delegate
					{
						_asyncTasks.Remove(task);
						task.Dispose();
						successHandler();
					},
					delegate(Exception e)
					{
						_asyncTasks.Remove(task);
						task.Dispose();
						errorHandler(e);
					});
			}

			public void Request<TServiceContract, TResponse>(
				Converter<TServiceContract, TResponse> asyncCode,
				Action<TResponse> successHandler,
				Action<Exception> errorHandler)
			{
				var response = default(TResponse);
				Invoke(
					delegate
					{
						Platform.GetService<TServiceContract>(
							service => response = asyncCode(service));
					},
					delegate
					{
						successHandler(response);
					},
					errorHandler);
			}

			public void CancelPending()
			{
				foreach (var task in _asyncTasks)
				{
					task.Dispose();
				}
				_asyncTasks.Clear();
			}

			public void Dispose()
			{
				CancelPending();
			}

			private void ComponentStoppedEventHandler(object source, EventArgs args)
			{
				CancelPending();
			}
		}

		#endregion

		private static readonly Dictionary<IApplicationComponent, ComponentTaskManager> _componentTaskManagers
			= new Dictionary<IApplicationComponent, ComponentTaskManager>();

		#region Public API

		/// <summary>
		/// Invokes an arbitrary block of code asynchronously, executing a continuation upon completion or error handler upon failure.
		/// </summary>
		/// <remarks>
		/// The invocation is tied to the lifetime of the specified application component.  That is, if the component is stopped, any
		/// asynchronous invocations pending completion will be discarded.
		/// </remarks>
		/// <param name="component"></param>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		/// <param name="errorHandler"></param>
		public static void Invoke(
			IApplicationComponent component,
			AsyncTask.Action asyncCode,
			AsyncTask.Action continuationCode,
			Action<Exception> errorHandler)
		{
			var ctm = GetComponentTaskManager(component);

			ctm.Invoke(asyncCode, continuationCode, errorHandler);
		}

		/// <summary>
		/// Invokes an arbitrary block of code asynchronously, executing a continuation upon completion.
		/// </summary>
		/// <remarks>
		/// The invocation is tied to the lifetime of the specified application component.  That is, if the component is stopped, any
		/// asynchronous invocations pending completion will be discarded.
		/// </remarks>
		/// <param name="component"></param>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		public static void Invoke(
			IApplicationComponent component,
			AsyncTask.Action asyncCode,
			AsyncTask.Action continuationCode)
		{
			var ctm = GetComponentTaskManager(component);

			ctm.Invoke(asyncCode, continuationCode, AsyncTask.DefaultErrorHandler);
		}

		/// <summary>
		/// Makes an asynchronous request, executing a continuation upon completion or error handler upon failure.
		/// </summary>
		/// <remarks>
		/// The request is tied to the lifetime of the specified application component.  That is, if the component is stopped, any
		/// asynchronous requests pending completion will be discarded.
		/// </remarks>
		/// <typeparam name="TServiceContract"></typeparam>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="component"></param>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		/// <param name="errorHandler"></param>
		public static void Request<TServiceContract, TResponse>(
			IApplicationComponent component,
			Converter<TServiceContract, TResponse> asyncCode,
			Action<TResponse> continuationCode,
			Action<Exception> errorHandler)
		{
			var ctm = GetComponentTaskManager(component);

			ctm.Request(asyncCode, continuationCode, errorHandler);
		}

		/// <summary>
		/// Makes an asynchronous request, executing a continuation upon completion.
		/// </summary>
		/// <remarks>
		/// The request is tied to the lifetime of the specified application component.  That is, if the component is stopped, any
		/// asynchronous requests pending completion will be discarded.
		/// </remarks>
		/// <typeparam name="TServiceContract"></typeparam>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="component"></param>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		public static void Request<TServiceContract, TResponse>(
			IApplicationComponent component,
			Converter<TServiceContract, TResponse> asyncCode,
			Action<TResponse> continuationCode)
		{
			var ctm = GetComponentTaskManager(component);

			ctm.Request(asyncCode, continuationCode, AsyncTask.DefaultErrorHandler);
		}

		/// <summary>
		/// Cancels any pending invocations or requests made by the specified application component.
		/// </summary>
		/// <param name="component"></param>
		public static void CancelPending(IApplicationComponent component)
		{
			ComponentTaskManager ctm;
			if (_componentTaskManagers.TryGetValue(component, out ctm))
			{
				ctm.CancelPending();
			}
		}

		#endregion

		#region Helpers

		private static ComponentTaskManager GetComponentTaskManager(IApplicationComponent component)
		{
			ComponentTaskManager taskManager;
			if (!_componentTaskManagers.TryGetValue(component, out taskManager))
			{
				taskManager = new ComponentTaskManager(component);
				_componentTaskManagers.Add(component, taskManager);
				component.Stopped += ((source, args) => _componentTaskManagers.Remove(component));
			}
			return taskManager;
		}

		#endregion
	}
}
