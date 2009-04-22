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
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Provides base implementation of <see cref="IProcedureQueryBroker{TInput,TOutput}"/>
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class ProcedureQueryBroker<TInput,TOutput> : Broker,IProcedureQueryBroker<TInput,TOutput>
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        private String _procedureName;

        protected ProcedureQueryBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureQueryBroker<TInput,TOutput> Members

        public IList<TOutput> Find(TInput criteria)
        {
            IList<TOutput> list = new List<TOutput>();

            InternalFind(criteria, -1, delegate(TOutput row)
            {
                list.Add(row);
            });

            return list;
        }

		public void Find(TInput criteria, ProcedureQueryCallback<TOutput> callback)
		{
			InternalFind(criteria, -1, callback);
		}

    	public TOutput FindOne(TInput criteria)
    	{
    		TOutput result = null;

			InternalFind(criteria, 1, delegate(TOutput row)
			{
				result = row;
			});

    		return result;
    	}

    	private void InternalFind(TInput criteria, int maxResults, ProcedureQueryCallback<TOutput> callback)
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
                
                // Set parameters
                SetParameters(command, criteria);

				if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					Platform.Log(LogLevel.Debug, "Executing stored procedure: {0}", _procedureName);

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
                    	int resultCount = 0;
                    	Dictionary<string, PropertyInfo> propMap = GetEntityMap(typeof (TOutput));
                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, propMap);

                            callback(row);

                        	resultCount++;
							if (maxResults > 0 && resultCount >= maxResults)
								break;
                        }
						myReader.Close();
						myReader = null;
					}
					// Note:  The retrieving of output parameters must occur after
					// the reader has been closed.
					GetOutputParameters(command, criteria);
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
