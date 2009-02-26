using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public abstract class DdlScriptGenerator : IDdlScriptGenerator
	{
		#region IDdlScriptGenerator Members

		public abstract string[] GenerateCreateScripts(Configuration config);

		public abstract string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel);

		public abstract string[] GenerateDropScripts(Configuration config);

		#endregion

		protected static Dialect GetDialect(Configuration config)
		{
			return Dialect.GetDialect(config.Properties);
		}
	}
}
