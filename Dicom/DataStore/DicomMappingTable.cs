namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data.SqlClient;
    using System.Data;
    using System.Xml.XPath;
    using System.IO;
    using System.Collections.Generic;

    public class DicomMappingTable
    {
        public DicomMappingTable(Url urlDataSource)
        {
            throw new Exception("Sorry, this is not yet implemented");
        }

        public DicomMappingTable(ConnectionString connectionStringDataSource)
        {
            SqlConnection connection = new SqlConnection(connectionStringDataSource.ToString());
            _connection = connection;
            SetupDatabaseConnection();
        }

        public DicomMappingTable(SqlConnection connection)
        {
            _connection = connection;
            SetupDatabaseConnection();
        }

        public bool Contains(TagName tagName)
        {
            return _tagNameToColumnDictionary.ContainsKey(tagName.ToString());
        }

        public bool Contains(Path path)
        {
            return _pathToColumnDictionary.ContainsKey(path.ToString());
        }

        public ColumnDescriptor GetColumn(TagName tagName)
        {
            if (_tagNameToColumnDictionary.ContainsKey(tagName.ToString()))
                return _tagNameToColumnDictionary[tagName.ToString()];
            else
                throw new Exception("Fix this!");
        }

        public ColumnDescriptor GetColumn(Path path)
        {
            if (_pathToColumnDictionary.ContainsKey(path.ToString()))
                return _pathToColumnDictionary[path.ToString()];
            else
                throw new Exception("Fix this!");
        }

        protected void InitializeDictionary(XPathDocument xml)
        {

            XPathNavigator nav = xml.CreateNavigator();

            if (!nav.HasChildren)
                return;

            if (!nav.MoveToFirstChild()) // document
                return;

            if (!nav.MoveToFirstChild()) // NewDataSet
                return;

            while (true)
            {
                if (!nav.MoveToFirstChild())    // Table
                    return;

                if (!nav.MoveToNext())  // DicomTag
                    return;

                if (!nav.MoveToFirstChild()) // TagName value
                    return;

                string name = nav.Value;    // get TagName value

                nav.MoveToParent();

                if (!nav.MoveToNext())  // Path
                    return;

                if (!nav.MoveToFirstChild())
                    return;

                string path = nav.Value;    // get Path value

                nav.MoveToParent();

                if (!nav.MoveToNext())  // IsComputed
                    return;

                if (!nav.MoveToFirstChild())
                    return; 

                bool isComputed = nav.ValueAsBoolean;    // get IsComputed value

                // done extracting XML tag data
                // put everything together
                ColumnDescriptor column = new ColumnDescriptor(new TagName(name), new Path(path), isComputed);
                _tagNameToColumnDictionary.Add(name, column);
                _pathToColumnDictionary.Add(path, column);

                // move to the next table
                nav.MoveToParent();

                nav.MoveToParent();    // back to enclose Table 

                if (!nav.MoveToNext())    // next Table
                    return;
            }
        }

        protected void SetupDatabaseConnection()
        {
            // initialize dictionary objects
            _tagNameToColumnDictionary = new Dictionary<string, ColumnDescriptor>();
            _pathToColumnDictionary = new Dictionary<string, ColumnDescriptor>();

            // get data from SQL database into XML form
            string selectCommand = "SELECT * FROM DicomTag";
            DataSet dictionaryData = new DataSet();
            SqlDataAdapter dictionaryAdapter = new SqlDataAdapter(selectCommand, _connection);
            dictionaryAdapter.Fill(dictionaryData);

            // debug
            dictionaryData.WriteXml("c:\\temp\\dicomtag.xml");

            // into the XML path object
            StringReader reader = new StringReader(dictionaryData.GetXml());
            XPathDocument xmlRepresentation = new XPathDocument(reader);
            InitializeDictionary(xmlRepresentation);
        }

        private Dictionary<string, ColumnDescriptor> _tagNameToColumnDictionary;
        private Dictionary<string, ColumnDescriptor> _pathToColumnDictionary;
        private SqlConnection _connection;
    }
}
