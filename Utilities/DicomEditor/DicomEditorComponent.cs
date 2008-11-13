#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Anonymization;

namespace ClearCanvas.Utilities.DicomEditor
{
    [ExtensionPoint()]
	public sealed class DicomEditorToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

	public sealed class DicomEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IDicomEditorDumpManagement
    {
        int LoadedFileDumpIndex { get; set; }

        void RevertEdits(bool revertAll);

    	void Anonymize(bool applyToAll);

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

		//Edited.
        event EventHandler TagEdited;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void UpdateDisplay();

		bool IsLocalFile { get; }

		event EventHandler IsLocalFileChanged;
	}

    [AssociateView(typeof(DicomEditorComponentViewExtensionPoint))]
    public class DicomEditorComponent : ApplicationComponent, IDicomEditorDumpManagement
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

            public event EventHandler TagEdited
            {
                add { _component._tagEditedEvent += value; }
                remove { _component._tagEditedEvent -= value; }
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

        	public bool IsLocalFile
        	{
				get { return _component.IsLocalFile; }
        	}

			public event EventHandler IsLocalFileChanged
			{
				add { _component._isLocalFileChanged += value; }
				remove { _component._isLocalFileChanged -= value; }
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
                    _dirtyFlags[i] = false;
                }
            }
            else
            {
                _loadedFiles[_position] = new DicomFile(_loadedFiles[_position].Filename);
                _loadedFiles[_position].Load(DicomReadOptions.KeepGroupLengths);
                _dirtyFlags[_position] = false;
            }
        }

        public void Anonymize(bool applyToAll)
        {
            if (applyToAll == false)
            {
				_anonymizer.Anonymize(_loadedFiles[_position]);
            }
            else
            {
                for (int i = 0; i < _loadedFiles.Count; i++)
                {
					_anonymizer.Anonymize(_loadedFiles[i]);
                }
            }
        }

    	public void SaveAll()
        {
            for (int i = 0; i < _loadedFiles.Count; i++)
            {
				//TODO: deal with saving mixtures of local and non-local files.
                _loadedFiles[i].Save(DicomWriteOptions.Default);
                _dirtyFlags[i] = false;
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
                    _dirtyFlags[_position] = true;
                    EventsHelper.Fire(_tagEditedEvent, this, EventArgs.Empty);
                }
                else
                {
                    for (int i = 0; i < _loadedFiles.Count; i++)
                    {
                        if (this.IsMetainfoTag(tag))
                        {
                            _loadedFiles[i].MetaInfo[tag].SetStringValue(value);
                        }
                        else
                        {
                            _loadedFiles[i].DataSet[tag].SetStringValue(value);
                        }
                        _dirtyFlags[i] = true;
                        EventsHelper.Fire(_tagEditedEvent, this, EventArgs.Empty);
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
                _dirtyFlags[_position] = true;
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
                    _dirtyFlags[i] = true;
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
                                                                                                                                                            _dirtyFlags[_position] = true;
                                                                                                                                                            EventsHelper.Fire(_tagEditedEvent, this, EventArgs.Empty);
                                                                                                                                                        }
                                                                                                                                                    }, 1.0f, delegate(DicomEditorTag one, DicomEditorTag two) { return DicomEditorTag.TagCompare(one, two, SortType.Value); }));
            _title = "";
            _loadedFiles = new List<DicomFile>();
            _position = 0;
            _dirtyFlags = new List<bool>();

			_anonymizer = new DicomAnonymizer();
        	_anonymizer.Options = DicomAnonymizerOptions.RelaxAllChecks;
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

		public ActionModelNode ContextMenuModel
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

		/// <summary>
		/// Loads the specified DICOM file into the editor component.
		/// </summary>
		/// <remarks>
		/// <para>If the specified <paramref name="file">filename</paramref> does not exist, an exception will be thrown.</para>
		/// </remarks>
		/// <param name="file">The filename of the DICOM file to load.</param>
        public void Load(string file)
        {
            DicomFile dicomFile = new DicomFile(file);
           
            dicomFile.Load(ClearCanvas.Dicom.DicomReadOptions.Default);   
            
            _loadedFiles.Add(dicomFile);
            _position = 0;
            _dirtyFlags.Add(false);
			this.IsLocalFile = true;
        }

		/// <summary>
		/// Loads a copy of the specified in-memory DICOM file into the editor component.
		/// </summary>
		/// <remarks>
		/// <para>The editor will make changes to a new copy of the specified in-memory file, so changes do not propagate to any
		/// other places where the same file may be open.</para>
		/// <para>If the file exists on disk, then the editor will reload the file from disk to ensure all data is available.
		/// If the file does not exist on disk, then currently the editor opens in readonly mode with export functions disabled.</para>
		/// </remarks>
		/// <param name="file">The filename of the DICOM file to load.</param>
		public void Load(DicomFile file)
		{
			DicomFile dicomFile;
			bool isLocal = System.IO.File.Exists(file.Filename);
			if (isLocal)
			{
				// ideally, we would have some way to know if file is fully loaded,
				// so we know if we can simply copy the dataset instead of re-reading from disk
				dicomFile = new DicomFile(file.Filename);
				dicomFile.Load(ClearCanvas.Dicom.DicomReadOptions.Default);
			}
			else
			{
				dicomFile = new DicomFile(file.Filename, file.MetaInfo.Copy(true), file.DataSet.Copy(true));
				dicomFile.MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(file.TransferSyntaxUid);
			}

			_loadedFiles.Add(dicomFile);
			_position = 0;
			_dirtyFlags.Add(false);
			this.IsLocalFile = isLocal;
		}

        public void Clear()
        {
            _loadedFiles.Clear();
            _position = 0;
            _dirtyFlags.Clear();
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

            this.ReadAttributeCollection(_loadedFiles[_position].MetaInfo, null, 0);          
            this.ReadAttributeCollection(_loadedFiles[_position].DataSet, null,  0);

            this.DicomFileTitle = _loadedFiles[_position].Filename;

            EventsHelper.Fire(_displayedDumpChangedEvent, this, new DisplayedDumpChangedEventArgs(_position == 0, _position == (_loadedFiles.Count - 1), _loadedFiles.Count == 1, _dirtyFlags[_position] == true));
        }


        private void ReadAttributeCollection(DicomAttributeCollection set, DicomEditorTag parent, int nestingLevel)
        {
            foreach (DicomAttribute attribute in set)
            {
				if (attribute.IsEmpty)
					continue;

                if (attribute is DicomAttributeSQ)
                {
                    DicomEditorTag editorSq = new DicomEditorTag(attribute, parent, nestingLevel);
                    _dicomTagData.Items.Add(editorSq);

                    DicomSequenceItem[] items = (DicomSequenceItem[])((DicomAttributeSQ)attribute).Values;
                    if (items.Length != 0)
                    {       
                        DicomEditorTag editorSqItem;
                        DicomSequenceItem sequenceItem;
                        for(int i=0; i<items.Length; i++)
                        {
                            sequenceItem = items[i];

                            editorSqItem = new DicomEditorTag("fffe", "e000", "Sequence Item", editorSq, i, nestingLevel + 1);
                            _dicomTagData.Items.Add(editorSqItem);

                            this.ReadAttributeCollection(sequenceItem, editorSqItem, nestingLevel + 2);
                            //add SQ Item delimiter
                            _dicomTagData.Items.Add(new DicomEditorTag("fffe", "e00d", "Item Delimitation Item", editorSqItem, i, nestingLevel + 1));
                        }
                    }
                    //add SQ delimiter
                    _dicomTagData.Items.Add(new DicomEditorTag("fffe", "e0dd", "Sequence Delimitation Item", editorSq, items.Length, nestingLevel));
                }
                else
                {
                    _dicomTagData.Items.Add(new DicomEditorTag(attribute, parent, nestingLevel));
                }
            }
        }

        private bool IsMetainfoTag(uint attribute)
        {
            return attribute <= 267546;
        }

        public string DicomFileTitle
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("DicomFileTitle");
            }
        }

        public Table<DicomEditorTag> DicomTagData
        {
            get { return _dicomTagData; }
		}

    	private bool IsLocalFile
    	{
    		get { return _isLocalFile; }
			set
			{
				if (_isLocalFile != value)
				{
					_isLocalFile = value;
					EventsHelper.Fire(_isLocalFileChanged, this, new EventArgs());
				}
			}
    	}

        #region Private Members

    	private DicomAnonymizer _anonymizer;

		private string _title;
        
        private Table<DicomEditorTag> _dicomTagData;
       
        private List<DicomFile> _loadedFiles;
        private int _position;
        private List<bool> _dirtyFlags;

        private ISelection _currentSelection;
        private event EventHandler _selectedTagChangedEvent;
        private event EventHandler<DisplayedDumpChangedEventArgs> _displayedDumpChangedEvent;
        private event EventHandler _tagEditedEvent;

        private ToolSet _toolSet;
        private ClickHandlerDelegate _defaultActionHandler;
        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;

    	private bool _isLocalFile;
		private event EventHandler _isLocalFileChanged;

        #endregion

    }
}
