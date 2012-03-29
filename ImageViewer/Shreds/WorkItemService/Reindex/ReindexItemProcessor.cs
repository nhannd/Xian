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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex
{
    public class ReindexItemProcessor : BaseItemProcessor<ReindexRequest, ReindexProgress>
    {
        public List<string> FilesToImport { get; set; }


        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
            
            FilesToImport = new List<string>();
            
            return initResult;
        }

        public override void Process()
        {
            if (CancelPending)
            {
                Proxy.Cancel();
                return;
            }
            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            Progress.IsCancelable = false;
            Proxy.UpdateProgress();

            var processor = new ClearCanvas.ImageViewer.Dicom.Core.ReindexProcessor();

            processor.Initialize();

            Progress.NumberOfStudiesToProcess = processor.DatabaseStudiesToScan + processor.StudyFoldersToScan;
            Proxy.UpdateProgress();

            processor.Process();
            
            Proxy.Complete();
        }

       

        public override bool CanStart(out string reason)
        {
            reason = string.Empty;
            // TODO: Check for ReIndex pending job?
            return true;
        }
    }
}
