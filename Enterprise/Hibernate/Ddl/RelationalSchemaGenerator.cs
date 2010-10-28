#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Cfg;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{


	public class RelationalSchemaOptions
	{
		public class NamespaceFilterOption
		{
			private readonly string _namespaceSpec;

			public NamespaceFilterOption()
			{
			}

			public NamespaceFilterOption(string namespaceSpec)
			{
				_namespaceSpec = namespaceSpec;
			}

			public bool Matches(string ns)
			{
				Platform.CheckForNullReference(ns, "ns");

				if(string.IsNullOrEmpty(_namespaceSpec))
					return true;

				return ns.StartsWith(_namespaceSpec);
			}
		}

		public RelationalSchemaOptions()
		{
			this.NamespaceFilter = new NamespaceFilterOption(null);
		}

		public EnumOptions EnumOption { get; set; }
		public bool SuppressForeignKeys { get; set; }
		public bool SuppressUniqueConstraints { get; set; }
		public bool SuppressIndexes { get; set; }
		public bool SuppressPrimaryKeys { get; set; }
		public NamespaceFilterOption NamespaceFilter { get; set; }
	}

	/// <summary>
	/// Generates scripts to create the tables, foreign key constraints, and indexes.
	/// </summary>
	class RelationalSchemaGenerator : DdlScriptGenerator
	{

		private readonly RelationalSchemaOptions _options;

		public RelationalSchemaGenerator(RelationalSchemaOptions options)
		{
			_options = options;
		}

		#region IDdlScriptGenerator Members

		public override string[] GenerateCreateScripts(Configuration config)
		{
			var currentModel = new RelationalModelInfo(config, _options.NamespaceFilter);
			var baselineModel = new RelationalModelInfo();		// baseline model is empty

			return GetScripts(config, baselineModel, currentModel);
		}

		public override string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel)
		{
			var currentModel = new RelationalModelInfo(config, _options.NamespaceFilter);

			return GetScripts(config, baselineModel, currentModel);
		}

		public override string[] GenerateDropScripts(Configuration config)
		{
			return new string[] { };
		}

		#endregion

		private string[] GetScripts(Configuration config, RelationalModelInfo baselineModel, RelationalModelInfo currentModel)
		{
			var comparator = new RelationalModelComparator(_options.EnumOption);
			var transform = comparator.CompareModels(baselineModel, currentModel);

			var renderer = Renderer.GetRenderer(config);
			return CollectionUtils.Map(transform.Render(renderer, new RenderOptions(_options)), (Statement s) => s.Sql).ToArray();
		}

	}
}
