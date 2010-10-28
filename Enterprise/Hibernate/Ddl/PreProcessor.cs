#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an extension point for pre-processing the NHibernate configuration prior to generating DDL scripts.
	/// </summary>
	[ExtensionPoint]
	public class DdlPreProcessorExtensionPoint : ExtensionPoint<IDdlPreProcessor>
	{
	}

	/// <summary>
	/// Pre-processes the NHibernate configuration prior to generating output.
	/// </summary>
	public class PreProcessor
	{
		private readonly bool _createIndexes;
		private readonly bool _autoIndexForeignKeys;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="createIndexes"></param>
		/// <param name="autoIndexForeignKeys"></param>
		public PreProcessor(bool createIndexes, bool autoIndexForeignKeys)
		{
			_createIndexes = createIndexes;
			_autoIndexForeignKeys = autoIndexForeignKeys;
		}

		/// <summary>
		/// Processes the specified persistent store.
		/// </summary>
		/// <param name="store"></param>
		public void Process(PersistentStore store)
		{
			Process(store.Configuration);
		}

		/// <summary>
		/// Processes the specified configuration.
		/// </summary>
		/// <param name="config"></param>
		public void Process(Configuration config)
		{
			// order in which processors are applied is important
			// because all FKs must be created prior to indexing

			// run the enum FK processor first
			Apply(config, new EnumForeignKeyProcessor());

			if (_createIndexes)
			{
				if (_autoIndexForeignKeys)
				{
					// run the fk index creator
					Apply(config, new ForeignKeyIndexProcessor());
				}

				// run the additional index creator
				Apply(config, new AdditionalIndexProcessor());
			}

			// run extension processors
			foreach (IDdlPreProcessor processor in new DdlPreProcessorExtensionPoint().CreateExtensions())
			{
				Apply(config, processor);
			}
		}

		private static void Apply(Configuration config, IDdlPreProcessor processor)
		{
			// it does not make sense to apply a given processor to the same configuration object
			// more than once
			// therefore we need to track whether a configuration object has been processed by a given processor
			// hopefully NH doesn't mind having extra keys stuck in the Configuration.Properties dictionary

			var key = "cc_custom_processor:" + processor.GetType().FullName;
			if(config.Properties.ContainsKey(key))
				return;

			// apply the processor
			processor.Process(config);

			// record that this processor has already been applied
			config.Properties[key] = "applied";
		}
	}
}
