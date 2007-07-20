using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;
using System.ComponentModel;

namespace ClearCanvas.Utilities.DicomEditor
{
    [ExtensionPoint()]
    public class DicomEditorToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class DicomEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IDicomEditorDumpManagement
    {
        int LoadedFileDumpIndex { get; set; }

        void RevertAllEdits();

        void RemoveAllPrivateTags();

        void SaveAll();

        void ApplyEdit(DicomEditorTag Tag, EditType Type, bool ApplyToAll);
    }

	public interface IDicomEditorToolContext : IToolContext
	{
        IDicomEditorDumpManagement DumpManagement { get; }

		DicomEditorTag SelectedTag { get; }

        DicomDump DisplayedDump { get; }

		IList<DicomEditorTag> SelectedTags { get; }

		event EventHandler SelectedTagChanged;

        event EventHandler<DisplayedDumpChangedEventArgs> DisplayedDumpChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void UpdateDisplay();
	}

    [AssociateView(typeof(DicomEditorComponentViewExtensionPoint))]
    public class DicomEditorComponent : ApplicationComponent, INotifyPropertyChanged, IDicomEditorDumpManagement
    {
        public class DicomEditorToolContext : ToolContext, IDicomEditorToolContext
        {
            DicomEditorComponent _component;

            public DicomEditorToolContext(DicomEditorComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component;
            }

            #region IDicomEditorToolContext Members

            public IDicomEditorDumpManagement DumpManagement
            {
                get { return _component; }
            }

            public DicomEditorTag SelectedTag
            {
                get
                {
                    if (_component._currentSelection == null)
                        return null;

                    return _component._currentSelection.Item as DicomEditorTag;
                }
            }

            public DicomDump DisplayedDump
            {
                get
                {
                    if (_component._displayedDump == null)
                        return null;

                    return _component._displayedDump;
                }
            }

            public IList<DicomEditorTag> SelectedTags
            {
                get
                {
                    if (_component._currentSelection == null)
                        return null;

                    List<DicomEditorTag> selectedTags = new List<DicomEditorTag>();

                    foreach (DicomEditorTag tag in _component._currentSelection.Items)
                        selectedTags.Add(tag);

                    return selectedTags;
                }
            }

            public event EventHandler SelectedTagChanged
            {
                add { _component._selectedTagChangedEvent += value; }
                remove { _component._selectedTagChangedEvent -= value; }
            }

            public event EventHandler<DisplayedDumpChangedEventArgs> DisplayedDumpChanged
            {
                add { _component._displayedDumpChangedEvent += value; }
                remove { _component._displayedDumpChangedEvent -= value; }
            }

            public ClickHandlerDelegate DefaultActionHandler
            {
                get { return _component._defaultActionHandler; }
                set { _component._defaultActionHandler = value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public void UpdateDisplay()
            {
                _component.UpdateComponent();
            }

            #endregion
        }

        #region IDicomEditorDumpManagement Members

        public int LoadedFileDumpIndex
        {
            get { return _position; }
            set 
            {
                Platform.CheckPositive(_loadedDicomDumps.Count, "_loadedDicomDumps.Length");

                if (value < 0)
                    _position = 0;
                else if (value > _loadedDicomDumps.Count - 1)
                    _position = _loadedDicomDumps.Count - 1;
                else
                    _position = value;
            }
        }

        public void RevertAllEdits()
        {
            for (int i = 0; i < _loadedDicomDumps.Count; i++)
            {
                _loadedDicomDumps[i].RevertEdits();
            }
        }

        public void RemoveAllPrivateTags()
        {
            for (int i = 0; i < _loadedDicomDumps.Count; i++)
            {
                _loadedDicomDumps[i].RemovePrivateTags();
            }
        }

        public void SaveAll()
        {
            for (int i = 0; i < _loadedDicomDumps.Count; i++)
            {
                _loadedDicomDumps[i].Save();
            }
        }

        public void ApplyEdit(DicomEditorTag Tag, EditType Type, bool ApplyToAll)
        {
            if (ApplyToAll == false)
            {
                _loadedDicomDumps[_position].AddEditItem(new EditItem(Tag, Type));
            }
            else
            {
                for (int i = 0; i < _loadedDicomDumps.Count; i++)
                {
                    _loadedDicomDumps[i].AddEditItem(new EditItem(Tag, Type));
                }
            }
        }

        #endregion

        public DicomEditorComponent()
        {
            _dicomTagData = new Table<DicomEditorTag>();
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingGroupElement, delegate(DicomEditorTag d) { return d.DisplayKey; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return this.TagCompare(one, two, SortType.GroupElement); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingTagName, delegate(DicomEditorTag d) { return d.TagName; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return this.TagCompare(one, two, SortType.TagName); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingVR, delegate(DicomEditorTag d) { return d.Vr; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return this.TagCompare(one, two, SortType.Vr); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingLength, delegate(DicomEditorTag d) { return d.Length.ToString(); }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return this.TagCompare(one, two, SortType.Length); }));
			_dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingValue, delegate(DicomEditorTag d) { return d.Value; }, delegate(DicomEditorTag d, string value)
			{
                                                                                                                                                                if (this.IsTagEditable(d))
                                                                                                                                                                {
                                                                                                                                                                    d.Value = value;
                                                                                                                                                                    this.ChangeTagValue();
                                                                                                                                                                }
                                                                                                                                                            }, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return this.TagCompare(one, two, SortType.Value); }));

