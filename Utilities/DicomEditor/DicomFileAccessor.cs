using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomFileAccessor
    {
        public DicomFileAccessor()
        {
        }


        public void SaveDicomFiles(DicomDump[] dumps)
        {
            
            foreach(DicomDump dump in dumps)
            {
                foreach (EditItem editItem in dump.EditItems)
                {
                    DcmTag editTag = new DcmTag(editItem.EditTag.Group, editItem.EditTag.Element);
                    DcmEVR eVr = editTag.getVR().getEVR();
                    if (editItem.Type == EditType.Update || editItem.Type == EditType.Create)
                    {
                        switch (eVr)
                        {
                            case DcmEVR.EVR_UL:
                                dump.File.Dataset.putAndInsertUint32(editTag, uint.Parse(editItem.EditTag.Value));
                                break;
                            case DcmEVR.EVR_US:
                                dump.File.Dataset.putAndInsertUint16(editTag, ushort.Parse(editItem.EditTag.Value));
                                break;
                            case DcmEVR.EVR_SQ:
                                break;
                            default:
                                dump.File.Dataset.putAndInsertString(editTag, editItem.EditTag.Value);
                                break;
                        }
                    }
                    else
                    {
                        dump.File.Dataset.findAndDeleteElement(new DcmTagKey(editTag.getGroup(), editTag.getElement()), false, false);
                    }
                }
                dump.File.WriteToMedia(E_TransferSyntax.EXS_Unknown);  
            }
        }

        public DicomDump LoadDicomDump(FileDicomImage File)
        {
            File.Dataset.loadAllDataIntoMemory();

            List<DicomEditorTag> dicomDump = null;

            dicomDump = GetDicomTags(File.MetaInfo.nextInContainer(null), File.MetaInfo, null, 0);
            dicomDump.AddRange(GetDicomTags(File.Dataset.nextInContainer(null), File.Dataset, null, 0));

            return new DicomDump(dicomDump, File);
        }

        private List<DicomEditorTag> GetDicomTags(DcmObject obj, DcmObject container, string parent, DisplayLevel displayLevel)
        {
            string tagName;
            ushort group;
            ushort element;
            DcmVR vr = null;
            string vrName;
            int length;
            string value = null;
            uint vm;

            DcmTag tag = null;
            List<DicomEditorTag> dicomDump = new List<DicomEditorTag>();

            while (obj != null)
            {
                tag = obj.getTag();
                group = tag.getGroup();
                element = tag.getElement();
                tagName = tag.getTagName();
                vr = tag.getVR();
                vrName = vr.getVRName();
                length = (int)obj.getLength();                

                DcmElement elem = OffisDcm.castToDcmElement(obj);
                vm = elem.getVM();                

                if (length == 0)
                {
                    value = "";
                }
                else
                {                
                    if (elem != null)
                    {
                        if (vr.getEVR() == DcmEVR.EVR_UL)
                        {
                            if (vm == 1)
                            {
                                uint uint32Val;
                                elem.getUint32(out uint32Val);
                                value = uint32Val.ToString();
                            }
                            else
                            {
                                IntPtr usptr = IntPtr.Zero;
                                elem.getUint32Array(ref usptr);
                                int arrayLength = (int)vm * 4;

                                byte[] bytes = this.ReadRawTag(usptr, arrayLength);
                                string[] display = new string[vm];

                                for (int j = 0; j <= vm - 1; j++)
                                {
                                    display[j] = BitConverter.ToUInt32(bytes, 4*j).ToString();                                
                                }
                                value = StringUtilities.Combine<string>(display, @"\");
                            }
                        }

                        else if (vr.getEVR() == DcmEVR.EVR_US)
                        {
                            if (vm == 1)
                            {
                                ushort ushortVal;
                                elem.getUint16(out ushortVal);
                                value = ushortVal.ToString();
                            }
                            else
                            {
                                IntPtr usptr = IntPtr.Zero;
                                elem.getUint16Array(ref usptr);
                                int arrayLength = (int)vm * 2;

                                byte[] bytes = this.ReadRawTag(usptr, arrayLength);
                                string[] display = new string[vm];

                                for (int j = 0; j <= vm - 1; j++)
                                {
                                    display[j] = BitConverter.ToUInt16(bytes, 2 * j).ToString();
                                }

                                value = StringUtilities.Combine<string>(display, @"\");
                            }
                        }
                        else if (vr.getEVR() == DcmEVR.EVR_SQ)
                        {
                            value = "";
                            DcmObject sequenceItemObject = elem.nextInContainer(null);
                            while (sequenceItemObject != null)
                            {
                                DcmTag sequenceItem = sequenceItemObject.getTag();
                                //adding the Item
                                DicomEditorTag item = new DicomEditorTag(sequenceItem.getGroup(), 
                                                             sequenceItem.getElement(),
                                                             sequenceItem.getTagName(),
                                                             null,
                                                             0,
                                                             null,
                                                             String.Format("({0:x4},", group) + String.Format("{0:x4})", element),
                                                             DisplayLevel.SequenceItem);
                                dicomDump.Add(item);

                                //traversing the tags within the Sequence Item
                                foreach (DicomEditorTag sequenceTags in GetDicomTags(sequenceItemObject.nextInContainer(null), sequenceItemObject, item.Key.ParentKeyString.Trim() + item.Key.DisplayKey.Trim(), DisplayLevel.SequenceItemAttribute))
                                {
                                    dicomDump.Add(new DicomEditorTag(sequenceTags));
                                }
                                                           
                                sequenceItemObject = sequenceItemObject.nextInContainer(sequenceItemObject);
                            }
                        }
                        else if (vr.getEVR() == DcmEVR.EVR_UNKNOWN || vr.getEVR() == DcmEVR.EVR_OB)
                        {
                            IntPtr uknptr = IntPtr.Zero;
                            elem.getUint8Array(ref uknptr);

                            //22 was chosen to match the output of the Offis dcmdump.exe utility
                            if (length <= 22) 
                            {
                                value = StringUtilities.Combine<byte>(this.ReadRawTag(uknptr, length), @"\", delegate(byte b) { return String.Format("{0:x2}", b); });
                            }
                            else
                            {
                                value = StringUtilities.Combine<byte>(this.ReadRawTag(uknptr, 22), @"\", delegate(byte b) { return String.Format("{0:x2}", b); }) + "...";
                            }
                        }                        
                        else
                        {
                            IntPtr ptr = IntPtr.Zero;
                            elem.getString(ref ptr);
                            value = Marshal.PtrToStringAnsi(ptr);
                        }
                    }
                }

                dicomDump.Add(new DicomEditorTag(group, element, tagName, vrName, length, value, parent, displayLevel));
                

                obj = container.nextInContainer(obj);
            }
            return dicomDump;
        }

        private byte[] ReadRawTag(IntPtr ptr, int length)
        {
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);

            return bytes;
        }
    }
}
