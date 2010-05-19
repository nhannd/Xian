#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
			private string _namespaceSpec;

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
