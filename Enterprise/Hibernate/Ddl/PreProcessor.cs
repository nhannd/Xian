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
        public void Process(PersistentStore store)
        {
            // order is important

            // run the enum FK processor first
            new EnumForeignKeyConstraintCreator().Process(store);

            // run the base index creator
            new BaselineIndexCreator().Process(store);

            // run the additional index creator
            new AdditionalIndexCreator().Process(store);

            // run extension processors
            foreach (IDdlPreProcessor processor in new DdlPreProcessorExtensionPoint().CreateExtensions())
            {
                processor.Process(store);
            }
        }
    }
}