            _title = "";
            _loadedDicomDumps = new List<DicomDump>();
            _position = 0;
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        public ActionModelRoot ContextMenuModel
        {
            get { return _contextMenuModel; }
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new DicomEditorToolExtensionPoint(), new DicomEditorToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomeditor-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomeditor-contextmenu", _toolSet.Actions);

        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        public void UpdateComponent()
        {
            _displayedDump = _loadedDicomDumps[_position];
            _dicomTagData.Items.Clear();
            foreach (DicomEditorTag d in _displayedDump.DisplayTagList)
            {
                _dicomTagData.Items.Add(d);
            }
            this.DicomFileTitle = _displayedDump.Filename;
            EventsHelper.Fire(_displayedDumpChangedEvent, this, new DisplayedDumpChangedEventArgs(_position == 0, _position == (_loadedDicomDumps.Count - 1), _loadedDicomDumps.Count == 1));
        }

        public void SetSelection(ISelection selection)
        {
            if (_currentSelection != selection)
            {
                _currentSelection = selection;
                EventsHelper.Fire(_selectedTagChangedEvent, this, EventArgs.Empty);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(
                  this,
                  new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ChangeTagValue()
        {
            DicomEditorTag tag = (DicomEditorTag)this._currentSelection.Item;
            this.ApplyEdit(tag, EditType.Update, false);

            //while ApplyEditToCurrent already updates the underlying length this is needed to prevent flicker
            DicomEditorTag original = _displayedDump.GetOriginalTag(tag.UidKey);
            string groupLengthKey = DicomEditorTag.GenerateUidSearchKey(tag.Group, 0, tag.ParentTag);

            DicomEditorTag originalGroupLengthTag = this._displayedDump.GetOriginalTag(groupLengthKey);
            if (originalGroupLengthTag != null)
            {
                int index = _dicomTagData.Items.FindIndex(delegate(DicomEditorTag d) { return d.UidKey == groupLengthKey; });

                int groupLength = int.Parse(originalGroupLengthTag.Value) + tag.Length - original.Length;
                //_dicomTagData.Items[index].Value = groupLength.ToString();
                _dicomTagData.Items[index] = new DicomEditorTag(originalGroupLengthTag.Group, originalGroupLengthTag.Element, originalGroupLengthTag.TagName, originalGroupLengthTag.Vr, originalGroupLengthTag.Length, groupLength.ToString(), null, DisplayLevel.Attribute);
            }
        }

        private bool IsTagEditable(DicomEditorTag tag)
        {
            ICollection<string> unEditableVRList = new string[] { "SQ", @"??", "OB", "OW"};

            return !unEditableVRList.Contains(tag.Vr) && tag.ParentTag == null && !tag.DisplayKey.Contains(",0000)") && !tag.DisplayKey.Contains("(0002,") && !((tag.Vr == "UL" || tag.Vr == "US") && tag.Value.Contains(@"\"));
        }

        private int TagCompare(DicomEditorTag one, DicomEditorTag two, SortType type)
        {
            return one.SortKey(type).CompareTo(two.SortKey(type));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public IEnumerable<DicomDump> Dumps
        {
            set 
            {
                _loadedDicomDumps.Clear();
                _loadedDicomDumps.AddRange(value);
                _position = 0;

                this.UpdateComponent();
            }
        }

        public string DicomFileTitle
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("DicomFileTitle");
            }
        }

        public Table<DicomEditorTag> DicomTagData
        {
            get { return _dicomTagData; }
        }

        #region Private Members
        private Table<DicomEditorTag> _dicomTagData;

        private string _title;
        private DicomDump _displayedDump;
        private List<DicomDump> _loadedDicomDumps;
        private int _position;

        private ISelection _currentSelection;
        private event EventHandler _selectedTagChangedEvent;
        private event EventHandler<DisplayedDumpChangedEventArgs> _displayedDumpChangedEvent;

        private ToolSet _toolSet;
        private ClickHandlerDelegate _defaultActionHandler;
        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;
        #endregion

    }
}
