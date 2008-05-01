using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Defines an extension point for pre-processing the NHibernate model prior to generating DDL scripts.
    /// </summary>
    [ExtensionPoint]
    public class DdlPreProcessorExtensionPoint : ExtensionPoint<IDdlPreProcessor>
    {
    }


    public class PreProcessor
    {
        private readonly bool _createIndexes;
        private readonly bool _autoIndexForeignKeys;


        public PreProcessor(bool createIndexes, bool autoIndexForeignKeys)
        {
            _createIndexes = createIndexes;
            _autoIndexForeignKeys = autoIndexForeignKeys;
        }

        public void Process(PersistentStore store)
        {
            // order is important

            // run the enum FK processor first
            new EnumForeignKeyProcessor().Process(store);

            if(_createIndexes)
            {
                if (_autoIndexForeignKeys)
                {
                    // run the fk index creator
                    new ForeignKeyIndexProcessor().Process(store);
                }

                // run the additional index creator
                new AdditionalIndexProcessor().Process(store);
            }

            // run extension processors
            foreach (IDdlPreProcessor processor in new DdlPreProcessorExtensionPoint().CreateExtensions())
            {
                processor.Process(store);
            }
        }
    }
}
