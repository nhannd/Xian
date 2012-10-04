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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Owls.Brokers;
using System.Data.SqlClient;
using log4net;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IViewDdlBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ViewDdlBroker : Broker, IViewDdlBroker
	{
		#region IViewDdlBroker members

		/// <summary>
		/// Creates the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		public void CreateTable(Type viewItemClass, ViewDdlBrokerOptions options)
		{
			Execute(viewItemClass, options, ts => ts.CreateTable(false, false));
		}

		/// <summary>
		/// Adds indexes to the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		public void AddIndexes(Type viewItemClass, ViewDdlBrokerOptions options)
		{
			Execute(viewItemClass, options, ts => ts.AddIndexes());
		}

		/// <summary>
		/// Drops the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		public void DropTable(Type viewItemClass, ViewDdlBrokerOptions options)
		{
			Execute(viewItemClass, options, ts => ts.DropTable());
		}

		/// <summary>
		/// Drops indexes on the view table for the specified view item class.
		/// </summary>
		/// <param name="viewItemClass"></param>
		/// <param name="options"></param>
		public void DropIndexes(Type viewItemClass, ViewDdlBrokerOptions options)
		{
			// need to use a bit of hackery here to drop all of the indexes on the table, without knowing them by name
			// the SQL script in the embedded resource knows how to do this
			var table = ViewItemTableMapping.GetTableForClass(viewItemClass, this.Configuration);
			var template = ActiveTemplate.FromEmbeddedResource("drop_all_indexes_from_table.sql.template", new ResourceResolver(this.GetType().Assembly));
			var sql = template.Evaluate(new Dictionary<string, object> { { "tableName", table } });

			Execute(options, new[] { sql });
		}

		#endregion

		#region Helpers

		private void Execute(Type viewItemClass, ViewDdlBrokerOptions options, Converter<TableScripts, string[]> scriptAction)
		{
			var table = ViewItemTableMapping.GetTableForClass(viewItemClass, this.Configuration);
			var tableScripts = new TableScripts(this.Configuration, table, true);
			var scripts = scriptAction(tableScripts);

			Execute(options, scripts);
		}

		private void Execute(ViewDdlBrokerOptions options, IEnumerable<string> scripts)
		{
			// note: this implementation is highly specific to MS-SQL server
			// unfortunately we have no choice but to break the DB abstraction layer here,
			// because we potentially need an elevated connection string to be able to drop and create tables
			var connString = options.ElevatedConnectionString ?? this.ConnectionString;
			using(var connection = new SqlConnection(connString))
			{
				connection.Open();
				foreach (var script in scripts)
				{
					// direct this to the NH SQL logger
					var log = LogManager.GetLogger("NHibernate.SQL");
					log.Debug(script);

					// set timeout to one hour - basically, we don't care how long this takes
					var command = new SqlCommand(script, connection) {CommandTimeout = 3600};
					command.ExecuteNonQuery();
				}
			}
		}

		#endregion
	}
}
