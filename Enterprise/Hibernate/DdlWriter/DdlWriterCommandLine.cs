#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Ddl;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    class DdlWriterCommandLine : CommandLine
    {
		public enum FormatOptions
		{
			sql,
			xml
		}

        private bool _autoIndexForeignKeys;
        private bool _createIndexes;
        private EnumOptions _enumOption = EnumOptions.all;
        private string _outputFile;
    	private bool _qualifyNames = true;
    	private FormatOptions _format = FormatOptions.sql;
    	private string _baselineModelFile;

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

		[CommandLineParameter("q", "Specifies whether to qualify names of database objects. Default is true.")]
		public bool QualifyNames
		{
			get { return _qualifyNames; }
			set { _qualifyNames = value; }
		}

        [CommandLineParameter("enums", "e", "Specifies whether to populate enumerations.  Possible values are 'all', 'hard' or 'none'.  If omitted, the default is 'all'")]
        public EnumOptions EnumOption
        {
            get { return _enumOption; }
            set { _enumOption = value; }
        }

        [CommandLineParameter("out", "Specifies the name of the ouput file.  If omitted, output is written to stdout.")]
        public string OutputFile
        {
            get { return _outputFile; }
            set { _outputFile = value; }
        }

		[CommandLineParameter("format", "f", "Specifies output format.  Possible values are 'sql' and 'xml'.  If omitted, the default is 'sql'")]
		public FormatOptions Format
    	{
			get { return _format; }
			set { _format = value; }
    	}

		[CommandLineParameter("baseline", "b", "Specifies the name of a file that contains the model to upgrade from, in xml format.")]
		public string BaselineModelFile
		{
			get { return _baselineModelFile; }
			set { _baselineModelFile = value; }
		}
	}
}
