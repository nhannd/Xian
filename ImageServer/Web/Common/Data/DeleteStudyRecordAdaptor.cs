#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
