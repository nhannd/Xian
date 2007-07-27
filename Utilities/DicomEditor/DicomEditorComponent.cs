using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;
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

        void RevertEdits(bool revertAll);

        void RemoveAllPrivateTags(bool applyToAll);

        void SaveAll();

        bool TagExists(uint tag);

        void EditTag(uint tag, string value, bool applyToAll);

        void DeleteTag(uint tag, bool applyToAll);
    }

	public interface IDicomEditorToolContext : IToolContext
	{
        IDicomEditorDumpManagement DumpManagement { get; }

        DicomEditorTag SelectedTag { get; }

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
                Platform.CheckPositive(_loadedFiles.Count, "_loadedDicomDumps.Length");

                if (value < 0)
                    _position = 0;
                else if (value > _loadedFiles.Count - 1)
                    _position = _loadedFiles.Count - 1;
                else
                    _position = value;
            }
        }

        public void RevertEdits(bool revertAll)
        {
            if (revertAll == true)
            {
                for (int i = 0; i < _loadedFiles.Count; i++)
                {
                    _loadedFiles[i] = new DicomFile(_loadedFiles[i].Filename);
                    _loadedFiles[i].Load(DicomReadOptions.KeepGroupLengths);
                }
            }
            else
            {
                _loadedFiles[_position] = new DicomFile(_loadedFiles[_position].Filename);
                _loadedFiles[_position].Load(DicomReadOptions.KeepGroupLengths);
            }
        }

        public void RemoveAllPrivateTags(bool applyToAll)
        {
            List<DicomTag> privateTags;

            if (applyToAll == false)
            {
                privateTags = new List<DicomTag>();
                foreach (DicomAttribute attribute in _loadedFiles[_position].DataSet)
                {
                    if (attribute.Tag.Name == "Private Tag")
                        privateTags.Add(attribute.Tag);                        
                }

                foreach (DicomTag tag in privateTags)
                {
                    _loadedFiles[_position].DataSet[tag] = null;
                }
            }
            else
            {
                for (int i = 0; i < _loadedFiles.Count; i++)
                {
                    privateTags = new List<DicomTag>();
                    foreach (DicomAttribute attribute in _loadedFiles[i].DataSet)
                    {
                        if (attribute.Tag.Name == "Private Tag")
                            privateTags.Add(attribute.Tag);
                    }

                    foreach (DicomTag tag in privateTags)
                    {
                        _loadedFiles[i].DataSet[tag] = null;
                    }
                }
            }
        }

        public void SaveAll()
        {
            for (int i = 0; i < _loadedFiles.Count; i++)
            {
                _loadedFiles[i].Save(DicomWriteOptions.Default);
            }
        }

        public bool TagExists(uint tag)
        {
            if (this.IsMetainfoTag(tag))
            {
                return _loadedFiles[_position].MetaInfo.Contains(tag);
            }
            else
            {
                return _loadedFiles[_position].DataSet.Contains(tag);
            }
        }

        public void EditTag(uint tag, string value, bool applyToAll)
        {
                if (applyToAll == false)
                {
                    if (this.IsMetainfoTag(tag))
                    {
                        _loadedFiles[_position].MetaInfo[tag].SetStringValue(value);
                    }
                    else
                    {
                        _loadedFiles[_position].DataSet[tag].SetStringValue(value);
                    }
                }
                else
                {
                    for (int i = 0; i < _loadedFiles.Count; i++)
                    {
                        if (this.IsMetainfoTag(tag))
                        {
                            _loadedFiles[_position].MetaInfo[tag].SetStringValue(value);
                        }
                        else
                        {
                            _loadedFiles[i].DataSet[tag].SetStringValue(value);
                        }
                    }
                }
        }

        public void DeleteTag(uint tag, bool applyToAll)
        {
            if (applyToAll == false)
            {
                if (this.IsMetainfoTag(tag))
                {
                    _loadedFiles[_position].MetaInfo[tag] = null;
                }
                else
                {
                    _loadedFiles[_position].DataSet[tag] = null;
                }
            }
            else
            {
                for (int i = 0; i < _loadedFiles.Count; i++)
                {
                    if (this.IsMetainfoTag(tag))
                    {
                        _loadedFiles[i].MetaInfo[tag] = null;
                    }
                    else
                    {
                        _loadedFiles[i].DataSet[tag] = null;
                    }
                }
            }
        }    

        #endregion

        public DicomEditorComponent()
        {
            _dicomTagData = new Table<DicomEditorTag>();
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingGroupElement, delegate(DicomEditorTag d) { return d.DisplayKey; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.GroupElement); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingTagName, delegate(DicomEditorTag d) { return d.TagName; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.TagName); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingVR, delegate(DicomEditorTag d) { return d.Vr; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.Vr); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingLength, delegate(DicomEditorTag d) { return d.Length; }, null, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.Length); }));
            _dicomTagData.Columns.Add(new TableColumn<DicomEditorTag, string>(SR.ColumnHeadingValue, delegate(DicomEditorTag d) { return d.Value; }, delegate(DicomEditorTag d, string value)
                                                                                                                                                    {
                                                                                                                                                        if (d.IsEditable())
                                                                                                                                                        {
                                                                                                                                                            d.Value = value;
                                                                                                                                                        }
                                                                                                                                                    }, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.Value); }));
            _title = "";
            _loadedFiles = new List<DicomFile>();
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

        public void Load(string file)
        {
            DicomFile dicomFile = new DicomFile(file);
           
            dicomFile.Load(ClearCanvas.Dicom.DicomReadOptions.Default);   
            
            _loadedFiles.Add(dicomFile);
            _position = 0;
        }

        public void Clear()
        {
            _loadedFiles.Clear();
            _position = 0;

        }
        
        public void SetSelection(ISelection selection)
        {
            if (_currentSelection != selection)
            {
                _currentSelection = selection;
                EventsHelper.Fire(_selectedTagChangedEvent, this, EventArgs.Empty);
            }
        }

        public void UpdateComponent()
        {
            _dicomTagData.Items.Clear();

            this.ReadAttributeCollection(_loadedFiles[_position].MetaInfo, null, DisplayLevel.Attribute);          
            this.ReadAttributeCollection(_loadedFiles[_position].DataSet, null,  DisplayLevel.Attribute);

            this.DicomFileTitle = _loadedFiles[_position].Filename;

            EventsHelper.Fire(_displayedDumpChangedEvent, this, new DisplayedDumpChangedEventArgs(_position == 0, _position == (_loadedFiles.Count - 1), _loadedFiles.Count == 1));
        }


        private void ReadAttributeCollection(DicomAttributeCollection set, DicomEditorTag parent, DisplayLevel displayLevel)
        {
            foreach (DicomAttribute attribute in set)
            {
                if (attribute is DicomAttributeSQ)
                {
                    DicomEditorTag editorSq = new DicomEditorTag(attribute, null, displayLevel);
                    _dicomTagData.Items.Add(editorSq);

                    DicomSequenceItem[] items = (DicomSequenceItem[])((DicomAttributeSQ)attribute).Values;
                    if (items.Length != 0)
                    {
                        DicomEditorTag editorSqItem = new DicomEditorTag("fffe", "e000", "Sequence Item", editorSq, DisplayLevel.SequenceItem);
                        _dicomTagData.Items.Add(editorSqItem);

                        foreach (DicomSequenceItem sequenceItem in items)
                        {
                            this.ReadAttributeCollection(sequenceItem, editorSqItem, DisplayLevel.SequenceItemAttribute);
                        }
                    }
                }
                else
                {
                    _dicomTagData.Items.Add(new DicomEditorTag(attribute, parent, displayLevel));
                }
            }
        }

        private bool IsMetainfoTag(uint attribute)
        {
            return attribute <= 267546;
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

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
        private string _title;
        
        private Table<DicomEditorTag> _dicomTagData;
       
        private List<DicomFile> _loadedFiles;
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
