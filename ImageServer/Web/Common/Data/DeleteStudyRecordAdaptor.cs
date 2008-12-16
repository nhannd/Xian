using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    class DeleteStudyRecordAdaptor : BaseAdaptor<StudyDeleteRecord,
        IStudyDeleteRecordEntityBroker, StudyDeleteRecordSelectCriteria, StudyDeleteRecordUpdateColumns>
    {
    }
}
