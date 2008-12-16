using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    class DdlWriterCommandLine : CommandLine
    {
        public enum EnumOptions
        {
            all,
            hard,
            none
        }

        private bool _autoIndexForeignKeys;
        private bool _createIndexes;
        private EnumOptions _populateEnumerations = EnumOptions.all;
        private string _outputFile;

        [CommandLineParameter("fki", "Specifies whether to auto-index all foreign keys.  Ignored unless /index is also specified.")]
        public bool AutoIndexForeignKeys
        {
            get { return _autoIndexForeignKeys; }
            set { _autoIndexForeignKeys = value; }
        }

        [CommandLineParameter("index", "ix", "Specifies whether to generate database indexes.")]
        public bool CreateIndexes
        {
            get { return _createIndexes; }
            set { _createIndexes = value; }
        }

        [CommandLineParameter("enums", "e", "Specifies whether to populate enumerations.  Possible values are 'all', 'hard' or 'none'.  If omitted, the default is 'all'")]
        public EnumOptions PopulateEnumerations
        {
            get { return _populateEnumerations; }
            set { _populateEnumerations = value; }
        }

        [CommandLineParameter("out", "Specifies the name of the ouput file.  If omitted, output is written to stdout.")]
        public string OutputFile
        {
            get { return _outputFile; }
            set { _outputFile = value; }
        }
    }
}
