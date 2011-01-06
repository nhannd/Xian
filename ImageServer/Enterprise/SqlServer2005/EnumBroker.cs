#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{    
    public abstract class EnumBroker<TOutput> : Broker, IEnumBroker<TOutput>
        where TOutput : ServerEnum, new()
    {
        #region IEnumBroker<TOutput> Members

        List<TOutput> IEnumBroker<TOutput>.Execute() 
        {
            List<TOutput> list = new List<TOutput>();
            TOutput tempValue = new TOutput();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {               
                command = new SqlCommand(String.Format("SELECT * FROM {0}",tempValue.Name), Context.Connection);
                command.CommandType = CommandType.Text;
				command.CommandTimeout = SqlServerSettings.Default.CommandTimeout;

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", tempValue.Name);
                    command.Dispose();
                    return list;
                }
                else
                {
                    if (myReader.HasRows)
                    {
						Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TOutput));

                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, propMap);

                            list.Add(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when retrieving enumerated value: {0}", tempValue.Name);

                throw new PersistenceException(String.Format("Unexpected problem when retrieving enumerated value: {0}: {1}", tempValue.Name, e.Message), e);
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
            return list;
        }

        #endregion
    }
}
