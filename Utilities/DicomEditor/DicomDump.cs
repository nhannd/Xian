using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomDump
    {
        public DicomDump(List<DicomEditorTag> Tags, FileDicomImage File)
        {
            _processedtaglist = new List<DicomEditorTag>();
            _changeset = new EditItemList();
            _rawtaglist = Tags;
            _file = File;
            ProcessTagEdits();
        }

        public string Filename
        {
            get { return _file.Filename; }
        }
        
        public FileDicomImage File
        {
            get { return _file; }
        }

        public IEnumerable<DicomEditorTag> DisplayTagList
        {
            get 
            { 
                _processedtaglist.Sort(delegate(DicomEditorTag one, DicomEditorTag two) { return one.SortKey(SortType.GroupElement).CompareTo(two.SortKey(SortType.GroupElement)); });
                return _processedtaglist;
            }
        }

        public IEnumerable<EditItem> EditItems
        {
            get { return _changeset.EditItems; }
        }

        public bool TagExists(string uidKey)
        {
            return _processedtaglist.Find(delegate (DicomEditorTag d) { return uidKey ==  d.UidKey; }) == null ? false : true;
        }

        public DicomEditorTag GetOriginalTag(string uidKey)
        {
            return _rawtaglist.Find(delegate(DicomEditorTag d) { return uidKey == d.UidKey; });
        }

        public void AddEditItem(EditItem item)
        {
            _changeset.ClearEditsForKey(item.UidKey);
           if (TagExistsInRaw(item.UidKey))
            {
                if (item.Type == EditType.Create)
                {
                    item.Type = EditType.Update;
                }
            }
            else
            {
                if (item.Type != EditType.Delete)
                {
                    if (item.Type == EditType.Update)
                    {
                        item.Type = EditType.Create;
                    }
                }
            }
            _changeset.InsertEditItem(item);
            ProcessTagEdits();
        }

        public void RevertEdits()
        {
            _changeset.Clear();
            ProcessTagEdits();
        }

        public void RemovePrivateTags()
        {
            IEnumerable<DicomEditorTag> startTagList = new List<DicomEditorTag>(this.DisplayTagList);
            foreach (DicomEditorTag currentTag in startTagList)
            {
                if (currentTag.Group % 2 != 0)
                {
                    this.AddEditItem(new EditItem(currentTag, EditType.Delete));
                }
            }
        }

        public void Save()
        {
            DicomFileAccessor accessor = new DicomFileAccessor();
            accessor.SaveDicomFiles(new DicomDump[1] { this });

            _rawtaglist.Clear();
            foreach (DicomEditorTag tag in this.DisplayTagList)
            {
                _rawtaglist.Add(new DicomEditorTag(tag));
            }
            _changeset.Clear();
        }

        private void ProcessTagEdits()
        {
            DicomEditorTag tag = null;
            _processedtaglist.Clear();
            //_processedtaglist.AddRange(_rawtaglist);

            foreach (DicomEditorTag rawTag in _rawtaglist)
            {
                _processedtaglist.Add(new DicomEditorTag(rawTag));
            }

            if (!(_changeset.Count == 0))
            {
                foreach (EditItem i in _changeset.EditItems)
                {
                    switch (i.Type)
                    {
                        case EditType.Create:
                            _processedtaglist.Add(i.EditTag);
                            AdjustGroupLengths(i.EditTag.Group, i.UidKey, 8 + i.EditTag.Length);
                            break;
                        case EditType.Update:
                            tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.UidKey.Equals(i.UidKey); });
                            //if ((_rawtaglist.Find(delegate(DicomTag d) { return d.Key.Equals(i.Key); }) != null) || tag.Key != null)
                            //{
                            if (tag != null)
                            {
                                _processedtaglist.Remove(tag);
                                _processedtaglist.Add(i.EditTag);
                                AdjustGroupLengths(i.EditTag.Group, i.UidKey, i.EditTag.Length - tag.Length);
                            }
                            else
                            {
                                _processedtaglist.Add(i.EditTag);
                                AdjustGroupLengths(i.EditTag.Group, i.UidKey, i.EditTag.Length);
                            }
                            //}
                            break;
                        case EditType.Delete:
                            tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.UidKey == i.UidKey; });
                            while (tag != null)
                            {
                                //if ((_rawtaglist.Find(delegate(DicomTag d) { return d.Key.Equals(i.Key); }) != null) || tag.Key != null)
                                //{
                                _processedtaglist.Remove(tag);
                                AdjustGroupLengths(tag.Group, i.UidKey, -8 - tag.Length);
                                //}
                                tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.UidKey == i.UidKey; });
                            }
                            break;
                    }
                }
                //_processedtaglist.Sort(delegate(DicomTag one, DicomTag two) { return one.Key.SortKey.CompareTo(two.Key.SortKey); });
            }
        }

        private void AdjustGroupLengths(ushort Group, string uidKey, int LengthChange)
        {
            
            string groupLengthKey = DicomEditorTag.GenerateUidSearchKey(Group, 0, null);
            DicomEditorTag tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.UidKey == groupLengthKey; });
            if (tag != null)
            {
                int groupLength = int.Parse(tag.Value) + LengthChange;
                tag.Value = groupLength.ToString();
            }
        }

        private bool TagExistsInRaw(string uidKey)
        {
            return _rawtaglist.Find(delegate(DicomEditorTag d) { return uidKey == d.UidKey; }) == null ? false : true;
        }
        
        private List<DicomEditorTag> _rawtaglist;
        private List<DicomEditorTag> _processedtaglist;
        private EditItemList _changeset;
        private FileDicomImage _file;
        
    }
}
