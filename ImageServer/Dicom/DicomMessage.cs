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

        public DicomMessage(AttributeCollection command, AttributeCollection data) : base()
        {
            base._metaInfo = command;
            base._dataSet = data;
        }

        public DicomMessage(DicomFile file)
        {
            base._metaInfo = new AttributeCollection();
            base._dataSet = file.DataSet;
        }
    }
}
