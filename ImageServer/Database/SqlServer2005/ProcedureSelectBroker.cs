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
using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class ProcedureSelectBroker<TInput,TOutput> : Broker,IProcedureSelectBroker<TInput,TOutput>
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        private String _procedureName;

        protected ProcedureSelectBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureSelectBroker<TInput,TOutput> Members

        public IList<TOutput> Execute(TInput criteria)
        {
            IList<TOutput> list = new List<TOutput>();

            Execute(criteria, delegate(TOutput row)
            {
                list.Add(row);
            });

            return list;
        }

        public void Execute(TInput criteria, SelectCallback<TOutput> callback)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(_procedureName, Context.Connection);
                command.CommandType = CommandType.StoredProcedure;

                // Set parameters
                SetParameters(command, criteria);


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

                throw new PersistenceException("Unexpected problem with stored procedure: " + _procedureName, e);
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
