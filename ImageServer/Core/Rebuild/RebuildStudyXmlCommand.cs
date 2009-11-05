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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Rebuild
{
	public class RebuildStudyXmlCommand : ServerCommand, IAggregateServerCommand
	{
		private readonly string _studyInstanceUid;
		private readonly string _rootPath;

		private readonly Stack<IServerCommand> _aggregateCommands = new Stack<IServerCommand>();

		public RebuildStudyXmlCommand(string studyInstanceUid, string rootPath) : base("Rebuild an existing Study XML file", true)
		{
			_studyInstanceUid = studyInstanceUid;
			_rootPath = rootPath;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			StudyXml currentXml = LoadStudyXml();

			StudyXml newXml = new StudyXml(_studyInstanceUid);
			foreach (SeriesXml series in currentXml)
			{
				string seriesPath = Path.Combine(_rootPath, series.SeriesInstanceUid);
				foreach (InstanceXml instance in series)
				{
					string instancePath = Path.Combine(seriesPath, instance.SopInstanceUid + ServerPlatform.DicomFileExtension);

					if (!theProcessor.ExecuteSubCommand(this, new InsertInstanceXmlCommand(newXml, instancePath)))
						throw new ApplicationException(theProcessor.FailureReason);
				}
			}

			if (!theProcessor.ExecuteSubCommand(this, new SaveXmlCommand(newXml, _rootPath, _studyInstanceUid)))
				throw new ApplicationException(theProcessor.FailureReason);
		}

		protected override void OnUndo()
		{
			// No specific undo, rollback of the sub-commands handles it.
		}

		/// <summary>
		/// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
		/// </summary>
		/// <returns>The <see cref="StudyXml"/> instance for the study</returns>
		private StudyXml LoadStudyXml()
		{
			StudyXml theXml = new StudyXml();

			String streamFile = Path.Combine(_rootPath, _studyInstanceUid + ".xml");
			if (File.Exists(streamFile))
			{
				using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
				{
					XmlDocument theDoc = new XmlDocument();

					StudyXmlIo.Read(theDoc, fileStream);

					theXml.SetMemento(theDoc);

					fileStream.Close();
				}
			}

			return theXml;
		}

		public Stack<IServerCommand> AggregateCommands
		{
			get { return _aggregateCommands; }
		}
	}
}
