using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{
    /// <summary>
    /// "Discard" xml parser.
    /// </summary>
    public class DiscardImagesCommandXmlParser
    {
        public ReconcileDiscardImagesDescriptor Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileDiscardImagesDescriptor>(doc.DocumentElement);
            }
            else
            {
                ReconcileDiscardImagesDescriptor desc = new ReconcileDiscardImagesDescriptor();
                desc.Action = StudyReconcileAction.Discard;
                desc.Automatic = false;
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }
}
