#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Text;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public class DatabaseRebuilder
    {
        public event EventHandler<ImageInsertCompletingEventArgs> ImageInsertCompletingEvent;
        public event EventHandler<ImageInsertCompletedEventArgs> ImageInsertCompletedEvent;
        public event EventHandler<DatabaseRebuildCompletedEventArgs> DatabaseRebuildCompletedEvent;
       
        public DatabaseRebuilder(String imageStoragePath, Boolean isSearchRecursive)
        {
            _dicomStore = SingleSessionDataAccessLayer.GetIDicomPersistentStore();
            _state = RebuilderState.Stopped;
            _imageStoragePath = imageStoragePath;
            _isSearchRecursive = isSearchRecursive;
            _fileList = Directory.GetFiles(_imageStoragePath,
                "*",
                _isSearchRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public String ImageStoragePath
        {
            get { return _imageStoragePath; }
            private set { _imageStoragePath = value; }
        }

        public Boolean IsSearchRecursive
        {
            get { return _isSearchRecursive; }
            private set { _isSearchRecursive = value; }
        }

        public Int32 NumberOfFiles
        {
            get { return _fileList.Length; }
        }

        public Boolean IsRebuilding
        {
            get { return (RebuilderState.Rebuilding == State); }
        }

        private RebuilderState State
        {
            get
            {
                lock (_synchronizationLock)
                {
                    return _state;
                }
            }

            set
            {
                lock (_synchronizationLock)
                {
                    _state = value;
                }
            }
        }

        private Boolean StopRequested
        {
            get
            {
                lock (_synchronizationLock)
                {
                    return _stopRequested;
                }
            }

            set
            {
                lock (_synchronizationLock)
                {
                    _stopRequested = value;
                }
            }
        }

        public void StartRebuild()
        {
            // TODO
            Platform.Log("Rebuild starting...");
            if (RebuilderState.Stopped != _state)
				throw new System.InvalidOperationException(SR.ExceptionCannotStartRebuilderUnlessStopped);

            State = RebuilderState.Rebuilding;
            Thread t = new Thread(new ThreadStart(DoRebuild));
            t.IsBackground = true;
            t.Start();
        }

        public void StopRebuild()
        {
            // TODO
            Platform.Log("Rebuild stopping...");
            if (RebuilderState.Rebuilding != _state)
				throw new System.InvalidOperationException(SR.ExceptionCannotStopRebuildUnlessStarted);

            StopRequested = true;
        }

        protected void OnImageInsertCompletedEvent(ImageInsertCompletedEventArgs args)
        {
            ClearCanvas.Common.Utilities.EventsHelper.Fire(ImageInsertCompletedEvent, this, args);
        }

        protected void OnImageInsertCompletingEvent(ImageInsertCompletingEventArgs args)
        {
            ClearCanvas.Common.Utilities.EventsHelper.Fire(ImageInsertCompletingEvent, this, args);
        }

        protected void OnDatabaseRebuildCompletedEvent(DatabaseRebuildCompletedEventArgs args)
        {
            ClearCanvas.Common.Utilities.EventsHelper.Fire(DatabaseRebuildCompletedEvent, this, args);
        }

        protected void DoRebuild()
        {
            StopRequested = false;
            bool rebuildWasAborted = false;

            foreach (string file in _fileList)
            {
                OnImageInsertCompletingEvent(new ImageInsertCompletingEventArgs(file));
                
                InsertSopInstance(file);
                
                OnImageInsertCompletedEvent(new ImageInsertCompletedEventArgs(file));

                if (StopRequested)
                {
                    State = RebuilderState.Stopped;
                    rebuildWasAborted = true;
                    break;
                }

                DatabaseFlush();
            }

            FinalDatabaseFlush();

            OnDatabaseRebuildCompletedEvent(new DatabaseRebuildCompletedEventArgs(rebuildWasAborted));
        }

        private void DatabaseFlush()
        {
            if (this.DicomStore.GetCachedStudiesCount() > 3)
                this.DicomStore.Flush();
        }

        private void FinalDatabaseFlush()
        {
            this.DicomStore.Flush();
        }

        protected void InsertSopInstance(string fileName)
        {
            DcmFileFormat file = new DcmFileFormat();
            OFCondition condition = file.loadFile(fileName);
            if (!condition.good())
            {
                // there was an error reading the file, possibly it's not a DICOM file
                return;
            }

            DcmMetaInfo metaInfo = file.getMetaInfo();
            DcmDataset dataset = file.getDataset();

            if (ConfirmProcessableFile(metaInfo, dataset))
            {
                this.DicomStore.InsertSopInstance(metaInfo, dataset, fileName);
            }

            // keep the file object alive until the end of this scope block
            // otherwise, it'll be GC'd and metaInfo and dataset will be gone
            // as well, even though they are needed in the InsertSopInstance
            // and sub methods
            GC.KeepAlive(file);
        }

        private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
        {
            StringBuilder stringValue = new StringBuilder(1024);
            OFCondition cond;
            cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
            if (cond.good())
            {
                // we want to skip Media Storage Directory Storage (DICOMDIR directories)
                if ("1.2.840.10008.1.3.10" == stringValue.ToString())
                    return false;
            }

            return true;
        }

        private DatabaseRebuilder() { }
        
        private IDicomPersistentStore DicomStore
        {
            get { return _dicomStore; }
        }

        private String _imageStoragePath;
        private Boolean _isSearchRecursive;
        private String[] _fileList;
        private Boolean _stopRequested = false;
        private Object _synchronizationLock = new Object();
        private enum RebuilderState { Stopped, Rebuilding };
        private RebuilderState _state;
        private IDicomPersistentStore _dicomStore;
    }
}
