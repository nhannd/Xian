using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Queue.Work
{
    public class InsertInstanceCommand : ServerCommand
    {
        #region Private Members

        private IReadContext _read;
        private DicomFile _file;
        private StudyStorageLocation _storageLocation;
        #endregion

        public InsertInstanceCommand(IReadContext read, DicomFile file, StudyStorageLocation location)
            : base("Insert Instance into Database")
        {
            Platform.CheckForNullReference(read, "File name");
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(location, "Study Storage Location");

            _file = file;
            _storageLocation = location;
            _read = read;
        }

        public override void Execute()
        {
            // Setup the insert parameters
            InstanceInsertParameters parms = new InstanceInsertParameters();
            _file.DataSet.LoadDicomFields(parms);
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.StatusEnum = StatusEnum.GetEnum("Online");

            // Get the Insert Instance broker and do the insert
            IInsertInstance insert = _read.GetBroker<IInsertInstance>();
            IList<InstanceKeys> keys = insert.Execute(parms);

            // If the Request Attributes Sequence is in the dataset, do an insert.
            if (_file.DataSet.Contains(DicomTags.RequestAttributesSequence))
            {
                DicomAttributeSQ attribute = _file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
                if (!attribute.IsEmpty)
                {
                    foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[])attribute.Values)
                    {
                        RequestAttributesInsertParameters requestParms = new RequestAttributesInsertParameters();
                        sequenceItem.LoadDicomFields(requestParms);
                        requestParms.SeriesKey = keys[0].SeriesKey;

                        IInsertRequestAttributes insertRequest = _read.GetBroker<IInsertRequestAttributes>();
                        insertRequest.Execute(requestParms);
                    }
                }
            }
        }

        public override void Undo()
        {

        }
    }
}
