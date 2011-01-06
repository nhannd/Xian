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
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Abstract base implementation of <see cref="IDdlScriptGenerator"/>.
	/// </summary>
	public abstract class DdlScriptGenerator : IDdlScriptGenerator
	{
		#region IDdlScriptGenerator Members

		public abstract string[] GenerateCreateScripts(Configuration config);

		public abstract string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel);

		public abstract string[] GenerateDropScripts(Configuration config);

		#endregion

		/// <summary>
		/// Gets the dialect object specified by the configuration.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		protected static Dialect GetDialect(Configuration config)
		{
			return Dialect.GetDialect(config.Properties);
		}
	}
}
