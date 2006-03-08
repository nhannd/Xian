namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data.SqlClient;
    using System.Data;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Data;
    using ClearCanvas.Dicom.OffisWrapper;
    using System.Transactions;

    /// <summary>
    /// Provides access to a SQL Server 2005 database for storing of DICOM object headers
    /// </summary>
    /// <example>
    /// <code>
    ///    class Program
    ///    {
    ///        static void Main(string[] args)
    ///        {
    ///            ApplicationEntity myself = new ApplicationEntity(new HostName(&quot;localhost&quot;), new AETitle(&quot;CCNETTEST&quot;), new ListeningPort(4000));
    ///            ApplicationEntity server = new ApplicationEntity(new HostName(&quot;clintondesk&quot;), new AETitle(&quot;CONQUESTSRV1&quot;), new ListeningPort(5678));
    ///
    ///            DicomClient client = new DicomClient(myself);
    ///
    ///            ReadOnlyQueryResultCollection results = client.Query(server, new PatientId(&quot;*001*&quot;));
    ///
    ///            if (results.Count &lt; 1)
    ///                return;
    ///
    ///            Program prog = new Program();
    ///
    ///            client.SopInstanceReceivedEvent += prog.NewImageEventHandler;
    ///
    ///            client.Retrieve(server, results[0].StudyInstanceUid, &quot;c:\\temp\\retrieveDB\\images&quot;);
    ///        }
    ///
    ///        void NewImageEventHandler(Object source, SopInstanceReceivedEventArgs args)
    ///        {
    ///            String connectionString = &quot;Data Source=CLINTONLAPTOP\\SQLEXPRESS;Initial Catalog=ripp_version5;User ID=sa;Password=root&quot;;
    ///
    ///            DateTime start = DateTime.Now;
    ///            DatabaseConnector db = new DatabaseConnector(connectionString);
    ///            db.StartImageInsertion();
    ///            db.InsertSopInstance(args.SopFileName);
    ///            db.StopImageInsertion();
    ///            DateTime stop = DateTime.Now;
    ///            TimeSpan duration = stop-start;
    ///
    ///            Console.WriteLine(&quot;{0} stored {1}&quot;, duration.ToString(), args.SopFileName);
    ///        }
    ///     }
    /// </code>
    /// </example>
    public class DatabaseConnector
    {
        public DatabaseConnector(ConnectionString connectionString)
        {
            _connection = new SqlConnection(connectionString.ToString());
            _state = ConnectorState.Constructed;
        }

        public void SetupConnector()
        {
            // TODO
            if (ConnectorState.Constructed != _state)
                throw new System.InvalidOperationException("Cannot invoke this operation when Connector instance is not in the 'Constructed' state");

            _connection.Open();
            _dicomMappingTable = new DicomMappingTable(_connection);

            _dataSet = new DataSet("RIPPDatabase");
            _sopInstanceAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM dbo.SopInstance", _connection);
            _seriesAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM dbo.Series", _connection);
            _studyAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM dbo.Study", _connection);
            _patientAdapter = new SqlDataAdapter("SELECT TOP 0 * FROM dbo.Patient", _connection);

            _sopInstanceAdapter.TableMappings.Add("Table", "SopInstance");
            _sopInstanceAdapter.FillSchema(_dataSet, SchemaType.Mapped);

            _seriesAdapter.TableMappings.Add("Table", "Series");
            _seriesAdapter.FillSchema(_dataSet, SchemaType.Mapped);

            _studyAdapter.TableMappings.Add("Table", "Study");
            _studyAdapter.FillSchema(_dataSet, SchemaType.Mapped);

            _patientAdapter.TableMappings.Add("Table", "Patient");
            _patientAdapter.FillSchema(_dataSet, SchemaType.Mapped);

            // check assumptions
            // TODO:
            if (_dataSet.Tables.Count < 4)
                throw new System.Exception("Too few tables");

            _state = ConnectorState.ReadyForOperations;
        }

        public void TeardownConnector()
        {
            // TODO
            if (ConnectorState.ReadyForOperations != _state)
                throw new System.InvalidOperationException("Cannot invoke this operation while the Connector instance is not in the 'ReadyForOperations' state");

            _connection.Close();
            _connection.Dispose();
            _dataSet.Dispose();
            _sopInstanceAdapter.Dispose();
            _seriesAdapter.Dispose();
            _studyAdapter.Dispose();
            _patientAdapter.Dispose();
            _state = ConnectorState.CeasedOperations;
        }

        public void InsertSopInstance(String fileName)
        {
            // TODO
            if (ConnectorState.ReadyForOperations != _state)
                throw new System.InvalidOperationException("Cannot invoke this operation while the Connector instance is not in the 'ReadyForOperations' state");

            DcmFileFormat file = new DcmFileFormat();
            OFCondition condition = file.loadFile(fileName);
            if (!condition.good())
            {
                // there was an error reading the file, possibly it's not a DICOM file
                return;
            }

            DcmMetaInfo metaInfo = file.getMetaInfo();
            DcmDataset dataset = file.getDataset();
            InsertSopInstance(metaInfo, dataset, "LocationUri", "file://" + fileName);

            // keep the file object alive until the end of this scope block
            // otherwise, it'll be GC'd and metaInfo and dataset will be gone
            // as well, even though they are needed in the InsertSopInstance
            // and sub methods
            GC.KeepAlive(file);
        }

        #region Protected Members

        protected void InsertSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, params String[] additionalColumnValues)
        {
            // check assumptions
            // TODO
            if ((additionalColumnValues.Length % 2) > 0)
                throw new System.Exception("Can't have odd length arguments, since there should be columnName/value pairs");

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    Int32 patientDatabaseId = InsertIntoPatient(metaInfo, sopInstanceDataset);
                    Int32 studyDatabaseId = InsertIntoStudy(metaInfo, sopInstanceDataset, patientDatabaseId);
                    Int32 seriesDatabaseId = InsertIntoSeries(metaInfo, sopInstanceDataset, studyDatabaseId);
                    Int32 sopDatabaseId = InsertIntoSopInstance(metaInfo, sopInstanceDataset, seriesDatabaseId, additionalColumnValues);
                }
                catch (SqlException ex)
                {

                    //You can specify additional error handling here
                }
                ts.Complete();
            }
        }

        protected Int32 InsertIntoSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, Int32 foreignKey, params String[] additionalColumnValues)
        {
            Int32 newSopInstanceForeignKey = InsertIntoTable(metaInfo, sopInstanceDataset, "SopInstance", foreignKey, additionalColumnValues);
            return newSopInstanceForeignKey;
        }

        protected Int32 InsertIntoSeries(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, Int32 foreignKey)
        {
            Int32 newSeriesForeignKey = InsertIntoTable(metaInfo, sopInstanceDataset, "Series", foreignKey);
            return newSeriesForeignKey;
        }

        protected Int32 InsertIntoStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, Int32 foreignKey)
        {
            Int32 newStudyForeignKey = InsertIntoTable(metaInfo, sopInstanceDataset, "Study", foreignKey);
            return newStudyForeignKey;
        }

        protected Int32 InsertIntoPatient(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            Int32 newPatientForeignKey = InsertIntoTable(metaInfo, sopInstanceDataset, "Patient");
            return newPatientForeignKey;
        }

        protected Int32 InsertIntoTable(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, String tableName, params String[] additionalColumnValues)
        {
            return InsertIntoTable(metaInfo, sopInstanceDataset, tableName, _unavailableForeignKey);
        }

        protected Int32 InsertIntoTable(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, String tableName, Int32 foreignKeyValue, params String[] additionalColumnValues)
        {
            String[] arrayString = ComputeSqlStatements(metaInfo, sopInstanceDataset, tableName, foreignKeyValue, additionalColumnValues);
            String insertCommandString = arrayString[0];
            String selectCommandString = arrayString[1];
            String updateCommandString = arrayString[2];

            SqlCommand getExistingIdentityCommand = new SqlCommand(selectCommandString, _connection);
            Int32 existingIdentity = Convert.ToInt32(getExistingIdentityCommand.ExecuteScalar());

            if (existingIdentity > 0)
            {
                return existingIdentity;
            }
            else
            {
                SqlCommand command = new SqlCommand(insertCommandString, _connection);
                command.ExecuteNonQuery();
                SqlCommand getIdentityCommand = new SqlCommand("SELECT SCOPE_IDENTITY()", _connection);
                Int32 newIdentity = Convert.ToInt32(getIdentityCommand.ExecuteScalar());
                return newIdentity;
            }
        }

        protected String CompleteUpdateCommand(String updateCommandString, Int32 existingIdentity, String tableName)
        {
            StringBuilder newUpdateCommandString = new StringBuilder(updateCommandString, 1024);
            newUpdateCommandString.AppendFormat(" WHERE {0} = {1}", _dataSet.Tables[tableName].Columns[0].ColumnName, existingIdentity);
            return updateCommandString.ToString();
        }

        /// <summary>
        /// Assumptions: first column (0) in a dataSet table MUST be the primary key, 
        /// second column (1) must be foreign key.
        /// </summary>
        /// <param name="sopInstanceDataset"></param>
        /// <param name="tableName"></param>
        /// <param name="foreignKeyValue">Foreign key value to be inserted in the row
        /// to keep it linked to another table.</param>
        /// <returns></returns>
        protected String[] ComputeSqlStatements(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, String tableName, Int32 foreignKeyValue, params String[] additionalColumnValues)
        {
            StringBuilder insertCommandString = new StringBuilder(1024);
            StringBuilder insertCommandStringFragment = new StringBuilder(1024);
            StringBuilder selectCommandString = new StringBuilder(1024);
            StringBuilder updateCommandString = new StringBuilder(1024);

            if (foreignKeyValue != _unavailableForeignKey)
            {
                insertCommandString.AppendFormat("INSERT INTO dbo.{0} ({1}", tableName, _dataSet.Tables[tableName].Columns[1].ColumnName);
                insertCommandStringFragment.AppendFormat(" VALUES({0}", foreignKeyValue);
                selectCommandString.AppendFormat("SELECT {0} FROM dbo.{1} WHERE ", _dataSet.Tables[tableName].Columns[0].ColumnName, tableName);
                updateCommandString.AppendFormat("UPDATE dbo.{0} SET {1} = {2}", tableName, _dataSet.Tables[tableName].Columns[1].ColumnName,
                    foreignKeyValue);

                // if we know for sure that there will be additional columns that 
                // need to be populated, then we can append a comma
                if (_dataSet.Tables[tableName].Columns.Count > 2)
                {
                    insertCommandString.Append(", ");
                    insertCommandStringFragment.Append(", ");
                    updateCommandString.Append(", ");
                }
            }
            else
            {
                insertCommandString.AppendFormat("INSERT INTO dbo.{0} (", tableName);
                insertCommandStringFragment.Append(" VALUES(");
                selectCommandString.AppendFormat("SELECT {0} FROM dbo.{1} WHERE ", _dataSet.Tables[tableName].Columns[0].ColumnName, tableName);
                updateCommandString.AppendFormat("UPDATE dbo.{0} SET ", tableName);
            }


            // for each column in the table, find one of 
            // (a) the corresponding path in the mapping table, or
            // (b) the matching value in the list of arguments passed in
            // if neither (a) nor (b) are available, then we skip to the 
            // next column in the table

            int numCols = _dataSet.Tables[tableName].Columns.Count;
            int index = 1;
            foreach (DataColumn col in _dataSet.Tables[tableName].Columns)
            {
                bool foundInMappingTable = _dicomMappingTable.Contains(new TagName(col.ColumnName));
                bool additionColumnValuesAvailable = (additionalColumnValues.Length > 0);

                Path path = new Path("");       // placeholder

                if (foundInMappingTable)
                {
                    path = _dicomMappingTable.GetColumn(new TagName(col.ColumnName)).Path;

                    if (index > 1)     // not first, so prepend comma
                    {
                        insertCommandString.Append(", ");
                        insertCommandStringFragment.Append(", ");
                        selectCommandString.Append(" AND ");
                        updateCommandString.Append(", ");
                    }

                    DicomTag tag = new DicomTag(path.GetPathElementAsInt32(0));
                    DcmTagKey key = new DcmTagKey(tag.Group, tag.Element);
                    StringBuilder dicomValue = new StringBuilder(256);

                    // for debugging only
                    //Console.WriteLine("Fetching {0}-{1}", key.toString(), col.ColumnName);
                    OFCondition cond = sopInstanceDataset.findAndGetOFString(key, dicomValue);
                        
                    // error code 0x0002 is Tag Not Found as of dcmtk v3.5.3
                    if (cond.bad() && cond.code() == 0x0002)        // tag is not available, check in meta info
                    {
                        // for debugging only
                        //Console.WriteLine("Checking metaInfo for {0}-{1}", key.toString(), col.ColumnName);
                        metaInfo.findAndGetOFString(key, dicomValue);
                    }

                    insertCommandString.AppendFormat("{0}", col.ColumnName);
                    insertCommandStringFragment.AppendFormat("'{0}'", dicomValue.ToString());
                    selectCommandString.AppendFormat("{0} = '{1}'", col.ColumnName, dicomValue.ToString());
                    updateCommandString.AppendFormat("{0} = '{1}'", col.ColumnName, dicomValue.ToString());
                    ++index;
                    continue;
                }
                else if (additionColumnValuesAvailable)
                {
                    for (int i = 0; i < additionalColumnValues.Length; ++i)
                    {
                        // scan looking at the columnNames
                        if ((i % 2) != 0)
                            continue;

                        if (additionalColumnValues[i] == col.ColumnName)     // found a match
                        {
                            if (index > 1)     // not first, so prepend comma
                            {
                                insertCommandString.Append(", ");
                                insertCommandStringFragment.Append(", ");
                                selectCommandString.Append(" AND ");
                                updateCommandString.Append(", ");
                            }

                            insertCommandString.AppendFormat("{0}", col.ColumnName);
                            insertCommandStringFragment.AppendFormat("'{0}'", additionalColumnValues[i+1]);
                            selectCommandString.AppendFormat("{0} = '{1}'", col.ColumnName, additionalColumnValues[i+1]);
                            updateCommandString.AppendFormat("{0} = '{1}'", col.ColumnName, additionalColumnValues[i+1]);

                            ++index;
                            break;
                        }
                    }

                    // didn't find the column name among the additionalColumnValues
                    // or if found, then already processed
                    continue;
                }
                else
                {
                    // skip this particular column, since it can neither be found
                    // in the mapping table nor in the additionalColumnValues
                    continue;
                }
            }

            insertCommandString.Append(") ");
            insertCommandStringFragment.Append(")");
            string insertCommand = insertCommandString.ToString() + insertCommandStringFragment;
            string selectCommand = selectCommandString.ToString();
            string updateCommand = updateCommandString.ToString();

            String[] arrayString = new String[3];
            arrayString[0] = insertCommand;
            arrayString[1] = selectCommand;
            arrayString[2] = updateCommand;

            return arrayString;
        }

#endregion

        #region Private Members

        /// <summary>
        /// Encapsulates the states that this type instance can be in
        /// Certain operations are not valid when the instance is in a given state
        /// S0: Unconstructed
        /// S1: Constructed
        /// S2: ReadyForOperations
        /// S3: CeasedOperations
        /// Instance construction: S0->S1
        /// SetupConnector(): S1->S2
        /// TeardownConnector(): S2->S3
        /// Once the instance is in S3, no other operation is valid
        /// </summary>
        private enum ConnectorState
        {
            Unconstructed,
            Constructed,
            ReadyForOperations,
            CeasedOperations
        }

        private SqlConnection _connection;
        private DataSet _dataSet;
        private SqlDataAdapter _sopInstanceAdapter;
        private SqlDataAdapter _seriesAdapter;
        private SqlDataAdapter _studyAdapter;
        private SqlDataAdapter _patientAdapter;
        private DicomMappingTable _dicomMappingTable;
        private readonly Int32 _unavailableForeignKey = -1;
        private ConnectorState _state = ConnectorState.Unconstructed;

        #endregion
    }
}
