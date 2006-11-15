using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public class AccessionNumberBroker : Broker, IAccessionNumberBroker
    {
        private static string TABLE_NAME = "AccessionSequence_";
        private static string COLUMN_NAME = "NextValue_";
        private static long INITIAL_VALUE = 100000000;

        /// <summary>
        /// Extension to generate DDL to create and initialize the Accession Sequence table
        /// </summary>
        [ExtensionOf(typeof(DdlScriptGeneratorExtensionPoint))]
        public class AccessionSequenceTableGenerator : IDdlScriptGenerator
        {
            #region IDdlScriptGenerator Members

            public string[] GenerateCreateScripts(PersistentStore store, NHibernate.Dialect.Dialect dialect)
            {
                return new string[]
				{
                    string.Format("create table {0} ( {1} {2} );", TABLE_NAME, COLUMN_NAME, dialect.GetTypeName( NHibernate.SqlTypes.SqlTypeFactory.GetInt64() )),
					string.Format("insert into {0} values ( {1} )", TABLE_NAME, INITIAL_VALUE)
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
            // try to read the next accession number
            long accNum = 0;
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

            // update the sequence
            int updatedRows = 0;
            try
            {
                IDbCommand update = this.Context.CreateSqlCommand(string.Format("UPDATE {0} SET {1} = ? WHERE {2} = ?", TABLE_NAME, COLUMN_NAME, COLUMN_NAME));

                ((IDbDataParameter)update.Parameters[0]).Value = accNum + 1;
                ((IDbDataParameter)update.Parameters[1]).Value = accNum;

                updatedRows = update.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new PersistenceException(SR.ErrorFailedUpdateNextSequenceNumber, e);
            }

            if (updatedRows == 0)
            {
                throw new PersistenceException(SR.ErrorFailedUpdateNextSequenceNumber, null);
            }

            return accNum.ToString();
        }

        #endregion
    }
}
