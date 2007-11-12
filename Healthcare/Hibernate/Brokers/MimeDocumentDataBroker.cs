using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Enterprise.Hibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class MimeDocumentDataBroker
    {
        private static readonly string TABLE_NAME = "MimeDocumentData_";
        private static readonly string COLUMN_NAME = "BinaryData_";
        private static readonly string COLUMN_TYPE = "varbinary(max) not null";

        /// <summary>
        /// Extension to generate DDL to create and initialize the Accession Sequence table
        /// </summary>
        [ExtensionOf(typeof(DdlScriptGeneratorExtensionPoint))]
        public class MimeDocumentDataDdlScriptGenerator : IDdlScriptGenerator
        {
            #region IDdlScriptGenerator Members

            public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
            {
                string defaultSchema = store.Configuration.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
                string tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + TABLE_NAME : TABLE_NAME;

                return new string[]
				{
                    string.Format("alter table {0} alter column {1} {2}", tableName, COLUMN_NAME, COLUMN_TYPE)
				};
            }

            public string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
            {
                return new string[] { };    // nothing to do
            }

            #endregion
        }
    }
}
