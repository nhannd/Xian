#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public class TableScripts
	{
		class Delta
		{
			public Delta(Configuration configuration)
			{
				From = new RelationalModelInfo(configuration);
				To = new RelationalModelInfo(configuration);
			}

			public RelationalModelInfo From { get; private set; }
			public RelationalModelInfo To { get; private set; }
		}

		private readonly Configuration _configuration;
		private readonly string _table;

		public TableScripts(Configuration configuration, string table, bool autoIndexForeignKeys)
		{
			_configuration = configuration;
			_table = table;

			// ensure that this configuration object has been pre-processed
			var preProcessor = new PreProcessor(true, autoIndexForeignKeys);
			preProcessor.Process(_configuration);
		}

		public string[] CreateTable(bool createIndexes, bool createConstraints)
		{
			return GetScripts(
				delta =>
				{
					CollectionUtils.Remove(delta.From.Tables, t => t.Name == _table);
					var ti = CollectionUtils.SelectFirst(delta.To.Tables, t => t.Name == _table);

					if(!createIndexes)
					{
						ti.Indexes.Clear();
						ti.PrimaryKey = null;
					}
					if(!createConstraints)
					{
						ti.UniqueKeys.Clear();
						ti.ForeignKeys.Clear();
					}
				});
		}

		public string[] AddIndexes()
		{
			return GetScripts(
				delta =>
				{
					var ti = CollectionUtils.SelectFirst(delta.From.Tables, t => t.Name == _table);
					ti.Indexes.Clear();
					ti.PrimaryKey = null;
				});
		}

		public string[] AddConstraints()
		{
			return GetScripts(
				delta =>
				{
					var ti = CollectionUtils.SelectFirst(delta.From.Tables, t => t.Name == _table);
					ti.UniqueKeys.Clear();
					ti.ForeignKeys.Clear();
				});
		}

		public string[] DropTable()
		{
			return GetScripts(
				delta => CollectionUtils.Remove(delta.To.Tables, t => t.Name == _table));
		}

		private string[] GetScripts(Action<Delta> action)
		{
			var delta = new Delta(_configuration);

			action(delta);

			var comparator = new RelationalModelComparator(EnumOptions.None);
			var transform = comparator.CompareModels(delta.From, delta.To);

			var renderer = Renderer.GetRenderer(_configuration);
			return CollectionUtils.Map(transform.Render(renderer, new RenderOptions()), (Statement s) => s.Sql).ToArray();
		}
	}
}
