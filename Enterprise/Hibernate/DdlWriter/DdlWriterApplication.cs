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
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DdlWriterApplication : IApplicationRoot
    {
        public void RunApplication(string[] args)
        {
            DdlWriterCommandLine cmdLine = new DdlWriterCommandLine();
            try
            {
                cmdLine.Parse(args);

                // if a file name was supplied, write to the file
                if (!string.IsNullOrEmpty(cmdLine.OutputFile))
                {
                    using (StreamWriter sw = File.CreateText(cmdLine.OutputFile))
                    {
                        WriteOutput(sw, cmdLine);
                    }
                }
                else
                {
                    // by default write to stdout
                    WriteOutput(Console.Out, cmdLine);
                }
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
            }
        }

        private void WriteOutput(TextWriter writer, DdlWriterCommandLine cmdLine)
        {
            try
            {
				// load the persistent store defined by the current set of binaries
				PersistentStore store = (PersistentStore)PersistentStoreRegistry.GetDefaultStore();

				// run pre-processors
				PreProcessor preProcessor = new PreProcessor(cmdLine.CreateIndexes, cmdLine.AutoIndexForeignKeys);
				preProcessor.Process(store);

				// get config and dialect
				Configuration config = store.Configuration;
            	Dialect dialect = Dialect.GetDialect(config.Properties);

				// if this is an upgrade, load the baseline model file
            	RelationalModelInfo baselineModel = null;
				if(!string.IsNullOrEmpty(cmdLine.BaselineModelFile))
				{
					RelationalModelSerializer serializer = new RelationalModelSerializer(config, dialect);
					baselineModel = serializer.ReadModel(File.OpenText(cmdLine.BaselineModelFile));
				}

				switch(cmdLine.Format)
				{
					case DdlWriterCommandLine.FormatOptions.sql:

						// create script writer and set properties based on command line 
						ScriptWriter scriptWriter = new ScriptWriter(config, dialect);
						scriptWriter.EnumOption = cmdLine.EnumOption;
						scriptWriter.QualifyNames = cmdLine.QualifyNames;
						scriptWriter.BaselineModel = baselineModel;

						// decide whether to write a creation or upgrade script, depending on if a baseline was supplied
						if(baselineModel == null)
							scriptWriter.WriteCreateScript(writer);
						else
							scriptWriter.WriteUpgradeScript(writer);

						break;

					case DdlWriterCommandLine.FormatOptions.xml:

						// we don't currently support outputting upgrades in XML format
						if(baselineModel != null)
							throw new NotSupportedException("Upgrade is not compatible with XML output format.");

						RelationalModelSerializer serializer = new RelationalModelSerializer(config, dialect);
						serializer.WriteModel(writer);
						break;

					default:
						throw new NotSupportedException(string.Format("{0} is not a valid output format.", cmdLine.Format));
				}
            }
            catch (Exception e)
            {
                Log(e.Message, LogLevel.Error);
            }
        }

        private void Log(object obj, LogLevel level)
        {
            Platform.Log(LogLevel.Error, obj);
            Console.WriteLine(obj);
        }
    }
}
