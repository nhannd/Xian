using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class AccessionNumberBroker : Broker, IAccessionNumberBroker
    {
        private static readonly string TABLE_NAME = "AccessionSequence_";
        private static readonly string COLUMN_NAME = "NextValue_";
        private static readonly long INITIAL_VALUE = 100000000;

        /// <summary>
        /// Extension to generate DDL to create and initialize the Accession Sequence table
        /// </summary>
        [ExtensionOf(typeof(DdlScriptGeneratorExtensionPoint))]
        public class AccessionSequenceDdlScriptGenerator : IDdlScriptGenerator
        {
            #region IDdlScriptGenerator Members

            public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
            {
                string defaultSchema = store.Configuration.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
                string tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + TABLE_NAME : TABLE_NAME;
                    
                return new string[]
				{
                    string.Format("create table {0} ( {1} {2} );", tableName, COLUMN_NAME, dialect.GetTypeName( NHibernate.SqlTypes.SqlTypeFactory.GetInt64() )),
					string.Format("insert into {0} values ( {1} )", tableName, INITIAL_VALUE)
				};
            }

            public string[] GenerateDropScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
            {
                return new string[] { dialect.GetDropTableString(TABLE_NAME) };
            }

            #endregion
        }



        #region IAccessionNumberBroker Members

        public string GetNextAccessionNumber()
        {
            int updatedRows = 0;
            long accNum = 0;

            // the loop is necessary to ensure that we succeed in obtaining an accession number
            // It is possible that another process is trying to do this at the same time,
            // hence there is an inevitable race condition which may cause the operation to occassionally fail
            // can we avoid the need for a loop by using Serializable transaction isolation for this operation???
            do
            {
                // try to read the next accession number
                try
                {
                    IDbCommand select = this.Context.CreateSqlCommand(string.Format("SELECT * from {0}", TABLE_NAME));
                    accNum = (long)select.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
                }

                if (accNum == 0)
                {
                    throw new HealthcareWorkflowException(SR.ErrorSequenceNotInitialized);
                }

                // update the sequence, by trying to update a row containing the previous number
                // this may fail if another process has updated in the meantime, in which case
                // the loop will just try again
                try
                {
                    string updateSql = string.Format("UPDATE {0} SET {1} = @next WHERE {2} = @prev", TABLE_NAME, COLUMN_NAME, COLUMN_NAME);
                    IDbCommand update = this.Context.CreateSqlCommand(updateSql);
                    AddParameter(update, "next", accNum + 1);
                    AddParameter(update, "prev", accNum);

                    updatedRows = update.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new PersistenceException(SR.ErrorFailedUpdateNextSequenceNumber, e);
                }
            }
            while (updatedRows == 0);

            return accNum.ToString();
        }

        private void AddParameter(IDbCommand cmd, string name, object value)
        {
            IDbDataParameter p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

        #endregion
    }
}
