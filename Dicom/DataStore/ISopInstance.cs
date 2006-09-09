using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISopInstance
    {
        Uid GetTransferSyntaxUid();
        Uid GetSopClassUid();
        Uid GetSopInstanceUid();
        bool IsIdenticalTo(ISopInstance sop);
        void SetParentSeries(ISeries series);
        DicomUri GetLocationUri();
        ISeries GetParentSeries();
    }
}
