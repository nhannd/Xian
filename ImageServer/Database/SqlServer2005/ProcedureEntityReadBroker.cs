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
    public abstract class ProcedureEntityReadBroker<TOutput> : Broker, IProcedureEntityReadBroker<TOutput>
        where TOutput : ProcedureEntity, new()
    {
        private String _procedureName;

        protected ProcedureEntityReadBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureEntityReadBroker<TOutput> Members

        public IList<TOutput> Execute()
        {
            IList<TOutput> list = new List<TOutput>();

            Execute(delegate(TOutput row)
            {
                list.Add(row);
            });

            return list;
        }

        public void Execute(ReadCallback<TOutput> callback)
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
                            
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(row);

                            for (int i = 0; i < myReader.FieldCount; i++)
                            {
                                if (myReader.GetName(i).Equals("GUID"))
                                {
                                    Guid uid = myReader.GetGuid(i);
                                    row.SetOid(uid);
                                    continue;
                                }

                                PropertyDescriptor prop = props[myReader.GetName(i)];

                                if (prop.PropertyType == typeof(String))
                                {
                                    prop.SetValue(row, myReader.GetString(i));
                                }
                                else if (prop.PropertyType == typeof(Int32))
                                {
                                    prop.SetValue(row, myReader.GetInt32(i));
                                }
                                else if (prop.PropertyType == typeof(Int16))
                                    prop.SetValue(row, myReader.GetInt16(i));
                                else if (prop.PropertyType == typeof(DateTime))
                                    prop.SetValue(row, myReader.GetDateTime(i));
                                else if (prop.PropertyType == typeof(EntityRef))
                                {
                                    Guid uid = myReader.GetGuid(i);
                                    prop.SetValue(row, new EntityRef(typeof(TOutput), uid, 0));
                                }
                                else if (prop.PropertyType == typeof(bool))
                                    prop.SetValue(row, myReader.GetBoolean(i));
                                else
                                    throw new EntityNotFoundException("Unsupported property type: " + prop.PropertyType, null);
                            }

                            callback(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(e, LogLevel.Error, "Unexpected exception when calling stored procedure: {0}", _procedureName);

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
