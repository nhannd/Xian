using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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
        public DiscardImagesDescription Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<DiscardImagesDescription>(doc.DocumentElement);
            }
            else
            {
                DiscardImagesDescription desc = new DiscardImagesDescription();
                desc.Action = ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess.ReconcileAction.Discard;
                desc.Automatic = false;
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }
}
