#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{

	public static class WorkflowEventListener
	{
		public class PatientProfileInfo
		{
			public string Id { get; set; }
			public string GivenName { get; set; }
			public string FamilyName { get; set; }
			public DateTime? BirthDate { get; set; }
			public string Sex { get; set; }

			public override string ToString()
			{
				return String.Format("{0}: {1}, {2}", Id, FamilyName, GivenName);
			}
		}

		public abstract class WorkflowEventListenerArgs
		{
		}

		public class PerformedProcedureStepCompletedArgs : WorkflowEventListenerArgs
		{
			public PatientProfileInfo PatientProfile { get; set; }
			public DateTime CompletedTime { get; set; }
			public string AccessionNumber { get; set; }
			public string ProcedureType { get; set; }

			public override string ToString()
			{
				return String.Format("{0}: {1}, {2}", ProcedureType, AccessionNumber, CompletedTime);
			}

		}
	}

	public interface IWorkflowEventListener
	{
		void PerformedProcedureStepCompleted(WorkflowEventListener.PerformedProcedureStepCompletedArgs args);
	}

	[ExtensionPoint]
	public class WorkflowEventListenerExtensionPoint : ExtensionPoint<IWorkflowEventListener>
	{
	}

	public class WorkflowEventPublisher : IWorkflowEventListener
	{
		private static readonly WorkflowEventPublisher _instance = new WorkflowEventPublisher();

		/// <summary>
		/// Singleton static instance.
		/// </summary>
		public static WorkflowEventPublisher Instance
		{
			get { return _instance; }
		}


		private readonly object[] _listeners;

		private WorkflowEventPublisher()
		{
			_listeners = new WorkflowEventListenerExtensionPoint().CreateExtensions();
		}


		#region Implementation of IWorkflowEventListener

		public void PerformedProcedureStepCompleted(WorkflowEventListener.PerformedProcedureStepCompletedArgs args)
		{
			ForEachListener(listener => listener.PerformedProcedureStepCompleted(args));
		}

		#endregion

		private void ForEachListener(Action<IWorkflowEventListener> action)
		{
			lock(_listeners)
			{
				foreach (IWorkflowEventListener listener in _listeners)
				{
					try
					{
						action(listener);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}
		}
	}
}
