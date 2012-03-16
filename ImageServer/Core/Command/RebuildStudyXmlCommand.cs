#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Command
{
	public class RebuildStudyXmlCommand : CommandBase, IAggregateCommand
	{
		private readonly string _studyInstanceUid;
		private readonly string _rootPath;
	    private StudyXml _newXml;

		private readonly Stack<ICommand> _aggregateCommands = new Stack<ICommand>();

        public StudyXml StudyXml { get { return _newXml; } }

		public RebuildStudyXmlCommand(string studyInstanceUid, string rootPath) : base("Rebuild an existing Study XML file", true)
		{
			_studyInstanceUid = studyInstanceUid;
			_rootPath = rootPath;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			StudyXml currentXml = LoadStudyXml();

            _newXml = new StudyXml(_studyInstanceUid);
			foreach (SeriesXml series in currentXml)
			{
				string seriesPath = Path.Combine(_rootPath, series.SeriesInstanceUid);
                if (!Directory.Exists(seriesPath))
                {
                    Platform.Log(LogLevel.Info, "RebuildXML: series folder {0} is missing", seriesPath);
                    continue;
                }

			    foreach (InstanceXml instance in series)
			    {
			        string instancePath = Path.Combine(seriesPath, instance.SopInstanceUid + ServerPlatform.DicomFileExtension);
			        if (!File.Exists(instancePath))
			        {
                        Platform.Log(LogLevel.Info, "RebuildXML: file {0} is missing", instancePath);
			        }
			        else
			        {
                        if (!theProcessor.ExecuteSubCommand(this, new InsertInstanceXmlCommand(_newXml, instancePath)))
			                throw new ApplicationException(theProcessor.FailureReason);
			        }
			    }
			}

            if (!theProcessor.ExecuteSubCommand(this, new SaveXmlCommand(_newXml, _rootPath, _studyInstanceUid)))
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

		public Stack<ICommand> AggregateCommands
		{
			get { return _aggregateCommands; }
		}
	}
}
