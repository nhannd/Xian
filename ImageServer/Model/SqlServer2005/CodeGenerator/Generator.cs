using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Enterprise;

namespace CodeGenerator
{
    public class Generator
    {
        private class Column
        {
            private readonly string _columnName;
            private readonly Type _columnType;
            private readonly string _variableName;

            public Column(string name, string type)
            {
                _columnName = name.Replace("GUID", "Key");
                _variableName = String.Format("_{0}{1}", _columnName.Substring(0, 1).ToLower(), _columnName.Substring(1));

                if (type.Equals("nvarchar"))
                    _columnType = typeof(String);
                else if (type.Equals("varchar"))
                    _columnType = typeof(String);
                else if (type.Equals("smallint"))
                    _columnType = typeof(short);
                else if (type.Equals("uniqueidentifier"))
                    _columnType = typeof(ServerEntityKey);
                else if (type.Equals("bit"))
                    _columnType = typeof(bool);
                else if (type.Equals("int"))
                    _columnType = typeof(int);
                else if (type.Equals("datetime"))
                    _columnType = typeof(DateTime);
                else if (type.Equals("xml"))
                    _columnType = typeof(XmlDocument);
                else if (type.Equals("decimal"))
                    _columnType = typeof(Decimal);
                else
                    throw new ApplicationException("Unexpected SQL Server type: " + type);
            }

            public string ColumnName
            {
                get { return _columnName; }
            }

            public Type ColumnType
            {
                get { return _columnType; }
            }

            public string VariableName
            {
                get { return _variableName; }
            }
        }

        private class Table
        {
            private readonly string _tableName;
            private readonly List<Column> _columnList = new List<Column>();

            public Table(string name)
            {
                _tableName = name;
            }

            public IList<Column> Columns
            {
                get { return _columnList; }
            }

            public string TableName
            {
                get { return _tableName; }
            }
        }

        #region Private Members
        private readonly List<Table> _tableList = new List<Table>();
        private string _imageServerModelFolder;
        private string _namespace;
        private readonly string _selectCriteriaFolder = Path.Combine("Model", "Criteria");
        private readonly string _entityBrokerFolder = Path.Combine("Model", "EntityBrokers");

        private string _entityInterfaceFolder;
        private string _entityInterfaceNamespace;
        private string _entityImplementationNamespace;
        private string _entityImplementationFolder;
        #endregion

        #region Public Properties
        public string ModelNamespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        private List<Table> TableList
        {
            get { return _tableList; }
        }

        public string SelectCriteriaFolder
        {
            get { return _selectCriteriaFolder; }
        }

        public string EntityBrokerFolder
        {
            get { return _entityBrokerFolder; }
        }

        public string ImageServerModelFolder
        {
            get { return _imageServerModelFolder; }
            set { _imageServerModelFolder = value; }
        }

        public string EntityInterfaceFolder
        {
            get { return _entityInterfaceFolder; }
            set { _entityInterfaceFolder = value; }
        }

        public string EntityInterfaceNamespace
        {
            get { return _entityInterfaceNamespace; }
            set { _entityInterfaceNamespace = value; }
        }

        public string EntityImplementationNamespace
        {
            get { return _entityImplementationNamespace; }
            set { _entityImplementationNamespace = value; }
        }

        public string EntityImplementationFolder
        {
            get { return _entityImplementationFolder; }
            set { _entityImplementationFolder = value; }
        }

        #endregion

        public void LoadTableInfo()
        {
            SqlConnection connection;
            connection = new SqlConnection();

            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ImageServerConnectString"];

            connection.ConnectionString = settings.ConnectionString;
            connection.Open();


            DataTable dataTable = connection.GetSchema("Tables", new string[] { null, "dbo", null, null });

            DataColumn colTableName = dataTable.Columns["TABLE_NAME"];
            if (colTableName != null)
                foreach (DataRow row in dataTable.Rows)
                {

                    String tableName = row[colTableName].ToString();
                    if (!tableName.StartsWith("sys"))
                    {
                        try
                        {
                            Table table = new Table(tableName);

                            DataTable dt = connection.GetSchema("Columns", new string[] { null, null, tableName });

                            DataColumn colColumnName = dt.Columns["COLUMN_NAME"];
                            DataColumn colColumnType = dt.Columns["DATA_TYPE"];

                            foreach (DataRow row2 in dt.Rows)
                            {
                                table.Columns.Add(
                                    new Column(row2[colColumnName].ToString(), row2[colColumnType].ToString()));
                            }

                            TableList.Add(table);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unexpected exception when processing table: " + e.Message);
                        }
                    }
                }
        }

