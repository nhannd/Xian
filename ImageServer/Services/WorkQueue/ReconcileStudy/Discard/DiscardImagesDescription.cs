using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard
{
    [XmlRoot("Reconcile")]
    public class DiscardImagesDescription : ReconcileDescription
    {
        public DiscardImagesDescription():base()
        {
            Action = ReconcileAction.Discard;
        }
    }
}
