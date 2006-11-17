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
                _processedtaglist.Sort(delegate(DicomEditorTag one, DicomEditorTag two) { return one.Key.SortKey.CompareTo(two.Key.SortKey); });
                return _processedtaglist;
            }
        }

        public IEnumerable<EditItem> EditItems
        {
            get { return _changeset.EditItems; }
        }

        public bool TagExists(DicomEditorTagKey Key)
        {
            return _processedtaglist.Find(delegate (DicomEditorTag d) { return Key.Equals(d.Key); }) == null ? false : true;
        }

        public DicomEditorTag GetOriginalTag(DicomEditorTagKey Key)
        {
            //retire this soon - use the actual object
            return _rawtaglist.Find(delegate(DicomEditorTag d) { return Key.Equals(d.Key); });
        }

        public void AddEditItem(EditItem item)
        {
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
                            AdjustGroupLengths(i.EditTag.Group, i.Key, 8 + i.EditTag.Length);
                            break;
                        case EditType.Update:
                            tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.Key.Equals(i.Key); });
                            //if ((_rawtaglist.Find(delegate(DicomTag d) { return d.Key.Equals(i.Key); }) != null) || tag.Key != null)
                            //{
                            if (tag != null)
                            {
                                _processedtaglist.Remove(tag);
                                _processedtaglist.Add(i.EditTag);
                                AdjustGroupLengths(i.EditTag.Group, i.Key, i.EditTag.Length - tag.Length);
                            }
                            else
                            {
                                _processedtaglist.Add(i.EditTag);
                                AdjustGroupLengths(i.EditTag.Group, i.Key, i.EditTag.Length);
                            }
                            //}
                            break;
                        case EditType.Delete:
                            tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.Key.Equals(i.Key); });
                            //if ((_rawtaglist.Find(delegate(DicomTag d) { return d.Key.Equals(i.Key); }) != null) || tag.Key != null)
                            //{
                                _processedtaglist.Remove(tag);
                                AdjustGroupLengths(tag.Group, i.Key, -8 - tag.Length);
                            //}
                            break;
                    }
                }
                //_processedtaglist.Sort(delegate(DicomTag one, DicomTag two) { return one.Key.SortKey.CompareTo(two.Key.SortKey); });
            }
        }

        private void AdjustGroupLengths(ushort Group, DicomEditorTagKey Key, int LengthChange)
        {
            
            DicomEditorTagKey groupLengthKey = new DicomEditorTagKey(Group, 0, Key.ParentKeyString, Key.DisplayLevel);
            DicomEditorTag tag = _processedtaglist.Find(delegate(DicomEditorTag d) { return d.Key.Equals(groupLengthKey); });
            if (tag != null)
            {
                int groupLength = int.Parse(tag.Value) + LengthChange;
                tag.Value = groupLength.ToString();
            }
        }
        
        private List<DicomEditorTag> _rawtaglist;
        private List<DicomEditorTag> _processedtaglist;
        private EditItemList _changeset;
        private FileDicomImage _file;
        
    }
}
