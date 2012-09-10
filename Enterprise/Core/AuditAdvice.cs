#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;


namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for honouring <see cref="AuditAttribute"/>s applied to service operation methods.
	/// </summary>
	public class AuditAdvice : IInterceptor
	{
		#region RecorderContext class

		/// <summary>
		/// Implementation of <see cref="IServiceOperationRecorderContext"/>
		/// </summary>
		class RecorderContext : IServiceOperationRecorderContext
		{
			private readonly IInvocation _invocation;
			private readonly IServiceOperationRecorder _recorder;
			private readonly AuditLog _auditLog;
			private readonly string _operationName;
			private EntityChangeSet _changeSet;

			internal RecorderContext(IInvocation invocation, IServiceOperationRecorder recorder)
			{
				_invocation = invocation;
				_recorder = recorder;
				_auditLog = new AuditLog(_recorder.Application, _recorder.Category);
				_operationName = string.Format("{0}.{1}", _invocation.InvocationTarget.GetType().FullName, _invocation.Method.Name);
			}

			string IServiceOperationRecorderContext.OperationName
			{
				get { return _operationName; }
			}

			Type IServiceOperationRecorderContext.ServiceClass
			{
				get { return _invocation.InvocationTarget.GetType(); }
			}

			MethodInfo IServiceOperationRecorderContext.OperationMethodInfo
			{
				get { return _invocation.MethodInvocationTarget; }
			}

			object IServiceOperationRecorderContext.Request
			{
				get { return _invocation.Arguments.FirstOrDefault(); }
			}

			object IServiceOperationRecorderContext.Response
			{
				get { return _invocation.ReturnValue; }
			}

			void IServiceOperationRecorderContext.Write(string operation, string message)
			{
				_auditLog.WriteEntry(operation ?? _operationName, message);
			}

			void IServiceOperationRecorderContext.Write(string message)
			{
				_auditLog.WriteEntry(_operationName, message);
			}

			EntityChangeSet IServiceOperationRecorderContext.ChangeSet
			{
				get { return _changeSet; }
			}

			internal void PreCommit(EntityChangeSet changeSet)
			{
				try
				{
					_changeSet = changeSet;
					_recorder.PreCommit(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			internal void PostCommit()
			{
				try
				{
					_recorder.PreCommit(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		#endregion

		#region InvocationInfo class 

		class InvocationInfo
		{
			private readonly List<RecorderContext> _recorderContexts;

			public InvocationInfo(List<RecorderContext> recorderContexts)
			{
				_recorderContexts = recorderContexts;
			}

			internal void PreCommit(EntityChangeSet changeSet)
			{
				foreach (var recorderContext in _recorderContexts)
				{
					recorderContext.PreCommit(changeSet);
				}
			}

			internal void PostCommit()
			{
				foreach (var recorderContext in _recorderContexts)
				{
					recorderContext.PostCommit();
				}
			}
		}

		#endregion

		#region ChangeSetListener class

		[ExtensionOf(typeof(EntityChangeSetListenerExtensionPoint))]
		public class ChangeSetListener: IEntityChangeSetListener
		{
			public void PreCommit(EntityChangeSetPreCommitArgs args)
			{
				// store a copy of the change set for use by recorders
				if (_invocationInfo == null)
					return;

				_invocationInfo.PreCommit(args.ChangeSet);
			}

			public void PostCommit(EntityChangeSetPostCommitArgs args)
			{
			}
		}

		#endregion

		/// <summary>
		/// Keep track of the invocation that is happening on the current thread.
		/// </summary>
		[ThreadStatic]
		private static InvocationInfo _invocationInfo;

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			try
			{
				var recorderContexts = AttributeUtils.GetAttributes<AuditAttribute>(invocation.MethodInvocationTarget, true)
										.Select(a => new RecorderContext(invocation, (IServiceOperationRecorder)Activator.CreateInstance(a.RecorderClass)))
										.ToList();

				_invocationInfo = new InvocationInfo(recorderContexts);

				invocation.Proceed();

				_invocationInfo.PostCommit();
			}
			finally
			{
				// clear current invocation
				_invocationInfo = null;
			}
		}

		#endregion
	}
}
