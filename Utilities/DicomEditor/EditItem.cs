using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class EditItemList
    {
        public EditItemList()
        {
            _editItemList = new List<EditItem>();
        }

        public void InsertEditItem(EditItem item)
        {
            foreach (EditItem i in _editItemList.FindAll(delegate(EditItem ei) { return ei.Key.Equals(item.Key); }))
            {
                _editItemList.Remove(i);
            }
            _editItemList.Add(item);

            _editItemList.Sort(delegate(EditItem one, EditItem two) { return one.Key.SortKey.CompareTo(two.Key.SortKey); });
        }

        public IEnumerable<EditItem> EditItems
        {
            get { return _editItemList; }
        }

        public void Clear()
        {
            _editItemList.Clear();
        }

        public void ClearEditsForKey(DicomEditorTagKey Key)
        {
            EditItem item = _editItemList.Find(delegate(EditItem e) { return e.Key.Equals(Key); });
            if (item != null)
            {
                _editItemList.Remove(item);
            }
        }

        public int Count
        {
            get { return _editItemList.Count; }
        }

        private List<EditItem> _editItemList;
    }

    public class EditItem
    {
        public EditItem(DicomEditorTag EditTag, EditType Type)
        {
            _editTag = EditTag;
            _editType = Type;
            _key = EditTag.Key;
        }

        public DicomEditorTagKey Key
        {
            get { return _key; }
        }

        public DicomEditorTag EditTag
        {
            get { return _editTag; }
            set { _editTag = value; }
        }

        public EditType Type
        {
            get { return _editType; }
            set { _editType = value; }
        }

        private DicomEditorTagKey _key;
        private DicomEditorTag _editTag;
        private EditType _editType;

        //private void UpdateTypeEditItem(EditItem updateTagItem)
        //{
        //    EditItem existingItem = null;
        //    _updateScenarios = UpdateScenarios.NewItem;

        //    if (this.Contains(updateTagItem)) //checks for same object
        //    {
        //        _updateScenarios = UpdateScenarios.ItemExistsInList;
        //    }
        //    else
        //    {
        //        foreach (EditItem i in this)
        //        {
        //            if (i.GroupElement.Equals(updateTagItem.GroupElement))
        //            {
        //                if (EditItem.AreEquivalent(i, updateTagItem))
        //                {
        //                    _updateScenarios = UpdateScenarios.EquivalentItemExistsInList;
        //                }
        //                else
        //                {
        //                    switch (i.Type)
        //                    {
        //                        case EditType.Create:
        //                            existingItem = i;
        //                            _updateScenarios = UpdateScenarios.UpdateCreateItem;
        //                            break;

        //                        case EditType.Update:
        //                            if (updateTagItem.Type == EditType.Delete)
        //                            {
        //                                existingItem = i;
        //                                _updateScenarios = UpdateScenarios.NetResultIsDelete;
        //                            }
        //                            else if (updateTagItem.NewTag.Value == i.OriginalTag.Value)
        //                            {
        //                                existingItem = i;
        //                                _updateScenarios = UpdateScenarios.TagBeingResetBackToOriginalValue;
        //                            }
        //                            else
        //                            {
        //                                existingItem = i;
        //                                _updateScenarios = UpdateScenarios.ChangeToExistingItem;
        //                            }
        //                            break;

        //                        case EditType.Delete:
        //                            if (updateTagItem.NewTag.Value == i.OriginalTag.Value)
        //                            {
        //                                existingItem = i;
        //                                _updateScenarios = UpdateScenarios.ReverseDelete;
        //                            }
        //                            else
        //                            {
        //                                existingItem = i;
        //                                _updateScenarios = UpdateScenarios.ReverseDeleteAndUpdate;
        //                            }
        //                            break;

        //                        default:
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        switch (_updateScenarios)
        //        {
        //            case UpdateScenarios.NewItem:
        //                this.Add(updateTagItem);
        //                //Fire Event to indicate a Change (so that we can change the row colour)
        //                break;

        //            case UpdateScenarios.UpdateCreateItem:
        //                updateTagItem.OriginalTag = existingItem.OriginalTag;
        //                this.Remove(existingItem);
        //                this.Add(updateTagItem);
        //                //Fire Event to indicate a Change (so that we can refresh table)
        //                break;

        //            case UpdateScenarios.TagBeingResetBackToOriginalValue:
        //                this.Remove(existingItem);
        //                //Fire Event to indicate a Change (so that we can change the row colour back to white)
        //                break;

        //            case UpdateScenarios.ChangeToExistingItem:
        //                updateTagItem.OriginalTag = existingItem.OriginalTag;
        //                this.Remove(existingItem);
        //                this.Add(updateTagItem);
        //                //Fire Event to indicate a Change (so that we can refresh table)
        //                break;

        //            case UpdateScenarios.NetResultIsDelete:
        //                updateTagItem.OriginalTag = existingItem.OriginalTag;
        //                updateTagItem.Type = EditType.Delete;
        //                this.Remove(existingItem);
        //                this.Add(updateTagItem);
        //                //Fire Event to indicate a Change (so that we can refresh table)
        //                break;

        //            case UpdateScenarios.ReverseDelete:
        //                this.Remove(existingItem);
        //                //Fire Event to indicate a Change (so that we can refresh table)
        //                break;

        //            case UpdateScenarios.ReverseDeleteAndUpdate:
        //                updateTagItem.OriginalTag = existingItem.OriginalTag;
        //                updateTagItem.Type = EditType.Update;
        //                this.Remove(existingItem);
        //                this.Add(updateTagItem);
        //                break;

        //            case UpdateScenarios.EquivalentItemExistsInList:
        //            case UpdateScenarios.ItemExistsInList:
        //                //intentional no op
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //}

    }

    public enum EditType
    {
        Create,
        Update,
        Delete
    }

    //public enum UpdateScenarios
    //{
    //    NewItem,                            // 1->2                                         1 edititem req
    //    UpdateCreateItem,                   // *->1,1->2 = *->2                             2 edititems req
    //    TagBeingResetBackToOriginalValue,   // 1->2,2->1 = Reset - no change to type        0 edititems req
    //    ChangeToExistingItem,               // 1->2,2->3 = 1->3 no change to type           1 edititems req (delete original)
    //    ItemExistsInList,                   // 1->2,1->2 = 1->2                             dup edititems (resolved if all matching are deleted first)
    //    EquivalentItemExistsInList,         // 1->2,3->2 = 1->2                             dup edititems (resolved if all matching are deleted first)
    //    NetResultIsDelete,                  // 1->2,2->* = 1->* - change type to Delete     1 edititems req (resolved if all matching are deleted first)
    //    ReverseDelete,                      // 1->*,*->1 = 0 - remove from list             becomes change back (if all matching are deleted first)
    //    ReverseDeleteAndUpdate              // 1->*,*->2 = 1->2 - change type to Update     1 edititems req(resolved if all matching are deleted first)
    //}
}
