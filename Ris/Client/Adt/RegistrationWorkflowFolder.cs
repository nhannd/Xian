#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface IRegistrationWorkflowFolderDropContext : IDropContext
    {
        /// <summary>
        /// Gets the enablement of the specified operation from the folder system
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        bool GetOperationEnablement(string operationName);

        /// <summary>
        /// Gets the folder that is the drop target of the current operation
        /// </summary>
        RegistrationWorkflowFolder DropTargetFolder { get; }

        /// <summary>
        /// Gets the folder system that owns the drop target folder
        /// </summary>
        RegistrationWorkflowFolderSystemBase FolderSystem { get; }
    }

    public abstract class RegistrationWorkflowFolder : WorkflowFolder<RegistrationWorklistItem>
    {
        class DropContext : IRegistrationWorkflowFolderDropContext
        {
            private RegistrationWorkflowFolder _folder;

            public DropContext(RegistrationWorkflowFolder folder)
            {
                _folder = folder;
            }

            #region IRegistrationWorkflowFolderDropContext Members

            public bool GetOperationEnablement(string operationName)
            {
                return _folder._folderSystem.GetOperationEnablement(operationName);
            }

            public RegistrationWorkflowFolder DropTargetFolder
            {
                get { return _folder; }
            }

            public RegistrationWorkflowFolderSystemBase FolderSystem
            {
                get
                {
                    return _folder._folderSystem;
                }
            }

            #endregion

            #region IDropContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _folder._folderSystem.DesktopWindow; }
            }

            #endregion
        }


        private RegistrationWorkflowFolderSystemBase _folderSystem;
        private IconSet _closedIconSet;
        private IconSet _openIconSet;

        private readonly EntityRef _worklistRef;

        private string _worklistClassName;

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName, string folderDescription, EntityRef worklistRef, ExtensionPoint<IDropHandler<RegistrationWorklistItem>> dropHandlerExtensionPoint)
            : base(folderSystem, folderName, folderDescription, new RegistrationWorklistTable())
        {
            _folderSystem = folderSystem;

            _closedIconSet = new IconSet(IconScheme.Colour, "FolderClosedSmall.png", "FolderClosedMedium.png", "FolderClosedMedium.png");
            _openIconSet = new IconSet(IconScheme.Colour, "FolderOpenSmall.png", "FolderOpenMedium.png", "FolderOpenMedium.png");
            this.IconSet = _closedIconSet;
            this.ResourceResolver = new ResourceResolver(this.GetType().Assembly, this.ResourceResolver);
            if (dropHandlerExtensionPoint != null)
            {
                this.InitDragDropHandling(dropHandlerExtensionPoint, new DropContext(this));
            }

            _worklistRef = worklistRef;
        }

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName, ExtensionPoint<IDropHandler<RegistrationWorklistItem>> dropHandlerExtensionPoint)
            : this(folderSystem, folderName, null, null, dropHandlerExtensionPoint)
        {
        }

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName)
            :this(folderSystem, folderName, null, null, null)
        {
        }

        public RegistrationWorkflowFolder(RegistrationWorkflowFolderSystemBase folderSystem, string folderName, string folderDescription, EntityRef worklistRef)
            : this(folderSystem, folderName, folderDescription, worklistRef, null)
        {
        }

        public string WorklistClassName
        {
            get { return _worklistClassName; }
            set { _worklistClassName = value; }
        }

        public IconSet ClosedIconSet
        {
            get { return _closedIconSet; }
            set { _closedIconSet = value; }
        }

        public IconSet OpenIconSet
        {
            get { return _openIconSet; }
            set { _openIconSet = value; }
        }

        public override void OpenFolder()
        {
            if (_openIconSet != null)
                this.IconSet = _openIconSet;

            base.OpenFolder();
        }

        public override void CloseFolder()
        {
            if (_closedIconSet != null)
                this.IconSet = _closedIconSet;

            base.CloseFolder();
        }

        protected override bool CanQuery()
        {
            return true;
        }

        protected override IList<RegistrationWorklistItem> QueryItems()
        {
            List<RegistrationWorklistItem> worklistItems = null;

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistRequest request = _worklistRef == null 
                        ? new GetWorklistRequest(this.WorklistClassName) 
                        : new GetWorklistRequest(_worklistRef);

                    GetWorklistResponse response = service.GetWorklist(request);
                    worklistItems = response.WorklistItems;
                });

            return worklistItems ?? new List<RegistrationWorklistItem>();
        }

        protected override int QueryCount()
        {
            int count = -1;

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetWorklistCountRequest request = _worklistRef == null
                        ? new GetWorklistCountRequest(this.WorklistClassName)
                        : new GetWorklistCountRequest(_worklistRef);

                    GetWorklistCountResponse response = service.GetWorklistCount(request);
                    count = response.ItemCount;
                });

            return count;
        }

        protected override bool IsMember(RegistrationWorklistItem item)
        {
            throw new NotImplementedException();
        }

        public bool GetOperationEnablement(string operationName)
        {
            return _folderSystem.GetOperationEnablement(operationName);
        }
   }
}