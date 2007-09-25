using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISopInstance
    {
		Uid GetSopInstanceUid();
		Uid GetSopClassUid();
		Uid GetTransferSyntaxUid();

		ISeries GetParentSeries();

		DicomUri GetLocationUri();
    }
}
