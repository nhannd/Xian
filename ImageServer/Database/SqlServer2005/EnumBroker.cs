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
    public abstract class EnumBroker<TOutput> : Broker, IEnumBroker<TOutput>
        where TOutput : ServerEnum, new()
    {
        #region IEnumBroker<TOutput> Members

        IList<TOutput> IEnumBroker<TOutput>.Execute() 
        {
            IList<TOutput> list = new List<TOutput>();
            TOutput tempValue = new TOutput();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {               
                command = new SqlCommand(String.Format("SELECT * FROM {0}",tempValue.Name), Context.Connection);
                command.CommandType = CommandType.Text;

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
                        while (myReader.Read())
                        {
                            TOutput row = new TOutput();

                            PopulateEntity(myReader, row, typeof(TOutput));

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
