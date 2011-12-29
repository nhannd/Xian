#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// ServerCommand for inserting a DICOM File into the persistent store.
	/// </summary>
	public class InsertInstanceCommand : ServerDatabaseCommand
	{
		#region Private Members

		private readonly DicomFile _file;
		private readonly StudyStorageLocation _storageLocation;
		private InstanceKeys _insertKey;
		#endregion

		#region Public Properties
		public InstanceKeys InsertKeys
		{
			get { return _insertKey; }
		}
		#endregion

		public InsertInstanceCommand(DicomFile file, StudyStorageLocation location)
			: base("Insert Instance into Database", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			Platform.CheckForNullReference(location, "Study Storage Location");

			_file = file;
			_storageLocation = location;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
		{
			// Setup the insert parameters
			var parms = new InsertInstanceParameters();
			_file.LoadDicomFields(parms);
			parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
			parms.StudyStorageKey = _storageLocation.Key;

            // Get any extensions that exist and process them
		    var ep = new ProcessorInsertExtensionPoint();
		    var extensions = ep.CreateExtensions();
            foreach (IInsertExtension e in extensions)
                e.InsertExtension(parms, _file);
            
			// Get the Insert Instance broker and do the insert
			var insert = updateContext.GetBroker<IInsertInstance>();

			if (_file.DataSet.Contains(DicomTags.SpecificCharacterSet))
			{
				string cs = _file.DataSet[DicomTags.SpecificCharacterSet].ToString();
				parms.SpecificCharacterSet = cs;
			}

			_insertKey = insert.FindOne(parms);

			// If the Request Attributes Sequence is in the dataset, do an insert.
			if (_file.DataSet.Contains(DicomTags.RequestAttributesSequence))
			{
				var attribute = _file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
				if (!attribute.IsEmpty)
				{
					foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[]) attribute.Values)
					{
						var requestParms = new RequestAttributesInsertParameters();
						sequenceItem.LoadDicomFields(requestParms);
						requestParms.SeriesKey = _insertKey.SeriesKey;

						var insertRequest = updateContext.GetBroker<IInsertRequestAttributes>();
						insertRequest.Execute(requestParms);
					}
				}
			}
		}
	}
}