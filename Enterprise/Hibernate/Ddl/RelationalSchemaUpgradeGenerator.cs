using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	class RelationalSchemaUpgradeGenerator : IDdlScriptGenerator
	{
		private readonly RelationalModelInfo _upgradeFrom;

		public RelationalSchemaUpgradeGenerator(RelationalModelInfo upgradeFrom)
		{
			_upgradeFrom = upgradeFrom;
		}


		#region IDdlScriptGenerator Members

		public string[] GenerateCreateScripts(Configuration config, Dialect dialect)
		{
			RelationalModelInfo currentModel = new RelationalModelInfo(config, dialect);

			RelationalModelComparator comparator = new RelationalModelComparator();
			List<Change> changes = comparator.CompareDatabases(_upgradeFrom, currentModel);

			IRenderer renderer = Renderer.GetRenderer(dialect);

			List<Statement> statements = new List<Statement>();
			foreach (Change change in changes)
			{
				statements.AddRange(change.GetStatements(renderer));
			}

			return CollectionUtils.Map<Statement, string>(statements,
				delegate(Statement s) { return s.Sql; }).ToArray();
		}

		public string[] GenerateDropScripts(Configuration config, Dialect dialect)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
