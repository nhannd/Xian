using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class SelectBroker<TInput,TOutput> : Broker,ISelectBroker<TInput,TOutput>
        where TInput : SelectCriteria
        where TOutput : ServerEntity, new()
    {
        private readonly String _entityName;

        protected SelectBroker(String entityName)
        {
            _entityName = entityName;
        }

        private static string GetWhereText(string variable, SearchConditionBase sc, SqlCommand command)
        {
            StringBuilder sb = new StringBuilder();
            String sqlColumnName;
            String sqlParmName;
            object[] values = new object[sc.Values.Length];

            if (variable.EndsWith("Key"))
                sqlColumnName = variable.Replace("Key", "GUID");
            else
                sqlColumnName = variable;

            // Remove the "." from the variable name, if it exists
            int j = sqlColumnName.IndexOf(".");
            if (j != -1)
                sqlParmName = sqlColumnName.Remove(j, 1);
            else
                sqlParmName = sqlColumnName;

            for (int i = 0; i < sc.Values.Length; i++)
            {
                ServerEntityKey key = sc.Values[i] as ServerEntityKey;
                if (key != null)
                    values[i] = key.Key;
                else
                {
                    ServerEnum e = sc.Values[i] as ServerEnum;
                    if (e != null)
                        values[i] = e.Enum;
                    else
                        values[i] = sc.Values[i];
                }
            }


            switch (sc.Test)
            {
                case SearchConditionTest.Equal:
                    sb.AppendFormat("{0} = @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotEqual:
                    sb.AppendFormat("{0} <> @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Like:
                    sb.AppendFormat("{0} like @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotLike:
                    sb.AppendFormat("{0} not like @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Between:
                    sb.AppendFormat("{0} between @{1}1 and @{1}2", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName + "1", values[0]);
                    command.Parameters.AddWithValue("@" + sqlParmName + "2", values[1]);
                    break;
                case SearchConditionTest.In:
                    sb.AppendFormat("{0} in (", sqlColumnName);  // assume at least one param
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == 0)
                            sb.AppendFormat("@{0}{1}", sqlParmName, i + 1);
                        else
                            sb.AppendFormat(", @{0}{1}", sqlParmName, i + 1);
                        command.Parameters.AddWithValue(string.Format("@{0}{1}", sqlParmName, i + 1), values[i]);
                    }
                    sb.Append(")");
                    break;
                case SearchConditionTest.LessThan:
                    sb.AppendFormat("{0} < @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.LessThanOrEqual:
                    sb.AppendFormat("{0} <= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThan:
                    sb.AppendFormat("{0} > @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThanOrEqual:
                    sb.AppendFormat("{0} >= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotNull:
                    sb.AppendFormat("{0} is not null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Null:
                    sb.AppendFormat("{0} is null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.None:
                default:
                    throw new ApplicationException();  // invalid
            }

            return sb.ToString();
        }
        public static String[] WhereSearchCriteria(string qualifier, SearchCriteria criteria, SqlCommand command)
        {
            List<string> list = new List<string>();

            if (criteria is SearchConditionBase)
            {
                SearchConditionBase sc = (SearchConditionBase)criteria;
                if (sc.Test != SearchConditionTest.None)
                {
                    String text = GetWhereText(qualifier, sc,command);
                    list.Add(text);
                }
            }
            else
            {
                // recurse on subCriteria
                foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
                {
                    string subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
                    list.AddRange(WhereSearchCriteria(subQualifier, subCriteria, command));
                }
            }

            return list.ToArray();
        }

        #region ISelectBroker<TInput,TOutput> Members

        public TOutput Load(ServerEntityKey entityRef)
        {
            TOutput row = new TOutput();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(String.Format("SELECT * FROM {0} WHERE GUID = @GUID", 
                    _entityName), Context.Connection);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@GUID", entityRef.Key);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    command.Dispose();
                    return null;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        myReader.Read();

                        PopulateEntity(myReader, row, typeof (TOutput));

                        return row;
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when retrieving enumerated value: {0}", _entityName);

                throw new PersistenceException(String.Format("Unexpected problem when retrieving enumerated value: {0}: {1}", _entityName, e.Message), e);
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

            return row;
        }

        public IList<TOutput> Find(TInput criteria)
        {
            IList<TOutput> list = new List<TOutput>();

            Find(criteria, delegate(TOutput row)
            {
                list.Add(row);
            });

            return list;
        }

        public void Find(TInput criteria, SelectCallback<TOutput> callback)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM {0}", _entityName);


            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;

                String[] where = WhereSearchCriteria(_entityName, criteria, command);
                bool first = true;
                foreach (String clause in where)
                {
                    if (first)
                    {
                        first = false;
                        sb.AppendFormat(" WHERE {0}", clause);
                    }
                    else
                        sb.AppendFormat(" AND {0}", clause);
                }
                command.CommandText = sb.ToString();

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    Platform.Log(LogLevel.Error, "Select statement: {0}", sb.ToString());

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
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sb.ToString());

                throw new PersistenceException(String.Format("Unexpected problem with select statment on table {0}: {1}", _entityName, e.Message), e);
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
