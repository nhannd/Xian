#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Enterprise.Core;
using NHibernate.Cfg;
using NHibernate.Dialect;

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
        public class AccessionSequenceDdlScriptGenerator : DdlScriptGenerator
        {
            #region IDdlScriptGenerator Members

            public override string[] GenerateCreateScripts(Configuration config)
            {
                string defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
                string tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + TABLE_NAME : TABLE_NAME;
                    
                return new string[]
				{
                    string.Format("create table {0} ( {1} {2} );", tableName, COLUMN_NAME, GetDialect(config).GetTypeName( NHibernate.SqlTypes.SqlTypeFactory.Int64 )),
					string.Format("insert into {0} values ( {1} )", tableName, INITIAL_VALUE)
				};
            }

        	public override string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel)
        	{
				return new string[] { };    // nothing to do
			}

        	public override string[] GenerateDropScripts(Configuration config)
            {
				return new string[] { GetDialect(config).GetDropTableString(TABLE_NAME) };
            }

            #endregion
        }



        #region IAccessionNumberBroker Members

    	/// <summary>
    	/// Peeks at the next accession number in the sequence, but does not advance the sequence.
    	/// </summary>
    	/// <returns></returns>
		public string PeekNextAccessionNumber()
		{
			// try to read the next accession number
			try
			{
				IDbCommand select = this.Context.CreateSqlCommand(string.Format("SELECT * from {0}", TABLE_NAME));
				return select.ExecuteScalar().ToString();
			}
			catch (Exception e)
			{
				throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
			}
		}

    	/// <summary>
    	/// Gets the next accession number in the sequence, advancing the sequence by 1.
    	/// </summary>
    	/// <returns></returns>
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
