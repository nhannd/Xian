using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Discard;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    public class StudyReconcileDescriptorParser
    {
        public StudyReconcileDescriptor Parse(XmlDocument doc)
        {
            if (doc.DocumentElement != null)
            {
                //TODO: use plugin?
                if (doc.DocumentElement.Name == "MergeStudy")
                {
                    MergeStudyCommandXmlParser parser = new MergeStudyCommandXmlParser();
                    return parser.Parse(doc);
                }
                else if (doc.DocumentElement.Name == "CreateStudy")
                {
                    CreateStudyCommandXmlParser parser = new CreateStudyCommandXmlParser();
                    return parser.Parse(doc);
                }
                else if (doc.DocumentElement.Name == "Discard")
                {
                    DiscardImagesCommandXmlParser parser = new DiscardImagesCommandXmlParser();
                    return parser.Parse(doc);
                }
                else if (doc.DocumentElement.Name == "Reconcile")
                {
                    return XmlUtils.Deserialize<StudyReconcileDescriptor>(doc.DocumentElement);
                }
                else
                {
                    throw new NotSupportedException(String.Format("Command: {0}", doc.DocumentElement.Name));
                }

            }
            return null;

        }
    }
}
