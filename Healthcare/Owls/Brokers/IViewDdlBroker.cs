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

namespace ClearCanvas.Healthcare.Owls.Brokers
{
	/// <summary>
	/// Specifies options to methods on <see cref="IViewDdlBroker"/>.
	/// </summary>
	public class ViewDdlBrokerOptions
	{
		/// <summary>
		/// Gets or sets an elevated connection string, that must have sufficient permissions to drop and create view tables and indexes.
		/// </summary>
		public string ElevatedConnectionString { get; set; }
	}

	/// <summary>
	/// Defines an interface to a broker that provides methods for creating and dropping view tables and indexes.
	/// </summary>
	public interface IViewDdlBroker : IPersistenceBroker
	{
		/// <summary>
		/// Creates the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		void CreateTable(Type viewItemClass, ViewDdlBrokerOptions options);

		/// <summary>
		/// Adds indexes to the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		void AddIndexes(Type viewItemClass, ViewDdlBrokerOptions options);

		/// <summary>
		/// Drops the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		void DropTable(Type viewItemClass, ViewDdlBrokerOptions options);

		/// <summary>
		/// Drops indexes on the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		void DropIndexes(Type viewItemClass, ViewDdlBrokerOptions options);
	}
}
