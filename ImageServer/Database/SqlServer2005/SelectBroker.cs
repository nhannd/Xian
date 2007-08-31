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
    /// <summary>
    /// Provides base implementation of <see cref="ISelectBroker{TInput,TOutput}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a base implementation for doing dynamic SQL queries.  It takes as input 
    /// query criteria as defined in a <see cref="SelectCriteria"/> derived class.  It outputs
    /// data into a <see cref="ServerEntity"/> defined class.
    /// </para>
    /// <para>
    /// The class generates a SQL Server compatible SELECT statement, executes the statement, 
    /// and returns results.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class SelectBroker<TInput,TOutput> : Broker,ISelectBroker<TInput,TOutput>
        where TInput : SelectCriteria
        where TOutput : ServerEntity, new()
    {
        private readonly String _entityName;

        protected SelectBroker(String entityName)
        {
            _entityName = entityName;
        }

        /// <summary>
        /// Gets WHERE clauses based on the input search condition.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="sc"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string GetWhereText(string variable, SearchConditionBase sc, SqlCommand command)
        {
            StringBuilder sb = new StringBuilder();
            String sqlColumnName;
            String sqlParmName;
            object[] values = new object[sc.Values.Length];

            // With the Server, all primary keys end with "Key".  The database implementation itself
            // names these columns with the name GUID instead of Key.
            if (variable.EndsWith("Key"))
                sqlColumnName = variable.Replace("Key", "GUID");
            else
                sqlColumnName = variable;

            // We use input parameters to the select statement.  We create a variable name for the 
            // input parameter based on the column name input.  Variable names can't have periods,
            // so we have to remove the "."
            int j = sqlColumnName.IndexOf(".");
            if (j != -1)
                sqlParmName = sqlColumnName.Remove(j, 1);
            else
                sqlParmName = sqlColumnName;

            // Now go through the actual input parameters.  Replace references to ServerEntityKey with
            // the GUID itself for these parameters, and replace ServerEnum derived references with the 
            // value of the enum in the array so the input parameters work properly.
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

            // Generate the actual WHERE clauses based on the type of condition.
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
                case SearchConditionTest.NotExists:
                    SelectCriteria notExistsSubCriteria = (SelectCriteria) values[0];

                    string sql;
                    sql = GetSql(notExistsSubCriteria.GetKey(), command, notExistsSubCriteria,
                        String.Format("{0}.GUID = {1}.{0}GUID", variable, notExistsSubCriteria.GetKey()));
                    sb.AppendFormat("NOT EXISTS ({0})",sql);
                    break;
                case SearchConditionTest.Exists:
                    SelectCriteria existsSubCriteria = (SelectCriteria)values[0];
                    string existsSql;
                    existsSql = GetSql(existsSubCriteria.GetKey(), command, existsSubCriteria,
                        String.Format("{0}.GUID = {1}.{0}GUID", variable, existsSubCriteria.GetKey()));
                    sb.AppendFormat("EXISTS ({0})", existsSql);
                    break;
                case SearchConditionTest.None:
                default:
                    throw new ApplicationException();  // invalid
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get an array of WHERE clauses for all of the search criteria specified.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="criteria"></param>
        /// <param name="command"></param>
        /// <returns></returns>
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
                    // Note:  this is a bit ugly, but we don't do the <Table>.<Column>
                    // syntax for Subselect type criteria.  Subselects only need 
                    // the table name, and there isn't a real column associated with it.
                    // We could pass the entity down all the way, but decided to do it
                    // this way instead.
                    string subQualifier;
                    if (subCriteria is SubSelect<SelectCriteria>)
                        subQualifier = qualifier;
                    else
                        subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
                    list.AddRange(WhereSearchCriteria(subQualifier, subCriteria, command));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the select</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-select, additional where clauses are included here for the select.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        public static string GetSql(string entityName, SqlCommand command, SelectCriteria criteria, String subWhere)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM {0}", entityName);

            // Generate an array of the WHERE clauses to be used.
            String[] where = WhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" WHERE {0}", subWhere);  
            }

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
            return sb.ToString();
        }

        #region ISelectBroker<TInput,TOutput> Members

        /// <summary>
        /// Load an entity based on the primary key.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
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
            SqlDataReader myReader = null;
            SqlCommand command = null;
            string sql = "";

            try
            {
                command = new SqlCommand();
                command.Connection = Context.Connection;
                command.CommandType = CommandType.Text;

                command.CommandText = sql = GetSql(_entityName, command, criteria, null);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    Platform.Log(LogLevel.Error, "Select statement: {0}", sql);

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
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sql);

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
