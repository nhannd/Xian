using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class DeletedStudyController
    {
        public void Delete(ServerEntityKey recordKey)
        {
            DeleteStudyRecordAdaptor adaptor = new DeleteStudyRecordAdaptor();
            adaptor.Delete(recordKey);
        }
    }
}
