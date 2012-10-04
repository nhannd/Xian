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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Owls.Brokers
{

	/// <summary>
	/// Arguments for the <see cref="IViewSourceBroker.PopulateView"/> method.
	/// </summary>
	public class PopulateViewArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="rootEntityClass">Root entity class from which view items are derived.</param>
		/// <param name="inclusionPredicate">Predicate on the root entity class that determine whether a given root entity instance is included in the view.</param>
		public PopulateViewArgs(Type rootEntityClass, ISearchPredicate inclusionPredicate)
		{
			this.RootEntityClass = rootEntityClass;
			this.ViewInclusionPredicate = inclusionPredicate;

			// set some reasonable defaults for optional params
			this.ReadBatchSize = 100;
			this.WriteBatchSize = 100;
			this.Timeout = TimeSpan.Zero;
			this.ProgressCallback = delegate { };
		}

		/// <summary>
		/// Gets the root entity class from which the view is derived.
		/// </summary>
		public Type RootEntityClass { get; private set; }

		/// <summary>
		/// Gets the inclusion predicate, which determines whether a given instance of the root entity is to be included in the view.
		/// </summary>
		public ISearchPredicate ViewInclusionPredicate { get; private set; }

		/// <summary>
		/// Gets or sets the batch size of items to read from source data.
		/// </summary>
		public int ReadBatchSize { get; set; }

		/// <summary>
		/// Gets or sets the batch size of items to write to view.
		/// </summary>
		public int WriteBatchSize { get; set; }

		/// <summary>
		/// Gets or sets the callback function to receive progress updates.
		/// </summary>
		public Action<PopulateViewProgress> ProgressCallback { get; set; }

		/// <summary>
		/// Gets or sets the time-out for the view population.
		/// </summary>
		public TimeSpan Timeout { get; set; }
	}

	/// <summary>
	/// Represents a view population progress report.
	/// </summary>
	public class PopulateViewProgress
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="processedItemCount"></param>
		public PopulateViewProgress(long processedItemCount)
		{
			this.ProcessedItemCount = processedItemCount;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		public PopulateViewProgress(string message)
		{
			this.Message = message;
		}

		/// <summary>
		/// Gets the number of items processsed.
		/// </summary>
		public long ProcessedItemCount { get; private set; }

		/// <summary>
		/// Gets the progress message.
		/// </summary>
		public string Message { get; private set; }
	}

	/// <summary>
	/// Defines an interface to a broker that is used to populate and update views from source data.
	/// </summary>
	public interface IViewSourceBroker : IPersistenceBroker
	{
		/// <summary>
		/// Populates the view from source data, as a single long-running (non-transactional) step.
		/// </summary>
		/// <param name="args"></param>
		void PopulateView(PopulateViewArgs args);
	}

	/// <summary>
	/// Defines an interface to a broker that is used to build and update views from source data.
	/// </summary>
	/// <typeparam name="TViewItem">View item class.</typeparam>
	/// <typeparam name="TRootEntity">Class of the root entity from which the view is derived.</typeparam>
	public interface IViewSourceBroker<TViewItem, TRootEntity> : IViewSourceBroker
		where TViewItem : Entity
		where TRootEntity : Entity
	{
		/// <summary>
		/// Creates view items from source data for the specified root entity instance, used to incrementally update the view.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		IList<TViewItem> GetViewItems(TRootEntity instance);
	}
}
