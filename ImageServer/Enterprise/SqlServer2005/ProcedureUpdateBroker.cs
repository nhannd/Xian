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
