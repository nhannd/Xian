#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Provides base implementation of <see cref="IProcedureUpdateBroker{TInput}"/>
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
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
				command.CommandTimeout = SqlServerSettings.Default.CommandTimeout;
				
				UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

				if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					Platform.Log(LogLevel.Debug, "Executing stored procedure: {0}", _procedureName);

                // Set parameters
                SetParameters(command, criteria);

                int rows = command.ExecuteNonQuery();

				GetOutputParameters(command, criteria);
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
