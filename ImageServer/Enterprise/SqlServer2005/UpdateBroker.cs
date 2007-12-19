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
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    /// <summary>
    /// Provides base implementation of <see cref="IUpdateBroker{TEntity, TParameters}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a base implementation for doing non-procedural dynamic SQL update/insert.  When used to
    /// update a record, it takes as input a <see cref="ServerEntityKey"/> which references to the record in the database,
    /// and an update parameter derived from <see cref="UpdateBrokerParameters"/> which specifies
    /// the fields to be updated.
    /// </para>
    /// <para>
    /// When used to insert a record, it takes an update parameter derived from <see cref="UpdateBrokerParameters"/> which specifies
    /// the values of for the fields in the new record. When successful, it returns the newly inserted entity derived from <see cref="ServerEntity"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TParameters"></typeparam>
    public abstract class UpdateBroker<TEntity, TParameters> : Broker, IUpdateBroker<TEntity, TParameters>
        where TParameters : UpdateBrokerParameters
        where TEntity : ServerEntity, new()
    {
        #region Private members
        private readonly String _entityName;
        #endregion Private members

        #region Constructors
        protected UpdateBroker(String entityName)
        {
            _entityName = entityName;
        }

        #endregion Constructors

        #region Protected Static methods
        /// <summary>
        /// Resolves the DB column name for a field name
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        protected static String GetDBColumnName(UpdateBrokerParameterBase parm)
        {
            String sqlColumnName;

            if (parm is UpdateBrokerParameter<ServerEntity>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("[{0}]", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("[{0}]", parm.FieldName);

                else
                    sqlColumnName = String.Format("[{0}GUID]", parm.FieldName);

            }
            else if (parm is UpdateBrokerParameter<ServerEntityKey>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("[{0}]", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("[{0}]", parm.FieldName);

                else
                    sqlColumnName = String.Format("[{0}GUID]", parm.FieldName);

            }
            else if (parm is UpdateBrokerParameter<ServerEnum>)
            {
                if (parm.FieldName.EndsWith("Enum"))
                    sqlColumnName = String.Format("[{0}]", parm.FieldName);

                else
                    sqlColumnName = String.Format("[{0}Enum]", parm.FieldName);

            }
            else
            {
                sqlColumnName = String.Format("[{0}]", parm.FieldName);
            }

            return sqlColumnName;
        }
        protected static String BuildWhereClause(ServerEntityKey key, UpdateBrokerParameters parm)
        {

            return String.Format("[GUID]='{0}'", key.Key);

        }
        protected static String BuildSetClause(UpdateBrokerParameters parameters)
        {
            StringBuilder setClause = new StringBuilder();
            bool first = true;
            foreach (UpdateBrokerParameterBase parm in parameters.SubParameters.Values)
            {
                String text;

                if (parm is UpdateBrokerParameter<XmlDocument>)
                {
                    UpdateBrokerParameter<XmlDocument> p = parm as UpdateBrokerParameter<XmlDocument>;

                    XmlDocument xml = p.Value;
                    StringWriter sw = new StringWriter();
                    XmlWriterSettings xmlSettings = new XmlWriterSettings();
                    xmlSettings.Encoding = Encoding.UTF8;
                    xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
                    xmlSettings.Indent = false;
                    xmlSettings.NewLineOnAttributes = false;
                    xmlSettings.CheckCharacters = true;
                    xmlSettings.IndentChars = "";

                    XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings);
                    xml.WriteTo(xmlWriter);
                    xmlWriter.Close();

                    text = String.Format("{0}='{1}'", GetDBColumnName(parm), sw);
                }
                else if (parm is UpdateBrokerParameter<ServerEnum>)
                {
                    UpdateBrokerParameter<ServerEnum> p = parm as UpdateBrokerParameter<ServerEnum>;
                    ServerEnum v = p.Value;
                    text = String.Format("{0}='{1}'", GetDBColumnName(parm), v.Enum);

                }
                else if (parm is UpdateBrokerParameter<ServerEntity>)
                {
                    UpdateBrokerParameter<ServerEntity> p = parm as UpdateBrokerParameter<ServerEntity>;
                    ServerEntity v = p.Value;
                    text = String.Format("{0}='{1}'", GetDBColumnName(parm), v.GetKey().Key);
                }
                else if (parm is UpdateBrokerParameter<ServerEntityKey>)
                {

                    UpdateBrokerParameter<ServerEntityKey> p = parm as UpdateBrokerParameter<ServerEntityKey>;
                    ServerEntityKey key = p.Value;
                    text = String.Format("{0}='{1}'", GetDBColumnName(parm), key.Key);
                }
                else
                {
                    text = String.Format("{0}='{1}'", GetDBColumnName(parm), parm.Value);
                }

                if (first)
                {
                    first = false;
                    setClause.AppendFormat(text);
                }
                else
                    setClause.AppendFormat(", {0}", text);
            }

            return setClause.ToString();
        }
        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="key">The GUID of the table row to update</param>
        /// <param name="parameters">The columns to update.</param>
        /// <returns>The SQL string.</returns>
        protected static string BuildUpdateSql(string entityName, SqlCommand command, ServerEntityKey key, UpdateBrokerParameters parameters)
        {
            
            // SET clause
            String setClause = BuildSetClause(parameters);

            // WHERE caluse
            String whereClause = BuildWhereClause(key, parameters);
            
            return String.Format("UPDATE [{0}] SET {1} WHERE {2}", entityName, setClause, whereClause);
        }

        /// <summary>
        /// Generates a SQL statement based on the input.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="parameters">The columns to insert.</param>
        /// <returns>The SQL string.</returns>
        protected static string BuildInsertSql(string entityName, UpdateBrokerParameters parameters)
        {

            Guid guid = Guid.NewGuid();

            // Build the text after the INSERT INTO clause
            StringBuilder intoText = new StringBuilder();
            intoText.Append("(");
            intoText.Append("[GUID]");
            foreach (UpdateBrokerParameterBase parm in parameters.SubParameters.Values)
            {
                intoText.AppendFormat(", {0}", GetDBColumnName(parm));
            }
            intoText.Append(")");

            // Build the text after the VALUES clause
            StringBuilder valuesText = new StringBuilder();
            valuesText.Append("(");
            valuesText.AppendFormat("'{0}'", guid);
            foreach (UpdateBrokerParameterBase parm in parameters.SubParameters.Values)
            {

                if (parm is UpdateBrokerParameter<ServerEnum>)
                {
                    ServerEnum v = (ServerEnum)parm.Value;
                    valuesText.AppendFormat(", '{0}'", v.Enum);
                }
                else if (parm is UpdateBrokerParameter<ServerEntity>)
                {
                    ServerEntity v = (ServerEntity)parm.Value;
                    valuesText.AppendFormat(", '{0}'", v.GetKey().Key);
                }
                else if (parm is UpdateBrokerParameter<ServerEntityKey>)
                {
                    ServerEntityKey key = (ServerEntityKey)parm.Value;
                    valuesText.AppendFormat(", '{0}'", key.Key);
                }
                else if (parm is UpdateBrokerParameter<XmlDocument>)
                {
                    XmlDocument xml = (XmlDocument)parm.Value;
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = false;

                    using (XmlWriter writer = XmlWriter.Create(sb, settings))
                    {
                        xml.WriteTo(writer);
                        writer.Flush();
                    }

                    valuesText.AppendFormat(", N'{0}'", sb); // NOTE: SQL Server 2005 stores data in Unicode (UTF-16) by default. XML string must be prefixed with an N to define it as a Unicode string
                }
                else
                {
                    valuesText.AppendFormat(", '{0}'", parm.Value);
                }

            }
            valuesText.Append(")");

            // Generate the INSERT statement
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO [{0}] {1} VALUES {2}\n", entityName, intoText, valuesText);

            // Add the SELECT statement. This allows us to popuplate the entity with the inserted values 
            // and return to the caller
            sql.AppendFormat("SELECT * FROM [{0}] WHERE [GUID]='{1}'", entityName, guid);

            return sql.ToString();

        }
        #endregion Protected Static methods

        #region IUpdateBroker<TInput,TOutput> Members

        public bool Delete (ServerEntityKey key)
        {
            Platform.CheckForNullReference(key, "key");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = String.Format("delete from {0} where GUID = '{1}'", _entityName, key.Key);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                    command != null ? command.CommandText : "");

                throw new PersistenceException(String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();

            }
        }

        public bool Update(ServerEntityKey key, TParameters parameters)
        {
            Platform.CheckForNullReference(key, "key");
            Platform.CheckForNullReference(parameters, "parameters");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = BuildUpdateSql(_entityName, command, key, parameters);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}", 
                        command != null ? command.CommandText : "");

                throw new PersistenceException(String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
               
                if (command != null)
                    command.Dispose();

            }

        }

        public TEntity Insert(TParameters parameters)
        {
            Platform.CheckForNullReference(parameters, "parameters");
            Platform.CheckFalse(parameters.IsEmpty, "parameters must not be empty");
            
            
            SqlCommand command = null;
            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = BuildInsertSql(_entityName, parameters);

                TEntity entity = null;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            entity = new TEntity();
                            PopulateEntity(reader, entity, typeof(TEntity));
                            break;
                        }
                    }
                }


                return entity;
               
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}", 
                    command != null ? command.CommandText : "");

                throw new PersistenceException(String.Format("Unexpected problem with update statment on table {0}: {1}", _entityName, e.Message), e);
            }
            finally
            {
                // Cleanup
                if (command != null)
                    command.Dispose();
            }
        }

        #endregion




    }
}
