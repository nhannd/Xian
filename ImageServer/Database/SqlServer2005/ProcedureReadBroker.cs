using System;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Sql;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    /// <summary>
    /// Provides base implementation of <see cref="IProcedureReadBroker{TOutput}"/>
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class ProcedureReadBroker<TOutput> : Broker, IProcedureReadBroker<TOutput>
        where TOutput : ServerEntity, new()
    {
        private String _procedureName;

        protected ProcedureReadBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureReadBroker<TOutput> Members

        public IList<TOutput> Execute()
        {
            IList<TOutput> list = new List<TOutput>();

            Execute(delegate(TOutput row)
            {
                list.Add(row);
            });

            return list;
        }

        public void Execute(ProcedureReadCallback<TOutput> callback)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(_procedureName, Context.Connection);
                command.CommandType = CommandType.StoredProcedure;

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to execute stored procedure '{0}'", _procedureName);
                    command.Dispose();
                    return;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, typeof(TOutput));

                            callback(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when calling stored procedure: {0}", _procedureName);

                throw new PersistenceException(String.Format("Unexpected problem with stored procedure: {0}: {1}", _procedureName, e.Message), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }
          
        }

        #endregion
    }
}
