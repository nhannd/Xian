#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using System.Data;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using NHibernate.Cfg;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Base class for brokers that encapsulate sequence generators.
	/// </summary>
	public abstract class SequenceBroker : Broker
	{
		private readonly string _tableName;
		private readonly string _columnName;
		private readonly long _initialValue;

		protected SequenceBroker(string tableName, string columnName, long initialValue)
		{
			_tableName = tableName;
			_columnName = columnName;
			_initialValue = initialValue;
		}

		public string[] GenerateCreateScripts(Configuration config)
		{
			var defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
			var tableName = !string.IsNullOrEmpty(defaultSchema) ? defaultSchema + "." + _tableName : _tableName;

			return new[]
				{
					string.Format("create table {0} ( {1} {2} );", tableName, _columnName, DdlScriptGenerator.GetDialect(config).GetTypeName( NHibernate.SqlTypes.SqlTypeFactory.Int64 )),
					string.Format("insert into {0} values ( {1} )", tableName, _initialValue)
				};
		}

		public string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel)
		{
			return new string[] { };	// nothing to do
		}

		public string[] GenerateDropScripts(Configuration config)
		{
			return new [] { DdlScriptGenerator.GetDialect(config).GetDropTableString(_tableName) };
		}


		/// <summary>
		/// Peeks at the next number in the sequence, but does not advance the sequence.
		/// </summary>
		/// <returns></returns>
		public string PeekNext()
		{
			// try to read the next accession number
			try
			{
				var select = this.CreateSqlCommand(string.Format("SELECT * from {0}", _tableName));
				return select.ExecuteScalar().ToString();
			}
			catch (Exception e)
			{
				throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
			}
		}

		/// <summary>
		/// Gets the next number in the sequence, advancing the sequence by 1.
		/// </summary>
		/// <returns></returns>
		public string GetNext()
		{
			int updatedRows;
			long sequenceValue;

			// the loop is necessary to ensure that we succeed in obtaining an accession number
			// It is possible that another process is trying to do this at the same time,
			// hence there is an inevitable race condition which may cause the operation to occassionally fail
			// can we avoid the need for a loop by using Serializable transaction isolation for this operation???
			do
			{
				// try to read the next accession number
				try
				{
					var select = this.CreateSqlCommand(string.Format("SELECT * from {0}", _tableName));
					sequenceValue = (long)select.ExecuteScalar();
				}
				catch (Exception e)
				{
					throw new PersistenceException(SR.ErrorFailedReadNextSequenceNumber, e);
				}

				if (sequenceValue == 0)
				{
					throw new HealthcareWorkflowException(SR.ErrorSequenceNotInitialized);
				}

				// update the sequence, by trying to update a row containing the previous number
				// this may fail if another process has updated in the meantime, in which case
				// the loop will just try again
				try
				{
					var updateSql = string.Format("UPDATE {0} SET {1} = @next WHERE {2} = @prev", _tableName, _columnName, _columnName);
					var update = this.CreateSqlCommand(updateSql);
					AddParameter(update, "next", sequenceValue + 1);
					AddParameter(update, "prev", sequenceValue);

					updatedRows = update.ExecuteNonQuery();
				}
				catch (Exception e)
				{
					throw new PersistenceException(SR.ErrorFailedUpdateNextSequenceNumber, e);
				}
			}
			while (updatedRows == 0);

			return sequenceValue.ToString();
		}

		private static void AddParameter(IDbCommand cmd, string name, object value)
		{
			var p = cmd.CreateParameter();
			p.ParameterName = name;
			p.Value = value;
			cmd.Parameters.Add(p);
		}
	}
}
