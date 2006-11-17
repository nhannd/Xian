using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClearCanvas.Utilities.DICOMEditor
{
    internal class EditorAccessor
    {
        //commented out while Debugging
        //protected void GetTags(string fileName) 
        internal DicomTagList GetDump()
        {
            string fileName = @"E:\IMG\64\64CharPatientID.dcm";
            DcmFileFormat fileFormat = new DcmFileFormat();
            OFCondition cond = fileFormat.loadFile(fileName, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, OffisDcm.DCM_MaxReadLength, false);
            if (!cond.good())
            {
                return new DicomTagList();
            }

            DcmDataset dataSet = fileFormat.getDataset();
            DcmObject obj = null;
            obj = dataSet.nextInContainer(null);
            DcmTag tag = null;
 
            string strTagName;
            string strGroupElement;
            string strVr;
            string strLength;
            string strValue = null;

            DicomTagList dicomDump = new DicomTagList();

            while (obj != null)
            {
                tag = obj.getTag();
                strGroupElement = tag.toString();
                strTagName = tag.getTagName();
                DcmVR vr = tag.getVR();
                strVr = vr.getVRName();
                strLength = obj.getLength().ToString();

                DcmElement elem = OffisDcm.castToDcmElement(obj);

                //StringBuilder buffer = new StringBuilder(256);

                IntPtr ptr = IntPtr.Zero;

                if (elem != null)
                {
                    if (vr.getEVR() == DcmEVR.EVR_UL)
                    {
                        uint uint32Val;
                        elem.getUint32(out uint32Val);
                        strValue = uint32Val.ToString();
                    }
                    else if (vr.getEVR() == DcmEVR.EVR_US)
                    {
                        ushort ushortVal;
                        elem.getUint16(out ushortVal);
                        strValue = ushortVal.ToString();
                    }
                    else 
                    {
                        elem.getString(ref ptr);
                        strValue = Marshal.PtrToStringAnsi(ptr);
                    }
                }

                int length = 20;
                if (strTagName.Length > length)
                    strTagName = strTagName.Substring(0, length);
                else
                    strTagName = strTagName.PadRight(length, ' ');

                length = 5;
                if (strLength.Length > length)
                    strLength = strLength.Substring(0, length);
                else
                    strLength = strLength.PadRight(length, ' ');

                if (false == tag.isUnknownVR())
                {
                    DicomTag dumpedTag = new DicomTag();
                    dumpedTag.GroupElement = strGroupElement;
                    dumpedTag.TagName = strTagName;
                    dumpedTag.Vr = strVr;
                    dumpedTag.Length = strLength;
                    dumpedTag.Value = strValue;

                    dicomDump.Add(dumpedTag);
                    Console.WriteLine("{0} {1} {2} {3} {4}", strGroupElement, strTagName, strVr, strLength, strValue);
                    //Console.WriteLine("{0} {1} {2} {3} {4}", dicomDump., strTagName, strVr, strLength, strValue);
                }
                    
                obj = dataSet.nextInContainer(obj);
            }
            return dicomDump;
        }
    }
}
