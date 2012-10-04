#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Owls.Brokers;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Arguments class for <see cref="IViewBuilder.CreateBuildActions"/>.
	/// </summary>
	public class CreateBuildActionsArgs
	{
		/// <summary>
		/// Gets or sets the action that the builder is to carry out.
		/// Only <see cref="UtilityAction.Build"/> and <see cref="UtilityAction.Reindex"/> are valid.
		/// </summary>
		public UtilityAction Action { get; set; }

		/// <summary>
		/// Gets or sets the read batch size for populating the view (size of batch of items read from source).
		/// </summary>
		public int ReadBatchSize { get; set; }

		/// <summary>
		/// Gets or sets the write batch size for populating the view (size of batch of view items written to view).
		/// </summary>
		public int WriteBatchSize { get; set; }

		/// <summary>
		/// Gets or sets an elevated connection string, providing the builder with sufficient privileges
		/// to drop and re-create view tables and indexes.
		/// </summary>
		public string ElevatedConnectionString { get; set; }
	}

	/// <summary>
	/// Defines an interface to an object that represent a step in a view building process.
	/// </summary>
	public interface IViewBuildAction
	{
		/// <summary>
		/// Executes this action.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="progressCallback"></param>
		void Execute(IPersistenceContext context, Action<PopulateViewProgress> progressCallback);
	}

	/// <summary>
	/// Defines an interface to a class that knows how to build a view from scratch.
	/// </summary>
	public interface IViewBuilder
	{
		/// <summary>
		/// Creates a set of build actions to build the view.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		IViewBuildAction[] CreateBuildActions(CreateBuildActionsArgs args);
	}
}
