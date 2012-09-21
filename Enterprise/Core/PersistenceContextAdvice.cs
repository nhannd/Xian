#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using Castle.DynamicProxy;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for honouring <see cref="ReadOperationAttribute"/> and <see cref="UpdateOperationAttribute"/>.
	/// </summary>
	public class PersistenceContextAdvice : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			var a = GetServiceOperationAttribute(invocation);
			if (a != null)
			{
				// persistence context required
				using (var scope = a.CreatePersistenceScope())
				{
					// configure change-set auditing
					ConfigureAuditing(PersistenceScope.CurrentContext, a, invocation);

					// proceed with invocation
					invocation.Proceed();

					// auto-commit transaction
					scope.Complete();
				}
			}
			else
			{
				// no persistence context required
				invocation.Proceed();
			}
		}

		protected ServiceOperationAttribute GetServiceOperationAttribute(IInvocation invocation)
		{
			return AttributeUtils.GetAttribute<ServiceOperationAttribute>(invocation.MethodInvocationTarget);
		}

		private static void ConfigureAuditing(IPersistenceContext context, ServiceOperationAttribute attribute, IInvocation invocation)
		{
			// if this is a read-context, there is no change set to audit
			var uctx = context as IUpdateContext;
			if (uctx == null)
				return;

			// if this operation is marked as not auditable, then explicitly
			// disable the change set recorder
			if (attribute.ChangeSetAuditable == false)
			{
				uctx.ChangeSetRecorder = null;
				return;
			}

			// if the current context has a change-set recorder installed
			// ensure that the ChangeSetRecorder.OperationName property is set appropriately
			// if the name is already set (by an outer service layer), don't change it
			if (uctx.ChangeSetRecorder != null && string.IsNullOrEmpty(uctx.ChangeSetRecorder.OperationName))
			{
				uctx.ChangeSetRecorder.OperationName = string.Format("{0}.{1}",
					invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);
			}
		}
	}
}
