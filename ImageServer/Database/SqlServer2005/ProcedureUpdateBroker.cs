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
    public abstract class ProcedureUpdateBroker<TInput> : Broker, IProcedureUpdateBroker<TInput>
        where TInput : ProcedureParameters
    {
        private String _procedureName;

        protected ProcedureUpdateBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureUpdateBroker<TInput> Members

        public bool Execute(TInput criteria)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(_procedureName, Context.Connection);
                command.CommandType = CommandType.StoredProcedure;

                // Set parameters
                SetParameters(command, criteria);

                int rows = command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when calling stored procedure: {0}", _procedureName);

                throw new PersistenceException(String.Format("Unexpected error with stored procedure: {0}", _procedureName), e);
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

            return true;
        }

        #endregion
    }
}
