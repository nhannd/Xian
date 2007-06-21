using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// Class representing a DICOM Message to be transferred over the network.
    /// </summary>
    public class DicomMessage : AbstractMessage
    {
        public AttributeCollection CommandSet
        {
            get { return MetaInfo; }
        }

        public SopClass SopClass
        {
            get
            {
                String sopClassUid = base.DataSet[DicomTags.SOPClassUID].ToString();

                return SopClass.GetSopClass(sopClassUid);
            }
        }
    }
}
