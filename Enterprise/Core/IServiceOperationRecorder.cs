#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	public interface IServiceOperationRecorderContext
	{
		/// <summary>
		/// Gets the logical name of the operation.
		/// </summary>
		string OperationName { get; }

		/// <summary>
		/// Gets the class that provides the service implementation.
		/// </summary>
		Type ServiceClass { get; }

		/// <summary>
		/// Gets the <see cref="MethodInfo"/> object describing the operation.
		/// </summary>
		MethodInfo OperationMethodInfo { get; }

		/// <summary>
		/// Gets the request object passed to the operation.
		/// </summary>
		object Request { get; }

		/// <summary>
		/// Gets the response object returned from the operation, or null if an exception was thrown.
		/// </summary>
		object Response { get; }

		/// <summary>
		/// Gets the current change set.
		/// </summary>
		EntityChangeSet ChangeSet { get; }

		/// <summary>
		/// Writes the specified audit message, using the specified operation name.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="message"></param>
		void Write(string operation, string message);

		/// <summary>
		/// Writes the specified audit message, using the default operation name.
		/// </summary>
		/// <param name="message"></param>
		void Write(string message);
	}



	/// <summary>
	/// Defines an interface for writing an audit log entry that records
	/// information about the invocation of a service operation.
	/// </summary>
	public interface IServiceOperationRecorder
	{
		string Application { get; }

		string Category { get; }

		void PreCommit(IServiceOperationRecorderContext recorderContext);

		void PostCommit(IServiceOperationRecorderContext recorderContext);
	}
}
