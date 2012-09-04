#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using Castle.Core.Interceptor;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;


namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for honouring <see cref="AuditAttribute"/>s applied to service operation methods.
	/// </summary>
	public class AuditAdvice : IInterceptor
	{
		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			Exception exception = null;
			try
			{
				invocation.Proceed();
			}
			catch (Exception e)
			{
				exception = e;
				throw;
			}
			finally
			{
				var auditAttrs = AttributeUtils.GetAttributes<AuditAttribute>(invocation.MethodInvocationTarget, true);
				if (auditAttrs.Count > 0)
				{
					// inherit the current persistence scope, which should still be valid, or optionally create a new one
					using (var scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.Required))
					{
						var operationName =
							string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

						var info = new ServiceOperationInvocationInfo(
							operationName,
							invocation.InvocationTarget.GetType(),
							invocation.MethodInvocationTarget,
							invocation.Arguments.FirstOrDefault(),
							invocation.ReturnValue,
							exception);

						// multiple audit recorders may be specified for a given service operation
						foreach (var attr in auditAttrs)
						{
							try
							{
								Audit(attr, info);
							}
							catch (Exception e)
							{
								// audit operation failed - this is low-level, so we log directly to log file
								Platform.Log(LogLevel.Error, e);
							}
						}

						scope.Complete();
					}
				}
			}
		}

		private static void Audit(AuditAttribute attr, ServiceOperationInvocationInfo info)
		{
			// create an instance of the specified recorder class
			var recorder = (IServiceOperationRecorder)Activator.CreateInstance(attr.RecorderClass);

			// write to the audit log
			var log = new AuditLog(null, recorder.Category);
			recorder.WriteLogEntry(info, log);
		}

		#endregion
	}
}
