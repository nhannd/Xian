#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core.CommandProcessor;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using UpdateStudySizeInDBCommand = ClearCanvas.ImageServer.Core.CommandProcessor.UpdateStudySizeInDBCommand;

namespace ClearCanvas.ImageServer.Core.Rebuild
{
	/// <summary>
	/// Class for rebuilding StudyXml files based on the current contents of the <see cref="StudyXml"/> file.
	/// </summary>
	public class StudyXmlRebuilder
	{
		#region Private Members

		private readonly StudyStorageLocation _location;

        #endregion

        #region Constructor

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="location">The location of the Study to rebuild.</param>
		public StudyXmlRebuilder(StudyStorageLocation location)
		{
			_location = location;
		}

		#endregion

		/// <summary>
		/// Do the actual rebuild.  On error, will attempt to reprocess the study.
		/// </summary>
		public void RebuildXml()
		{
			string rootStudyPath = _location.GetStudyPath();

			try
			{
				using (ServerCommandProcessor processor = new ServerCommandProcessor("Rebuild XML"))
				{
				    var command = new RebuildStudyXmlCommand(_location.StudyInstanceUid, rootStudyPath);
					processor.AddCommand(command);

                    var updateCommand = new UpdateStudySizeInDBCommand(_location, command);
                    processor.AddCommand(updateCommand);

					if (!processor.Execute())
					{
						throw new ApplicationException(processor.FailureReason, processor.FailureException);
					}

                    Study theStudy = _location.Study;
                    if (theStudy.NumberOfStudyRelatedInstances != command.StudyXml.NumberOfStudyRelatedInstances)
                    {
                        // We rebuilt, but the counts don't match.
                        throw new StudyIntegrityValidationFailure(ValidationErrors.InconsistentObjectCount,
                                                                  new ValidationStudyInfo(theStudy,
                                                                                          _location.ServerPartition),
                                                                  string.Format(
                                                                      "Database study count {0} does not match study xml {1}",
                                                                      theStudy.NumberOfStudyRelatedInstances,
                                                                      command.StudyXml.NumberOfStudyRelatedInstances));
                    }

					Platform.Log(LogLevel.Info, "Completed reprocessing Study XML file for study {0}", _location.StudyInstanceUid);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected error when rebuilding study XML for directory: {0}",
				             _location.FilesystemPath);
				StudyReprocessor reprocessor = new StudyReprocessor();
                try
                {
                    WorkQueue reprocessEntry = reprocessor.ReprocessStudy("Rebuild StudyXml", _location, Platform.Time);
                    if (reprocessEntry != null)
                    {
                        Platform.Log(LogLevel.Error, "Failure attempting to reprocess study: {0}",
                                     _location.StudyInstanceUid);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Inserted reprocess request for study: {0}",
                                     _location.StudyInstanceUid);
                }
                catch(InvalidStudyStateOperationException ex)
                {
                    Platform.Log(LogLevel.Error, "Failure attempting to reprocess study {0}: {1}",
                                     _location.StudyInstanceUid, ex.Message);
                }
			}
		}
	}
}