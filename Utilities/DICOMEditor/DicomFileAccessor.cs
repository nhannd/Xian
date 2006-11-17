using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;

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
                DcmFileFormat fileFormat = new DcmFileFormat();
                //OFCondition cond = fileFormat.loadFile(dump.Filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, OffisDcm.DCM_MaxReadLength, E_FileReadMode.ERM_autoDetect);
                OFCondition cond = fileFormat.loadFile(dump.Filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_recalcGL, OffisDcm.DCM_MaxReadLength, false);
                DicomHelper.CheckReturnValue(cond, dump.Filename);
                fileFormat.loadAllDataIntoMemory();

                DcmMetaInfo metaInfo = fileFormat.getMetaInfo();
                DcmDataset dataSet = fileFormat.getDataset();

                foreach (EditItem editItem in dump.EditItems)
                {
                    DcmTag editTag = new DcmTag(editItem.EditTag.Group, editItem.EditTag.Element);
                    DcmEVR eVr = editTag.getVR().getEVR();
                    if (editItem.Type == EditType.Update || editItem.Type == EditType.Create)
                    {
                        switch (eVr)
                        {
                            case DcmEVR.EVR_UL:
                                dataSet.putAndInsertUint32(editTag, uint.Parse(editItem.EditTag.Value));
                                break;
                            case DcmEVR.EVR_US:
                                dataSet.putAndInsertUint16(editTag, ushort.Parse(editItem.EditTag.Value));
                                break;
                            case DcmEVR.EVR_SQ:
                                break;
                            default:
                                dataSet.putAndInsertString(editTag, editItem.EditTag.Value);
                                break;
                        }
                    }
                    else
                    {
                        dataSet.findAndDeleteElement(new DcmTagKey(editTag.getGroup(), editTag.getElement()), false, false);
                    }
                }
                fileFormat.saveFile(dump.Filename, E_TransferSyntax.EXS_Unknown);
                fileFormat.Dispose();   
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

                IntPtr ptr = IntPtr.Zero;

                if (elem != null)
                {
                    if (vr.getEVR() == DcmEVR.EVR_UL)
                    {
                        uint uint32Val;
                        elem.getUint32(out uint32Val);
                        value = uint32Val.ToString();
                    }

                    else if (vr.getEVR() == DcmEVR.EVR_US)
                    {
                        ushort ushortVal;
                        elem.getUint16(out ushortVal);
                        value = ushortVal.ToString();
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
                    else
                    {
                        elem.getString(ref ptr);
                        value = Marshal.PtrToStringAnsi(ptr);
                    }
                }

                //if (!tag.isUnknownVR())
                //{
                    dicomDump.Add(new DicomEditorTag(group, element, tagName, vrName, length, value, parent, displayLevel));
                //}

                obj = container.nextInContainer(obj);
            }
            return dicomDump;
        }
    }
}
