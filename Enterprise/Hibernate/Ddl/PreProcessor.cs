using System;
using System.Collections.Generic;
using System.Text;
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
		/// Processes the specified configuration.
		/// </summary>
		/// <param name="config"></param>
		public void Process(Configuration config)
        {
            // order is important

            // run the enum FK processor first
			new EnumForeignKeyProcessor().Process(config);

            if(_createIndexes)
            {
                if (_autoIndexForeignKeys)
                {
                    // run the fk index creator
					new ForeignKeyIndexProcessor().Process(config);
                }

                // run the additional index creator
				new AdditionalIndexProcessor().Process(config);
            }

            // run extension processors
            foreach (IDdlPreProcessor processor in new DdlPreProcessorExtensionPoint().CreateExtensions())
            {
				processor.Process(config);
            }
        }
    }
}
