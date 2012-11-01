#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.ModelExtensions
{
    public static class WorkQueueExtensions
    {
        /// <summary>
        /// Indicates whether or not this WQI will result in patient/study information change.
        /// This usually indicates if the operation can be safely deleted from the system without any major consequences.
        /// </summary>
        public static bool WillResultInDataChanged(this WorkQueue item)
        {
            var harmlessWQITypes = new[]{
                WorkQueueTypeEnum.AutoRoute,
                WorkQueueTypeEnum.CompressStudy, // not changing patient/study info 
                WorkQueueTypeEnum.PurgeStudy, // nearline or online
                WorkQueueTypeEnum.MigrateStudy,
                WorkQueueTypeEnum.WebMoveStudy
            };

            return !harmlessWQITypes.Contains(item.WorkQueueTypeEnum);
        }
    }
}
