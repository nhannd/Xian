using System;
using System.IO;

using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public delegate void WriterStatusChangedEventHandler(String Description);

    class DdlWriter
    {
        enum CreateFileType
        {
            Create,
            Drop
        };

        #region Fields
        private String _createSchemaFileName;
        private String _dropSchemaFileName;
        private Configuration _cfg;
        #endregion

        #region Properties

        public String CreateSchemaFileName
        {
            get { return _createSchemaFileName; }
            set { _createSchemaFileName = value; }
        }

        public String DropSchemaFileName
        {
            get { return _dropSchemaFileName; }
            set { _dropSchemaFileName = value; }
        }

        #endregion

        #region Events
        public event WriterStatusChangedEventHandler Changed;
        #endregion

        #region Constructors

        public DdlWriter()
        {
            _createSchemaFileName = "CreateTables.ddl";
            _dropSchemaFileName = "DropTables.ddl";

            PersistentStore store = new PersistentStore();
            store.Initialize();
            _cfg = store.Configuration;
        }

        public DdlWriter(String createSchemaFileName, String dropSchemaFileName)
        {
            _createSchemaFileName = createSchemaFileName;
            _dropSchemaFileName = dropSchemaFileName;

            PersistentStore store = new PersistentStore();
            store.Initialize();
            _cfg = store.Configuration;
        }

        #endregion

        #region Methods

        public void Execute()
        {
            OnChanged("Starting DDL Writer...");

            CreateFile(CreateFileType.Create, _createSchemaFileName);
            OnChanged("Create Schema Script created...");

            CreateFile(CreateFileType.Drop, _dropSchemaFileName);
            OnChanged("Drop Schema Script created...");

            OnChanged("Finished");
        }

        private void CreateFile(CreateFileType fileType, String fileName)
        {
            string[] ddl;
            ddl = fileType == CreateFileType.Create ?
                _cfg.GenerateSchemaCreationScript(NHibernate.Dialect.MsSql2000Dialect.GetDialect()) :
                _cfg.GenerateDropSchemaScript(NHibernate.Dialect.MsSql2000Dialect.GetDialect());
               
            StreamWriter sw;
            sw = File.CreateText(fileName);

            foreach (string s in ddl)
            {
                sw.WriteLine(s);
            }

            sw.Close();
        }

        protected void OnChanged(String description)
        {
            if (Changed != null)
            {
                Changed(description);
            }
        }

        #endregion
    }
}
