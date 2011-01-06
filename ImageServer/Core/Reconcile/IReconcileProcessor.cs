#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Defines the interface of processors that processes different types of 'ReconcileStudy' entries
	/// </summary>
	public interface IReconcileProcessor : IDisposable
	{
		/// <summary>
		/// Gets the name of the processor
		/// </summary>
		String Name { get; }

		/// <summary>
		/// Gets the reason why <see cref="Execute"/> failed.
		/// </summary>
		String FailureReason { get;}

		/// <summary>
		/// Initializes the processor with the specified context
		/// </summary>
		/// <param name="context"></param>
		/// <param name="complete"></param>
		void Initialize(ReconcileStudyProcessorContext context, bool complete);

		/// <summary>
		/// Executes the processor
		/// </summary>
		/// <returns></returns>
		bool Execute();
	}
}
