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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    public abstract class Broker : IPersistenceBroker
    {
        private PersistenceContext _context;

        /// <summary>
        /// Returns the persistence context associated with this broker instance.
        /// </summary>
        protected PersistenceContext Context
        {
            get { return _context; }
        }

        public void SetContext(IPersistenceContext context)
        {
            _context = (PersistenceContext)context;
        }

        protected static void SetParameters(SqlCommand command, ProcedureParameters parms)
        {
            foreach (SearchCriteria parm in parms.SubCriteria.Values)
            {
                String sqlParmName = "@" + parm.GetKey();

                if (parm is ProcedureParameter<DateTime?>)
                {
                    ProcedureParameter<DateTime?> parm2 = (ProcedureParameter<DateTime?>)parm;
					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.DateTime);
						sqlParm.IsNullable = true;
						sqlParm.Direction = ParameterDirection.Output;
					}
                }
                else if (parm is ProcedureParameter<DateTime>)
                {
                    ProcedureParameter<DateTime> parm2 = (ProcedureParameter<DateTime>)parm;
					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.DateTime);
						sqlParm.Direction = ParameterDirection.Output;
					}
				}
                else if (parm is ProcedureParameter<int>)
                {
                    ProcedureParameter<int> parm2 = (ProcedureParameter<int>)parm;

					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Int);
						sqlParm.Direction = ParameterDirection.Output;
					}
				}
                else if (parm is ProcedureParameter<ServerEntityKey>)
                {
                    sqlParmName = sqlParmName.Replace("Key", "GUID");
                    ProcedureParameter<ServerEntityKey> parm2 = (ProcedureParameter<ServerEntityKey>)parm;

					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value.Key);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.UniqueIdentifier);
						sqlParm.Direction = ParameterDirection.Output;
					}
				}
                else if (parm is ProcedureParameter<bool>)
                {
                    ProcedureParameter<bool> parm2 = (ProcedureParameter<bool>)parm;
					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Bit);
						sqlParm.Direction = ParameterDirection.Output;
					}
				}
                else if (parm is ProcedureParameter<string>)
                {
                    ProcedureParameter<string> parm2 = (ProcedureParameter<string>)parm;

					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.NVarChar, 1024);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
				}
                else if (parm is ProcedureParameter<ServerEnum>)
                {
                    ProcedureParameter<ServerEnum> parm2 = (ProcedureParameter<ServerEnum>)parm;
					if (parm2.Value == null)
						command.Parameters.AddWithValue(sqlParmName, null);
					else
					{
						if (parm2.Output)												
							throw new PersistenceException("Unsupported output parameter type: ServerEnum",null);

						command.Parameters.AddWithValue(sqlParmName, parm2.Value.Enum);
					}
                }
                else if (parm is ProcedureParameter<Decimal>)
                {
                    ProcedureParameter<Decimal> parm2 = (ProcedureParameter<Decimal>)parm;

					if (!parm2.Output)
						command.Parameters.AddWithValue(sqlParmName, parm2.Value);
					else
					{
						SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Decimal);
						sqlParm.Direction = ParameterDirection.Output;
					}
                }
                else if (parm is ProcedureParameter<XmlDocument>)
                {
                    ProcedureParameter<XmlDocument> parm2 = (ProcedureParameter<XmlDocument>)parm;
                    if (parm2.Value == null)
                        command.Parameters.AddWithValue(sqlParmName, null);
                    else
                    {
						if (parm2.Output)
							throw new PersistenceException("Unsupported output parameter type: XmlDocument", null);

                        XmlNodeReader reader = new XmlNodeReader(parm2.Value.DocumentElement);                        
                        SqlXml xml = new SqlXml(reader);

                        command.Parameters.AddWithValue(sqlParmName, xml);
                    }
                }
                else
                    throw new PersistenceException("Unknown procedure parameter type: " + parm.GetType(), null);

            }

        }

		protected static void GetOutputParameters(SqlCommand command, ProcedureParameters parms)
		{
			foreach (SearchCriteria parm in parms.SubCriteria.Values)
			{
				String sqlParmName = "@" + parm.GetKey();

				if (parm is ProcedureParameter<DateTime?>)
				{
					ProcedureParameter<DateTime?> parm2 = (ProcedureParameter<DateTime?>)parm;
					if (!parm2.Output)
						continue;
					else
					{
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						parm2.Value = (DateTime?)sqlParm.Value;
					}
				}
				else if (parm is ProcedureParameter<DateTime>)
				{
					ProcedureParameter<DateTime> parm2 = (ProcedureParameter<DateTime>)parm;
					if (!parm2.Output)
						continue;
					else
					{
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						parm2.Value = (DateTime)sqlParm.Value;
					}
				}
				else if (parm is ProcedureParameter<int>)
				{
					ProcedureParameter<int> parm2 = (ProcedureParameter<int>)parm;

					if (!parm2.Output)
						continue;
					else
					{
						
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						//object o = command.Connection.Get
						if (sqlParm.Value != null)
							parm2.Value = (int)sqlParm.Value;
					}
				}
				else if (parm is ProcedureParameter<ServerEntityKey>)
				{
					sqlParmName = sqlParmName.Replace("Key", "GUID");
					ProcedureParameter<ServerEntityKey> parm2 = (ProcedureParameter<ServerEntityKey>)parm;

					if (!parm2.Output)
						continue;
					else
					{
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						parm2.Value = new ServerEntityKey("", sqlParm.Value);
					}
				}
				else if (parm is ProcedureParameter<bool>)
				{
					ProcedureParameter<bool> parm2 = (ProcedureParameter<bool>)parm;
					if (!parm2.Output)
						continue;
					else
					{
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						parm2.Value = (bool)sqlParm.Value;
					}
				}
				else if (parm is ProcedureParameter<Decimal>)
				{
					ProcedureParameter<Decimal> parm2 = (ProcedureParameter<Decimal>)parm;

					if (!parm2.Output)
						continue;
					else
					{
						SqlParameter sqlParm = command.Parameters[sqlParmName];
						parm2.Value = (Decimal)sqlParm.Value;
					}
				}
                else if (parm is ProcedureParameter<string>)
                {
                    ProcedureParameter<string> parm2 = (ProcedureParameter<string>)parm;

                    if (!parm2.Output)
                        continue;
                    else
                    {
                        SqlParameter sqlParm = command.Parameters[sqlParmName];
                        if (sqlParm.Value != DBNull.Value)
                            parm2.Value = (string)sqlParm.Value;
                    }
                }
			}
		}
        protected static void PopulateEntity(SqlDataReader reader, ServerEntity entity, Type entityType)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(entity);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                String columnName = reader.GetName(i);

				// Special case for when we select a range of values with an EntityBroker, just ignore
				if (columnName.Equals("RowNum"))
					continue;

                if (columnName.Equals("GUID"))
                {
                    Guid uid = reader.GetGuid(i);
                    entity.SetKey(new ServerEntityKey(entity.Name, uid));
                    continue;
                }

                if (columnName.Equals(entity.Name) && columnName.Contains("Enum"))
                    columnName = "Enum";
                if (columnName.Contains("GUID"))
                    columnName = columnName.Replace("GUID", "Key");

                PropertyDescriptor prop = props[columnName];
                if (prop == null)
                    throw new EntityNotFoundException("Unable to match column to property: " + columnName, null);

                if (reader.IsDBNull(i))
                {
                    prop.SetValue(entity, null);
                    continue;
                }

                if (prop.PropertyType == typeof(String))
                    prop.SetValue(entity, reader.GetString(i));
                else if (prop.PropertyType == typeof(Int32))
                    prop.SetValue(entity, reader.GetInt32(i));
                else if (prop.PropertyType == typeof(Int16))
                    prop.SetValue(entity, reader.GetInt16(i));
                else if (prop.PropertyType == typeof(double))
                    prop.SetValue(entity, reader.GetDouble(i));
                else if (prop.PropertyType == typeof(Decimal))
                    prop.SetValue(entity, reader.GetDecimal(i));
                else if (prop.PropertyType == typeof(float))
                    prop.SetValue(entity, reader.GetFloat(i));
                else if (prop.PropertyType == typeof(DateTime))
                    prop.SetValue(entity, reader.GetDateTime(i));
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(entity, reader.GetBoolean(i));
                else if (prop.PropertyType == typeof(XmlDocument))
                {
                    SqlXml xml = reader.GetSqlXml(i);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml.Value);
                    prop.SetValue(entity, xmlDoc);
                }
                else if (prop.PropertyType == typeof(ServerEntityKey))
                {
                    Guid uid = reader.GetGuid(i);
                    prop.SetValue(entity, new ServerEntityKey(columnName.Replace("Key", ""), uid));
                }
                else if (typeof(ServerEnum).IsAssignableFrom(prop.PropertyType))
                {
                    short enumVal = reader.GetInt16(i);
                    ConstructorInfo construct = prop.PropertyType.GetConstructor(new Type[0]);
                    ServerEnum val = (ServerEnum)construct.Invoke(null);
                    val.SetEnum(enumVal);
                    prop.SetValue(entity, val);
                }
                else
                    throw new EntityNotFoundException("Unsupported property type: " + prop.PropertyType, null);
            }
        }



    }
}