        private void WriterHeader(StreamWriter writer, string nameSpace)
        {
            writer.WriteLine("#region License");
            writer.WriteLine("");
            writer.WriteLine("// Copyright (c) 2006-2009, ClearCanvas Inc.");
            writer.WriteLine("// All rights reserved.");
            writer.WriteLine("//");
            writer.WriteLine("// Redistribution and use in source and binary forms, with or without modification, ");
            writer.WriteLine("// are permitted provided that the following conditions are met:");
            writer.WriteLine("//");
            writer.WriteLine("//    * Redistributions of source code must retain the above copyright notice, ");
            writer.WriteLine("//      this list of conditions and the following disclaimer.");
            writer.WriteLine("//    * Redistributions in binary form must reproduce the above copyright notice, ");
            writer.WriteLine("//      this list of conditions and the following disclaimer in the documentation ");
            writer.WriteLine("//      and/or other materials provided with the distribution.");
            writer.WriteLine("//    * Neither the name of ClearCanvas Inc. nor the names of its contributors ");
            writer.WriteLine("//      may be used to endorse or promote products derived from this software without ");
            writer.WriteLine("//      specific prior written permission.");
            writer.WriteLine("//");
            writer.WriteLine("// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" ");
            writer.WriteLine("// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, ");
            writer.WriteLine("// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR ");
            writer.WriteLine("// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR ");
            writer.WriteLine("// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, ");
            writer.WriteLine("// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE ");
            writer.WriteLine("// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) ");
            writer.WriteLine("// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, ");
            writer.WriteLine("// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ");
            writer.WriteLine("// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY ");
            writer.WriteLine("// OF SUCH DAMAGE.");
            writer.WriteLine("");
            writer.WriteLine("#endregion");
            writer.WriteLine("");
            writer.WriteLine("// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.");
            writer.WriteLine("");
            writer.WriteLine("namespace {0}", nameSpace);
            writer.WriteLine("{");
        }

        private void WriteFooter(StreamWriter writer)
        {
            writer.WriteLine("}");
        }

        private void WriteEnumBrokerInterfaceFile(Table table)
        {
            String fileName = String.Format("I{0}Broker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(this.EntityInterfaceFolder, fileName));

            WriterHeader(writer, this.EntityInterfaceNamespace);

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("    public interface I{0}Broker: IEnumBroker<{0}>", table.TableName);
            writer.WriteLine("    { }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEnumBrokerImplementationFile(Table table)
        {
            String fileName = String.Format("{0}Broker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(this.EntityImplementationFolder, fileName));

            WriterHeader(writer, this.EntityImplementationNamespace);

            writer.WriteLine("    using ClearCanvas.Common;");
			writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise.SqlServer2005;");
            writer.WriteLine("");

            writer.WriteLine("    [ExtensionOf(typeof(BrokerExtensionPoint))]");
            writer.WriteLine("    public class {0}Broker : EnumBroker<{0}>, I{0}Broker", table.TableName);
            writer.WriteLine("    { }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEnumFile(Table table)
        {
            String fileName = String.Format("{0}.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(ImageServerModelFolder, fileName));

            WriterHeader(writer, ModelNamespace);

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Collections.Generic;");
            writer.WriteLine("    using {0};",EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("    using System.Reflection;");
            writer.WriteLine("");

            writer.WriteLine("[Serializable]");
            writer.WriteLine("public partial class {0} : ServerEnum", table.TableName);
            writer.WriteLine("{");
           
            WritePredefinedEnums(table, writer);

            writer.WriteLine("      #region Constructors");
            writer.WriteLine("      public {0}():base(\"{0}\")", table.TableName);
            writer.WriteLine("      {}");
            writer.WriteLine("      #endregion");
            writer.WriteLine("      #region Public Members");
            writer.WriteLine("      public override void SetEnum(short val)");
            writer.WriteLine("      {");
            writer.WriteLine("          ServerEnumHelper<{0}, I{0}Broker>.SetEnum(this, val);", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      static public List<{0}> GetAll()", table.TableName);
            writer.WriteLine("      {");
            writer.WriteLine("          return ServerEnumHelper<{0}, I{0}Broker>.GetAll();", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      static public {0} GetEnum(string lookup)", table.TableName);
            writer.WriteLine("      {");
            writer.WriteLine("          return ServerEnumHelper<{0}, I{0}Broker>.GetEnum(lookup);", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      #endregion");

            writer.WriteLine("}");


            WriteFooter(writer);

            writer.Close();
        }

        private void WritePredefinedEnums(Table table, StreamWriter writer)
        {
            Dictionary<string, string> _enumValues = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ImageServerConnectString"];
                connection.ConnectionString = settings.ConnectionString;
                connection.Open();

                StringBuilder cmd = new StringBuilder();
                cmd.AppendFormat("SELECT * FROM {0}", table.TableName);
                SqlCommand cmdSelect = new SqlCommand(cmd.ToString(), connection);

                SqlDataReader myReader = cmdSelect.ExecuteReader();
                while (myReader.Read())
                {
                    string lookup = (string)myReader["Lookup"];
                    string longDescription = (string)myReader["LongDescription"];

                    _enumValues.Add(lookup, longDescription);
                }

                writer.WriteLine("      #region Private Static Members");
                foreach (string lookupValue in _enumValues.Keys)
                {
                    string fieldName = lookupValue.Replace(" ", "");
                    writer.WriteLine("      private static readonly {0} _{1} = GetEnum(\"{2}\");", table.TableName, fieldName, lookupValue);
                }
                writer.WriteLine("      #endregion");
                writer.WriteLine("");

                writer.WriteLine("      #region Public Static Properties");
                foreach (string lookupValue in _enumValues.Keys)
                {
                    string fieldName = lookupValue.Replace(" ", "");
                    string description = _enumValues[lookupValue];

                    writer.WriteLine("      /// <summary>");
                    writer.WriteLine("      /// {0}", description);
                    writer.WriteLine("      /// </summary>");
                    writer.WriteLine("      public static {0} {1}", table.TableName, fieldName);
                    writer.WriteLine("      {");
                    writer.WriteLine("          get {{ return _{0}; }}", fieldName);
                    writer.WriteLine("      }");
                }
                
                writer.WriteLine("");
            }
            writer.WriteLine("      #endregion");
            writer.WriteLine("");
        }

        private void WriteEntityBrokerInterfaceFile(Table table)
        {
            String fileName = String.Format("I{0}EntityBroker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(this.EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
			writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("");

            writer.WriteLine("public interface I{0}EntityBroker : IEntityBroker<{0}, {0}SelectCriteria, {0}UpdateColumns>", table.TableName);
            writer.WriteLine("{ }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntityBrokerImplementationFile(Table table)
        {
            String fileName = String.Format("{0}EntityBroker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(this.EntityImplementationFolder, fileName));

            WriterHeader(writer, EntityImplementationNamespace);

            writer.WriteLine("    using ClearCanvas.Common;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
			writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise.SqlServer2005;");
            writer.WriteLine("");

            writer.WriteLine("    [ExtensionOf(typeof(BrokerExtensionPoint))]");
            writer.WriteLine("    public class {0}Broker : EntityBroker<{0}, {0}SelectCriteria, {0}UpdateColumns>, I{0}EntityBroker", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        public {0}Broker() : base(\"{0}\")", table.TableName);
            writer.WriteLine("        { }");
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }
        private void WriteModelFile(Table table)
        {
            String fileName = String.Format("{0}.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);
            StreamWriter writer = new StreamWriter(Path.Combine(ImageServerModelFolder, fileName));

            WriterHeader(writer, ModelNamespace);

            writer.WriteLine("    using System;");
            bool bDicomReference = false;
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                    {
                        bDicomReference = true;
                        break;
                    }
                }
            }
            if (bDicomReference)
                writer.WriteLine("    using ClearCanvas.Dicom;");
            writer.WriteLine("    using ClearCanvas.Enterprise.Core;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
			writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("");

            writer.WriteLine("    [Serializable]");
            writer.WriteLine("    public partial class {0}: ServerEntity", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        #region Constructors");
            writer.WriteLine("        public {0}():base(\"{0}\")", table.TableName);
            writer.WriteLine("        {}");
            writer.WriteLine("        #endregion");
            writer.WriteLine("");
            writer.WriteLine("        #region Private Members");
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    if (col.ColumnName.EndsWith("Enum"))
                        writer.WriteLine("        private {0} {1};", col.ColumnName, col.VariableName);
                    else
                        writer.WriteLine("        private {0} {1};", col.ColumnType, col.VariableName);
                }
            }
            writer.WriteLine("        #endregion");
            writer.WriteLine("");
            writer.WriteLine("        #region Public Properties");
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                        writer.WriteLine("        [DicomField(DicomTags.{0}, DefaultValue = DicomFieldDefault.Null)]", col.ColumnName);

                    writer.WriteLine("        [EntityFieldDatabaseMappingAttribute(TableName=\"{0}\", ColumnName=\"{1}\")]",table.TableName, col.ColumnName.Replace("Key", "GUID"));

                    if (col.ColumnName.EndsWith("Enum"))
                        writer.WriteLine("        public {0} {1}", col.ColumnName, col.ColumnName);
                    else
                        writer.WriteLine("        public {0} {1}", col.ColumnType, col.ColumnName);
                    writer.WriteLine("        {");
                    writer.WriteLine("        get {{ return {0}; }}", col.VariableName);
                    writer.WriteLine("        set {{ {0} = value; }}", col.VariableName);
                    writer.WriteLine("        }");
                }
            }
            writer.WriteLine("        #endregion");
            writer.WriteLine("");
            writer.WriteLine("        #region Static Methods");
            writer.WriteLine("        static public {0} Load(ServerEntityKey key)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())");
            writer.WriteLine("            {");
            writer.WriteLine("                return Load(read, key);");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("        static public {0} Load(IReadContext read, ServerEntityKey key)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            I{0}EntityBroker broker = read.GetBroker<I{0}EntityBroker>();", table.TableName);
            writer.WriteLine("            {0} theObject = broker.Load(key);", table.TableName);
            writer.WriteLine("            return theObject;");
            writer.WriteLine("        }");
            writer.WriteLine("        #endregion");
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntitySelectCriteriaFile(Table table)
        {
            String fileName = String.Format("{0}SelectCriteria.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            writer.WriteLine("    using ClearCanvas.Enterprise.Core;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("    public partial class {0}SelectCriteria : EntitySelectCriteria", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        public {0}SelectCriteria()", table.TableName);
            writer.WriteLine("        : base(\"{0}\")", table.TableName);
            writer.WriteLine("        {}");

            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    string colType = col.ColumnName.EndsWith("Enum") ? col.ColumnName : col.ColumnType.ToString();
                    string colName = col.ColumnName;
                    writer.WriteLine("        public ISearchCondition<{0}> {1}", colType, colName);
                    writer.WriteLine("        {");
                    writer.WriteLine("            get");
                    writer.WriteLine("            {");
                    writer.WriteLine("              if (!SubCriteria.ContainsKey(\"{0}\"))", colName);
                    writer.WriteLine("              {");
                    writer.WriteLine("                 SubCriteria[\"{0}\"] = new SearchCondition<{1}>(\"{0}\");", colName, colType);
                    writer.WriteLine("              }");
                    writer.WriteLine("              return (ISearchCondition<{0}>)SubCriteria[\"{1}\"];", colType, colName);
                    writer.WriteLine("            } ");
                    writer.WriteLine("        }");
                }
            }
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntityUpdateColumnsFile(Table table)
        {
            String fileName = String.Format("{0}UpdateColumns.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(this.EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            bool bDicomReference = false;
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                    {
                        bDicomReference = true;
                        break;
                    }
                }
            }
            if (bDicomReference)
                writer.WriteLine("    using ClearCanvas.Dicom;");

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("   public class {0}UpdateColumns : EntityUpdateColumns", table.TableName);
            writer.WriteLine("   {");
            writer.WriteLine("       public {0}UpdateColumns()", table.TableName);
            writer.WriteLine("       : base(\"{0}\")", table.TableName);
            writer.WriteLine("       {}");

            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    string colType = col.ColumnName.EndsWith("Enum") ? col.ColumnName : col.ColumnType.ToString();
                    string colName = col.ColumnName;
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                        writer.WriteLine("       [DicomField(DicomTags.{0}, DefaultValue = DicomFieldDefault.Null)]", colName);

                    writer.WriteLine("        public {0} {1}", colType, colName);
                    writer.WriteLine("        {");
                    writer.WriteLine(
                        "            set {{ SubParameters[\"{0}\"] = new EntityUpdateColumn<{1}>(\"{0}\", value); }}", colName,
                        colType);
                    writer.WriteLine("        }");
                }
            }
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        public void Generate()
        {
            LoadTableInfo();

            foreach (Table table in TableList)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Working with table: {0}", table.TableName);
                Console.WriteLine("");

                if (table.TableName.EndsWith("Enum"))
                {
                    WriteEnumBrokerInterfaceFile(table);
                    WriteEnumBrokerImplementationFile(table);
                    WriteEnumFile(table);
                }
                else
                {
                    WriteEntityBrokerInterfaceFile(table);
                    WriteEntitySelectCriteriaFile(table);
                    WriteEntityUpdateColumnsFile(table);
                    WriteEntityBrokerImplementationFile(table);

                    WriteModelFile(table);
                }


            }
            Console.WriteLine("Done!");
        }
    }
}
