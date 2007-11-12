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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerGroupEditComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerGroupEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerGroupEditComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerGroupEditComponentViewExtensionPoint))]
    public class DicomServerGroupEditComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerGroupEditComponent(ServerTree dicomServerTree, ServerUpdateType updatedType)
        {
            _isNewServerGroup = updatedType.Equals(ServerUpdateType.Add)? true : false;
            _serverTree = dicomServerTree;
            if (!_isNewServerGroup)
            {
                _serverGroupName = _serverTree.CurrentNode.Name;
            }
            else
            {
                _serverGroupName = "";
            }

        }

        public void Accept()
        {
            if (!IsServerGroupNameValid() || !this.Modified)
                return;

            if (!_isNewServerGroup)
            {
                ServerGroup serverGroup = (ServerGroup)_serverTree.CurrentNode;
                serverGroup.NameOfGroup = _serverGroupName;
            }
            else
            {
                ServerGroup serverGroup = new ServerGroup(_serverGroupName);
                ((ServerGroup)_serverTree.CurrentNode).AddChild(serverGroup);
                _serverTree.CurrentNode = serverGroup;
            }

			_serverTree.Save();
            _serverTree.FireServerTreeUpdatedEvent();
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            Host.Exit();
            return;
        }

        public void Cancel()
        {
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        private bool IsServerGroupNameEmpty()
        {
            if (_serverGroupName == null || _serverGroupName.Equals("")
                || !_isNewServerGroup && _serverGroupName.Equals(_serverTree.CurrentNode.Name))
            {
                return false;
            }

            return true;
        }

        private bool IsServerGroupNameValid()
        {
            string conflictingPath = "";
        	bool valid = true;
			if (_isNewServerGroup && _serverTree.CanAddGroupToCurrentGroup(_serverGroupName, out conflictingPath))
				valid = false;
			else if (!_isNewServerGroup && _serverTree.CanEditCurrentGroup(_serverGroupName, out conflictingPath))
				valid = false;

			if (!valid)
			{
				this.Modified = false;
				StringBuilder msgText = new StringBuilder();
				msgText.AppendFormat(SR.FormatServerGroupConflict, _serverGroupName, conflictingPath);
				throw new DicomServerException(msgText.ToString());
			}

			return true;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Fields

        private ServerTree _serverTree;
        private string _serverGroupName = "";
        private bool _isNewServerGroup;

        public ServerTree ServerTree
        {
            get { return _serverTree; }
            set { _serverTree = value; }
        }

        public string ServerGroupName
        {
            get { return _serverGroupName; }
            set
            {
                _serverGroupName = value;
                this.Modified = IsServerGroupNameEmpty();
            }
        }

        #endregion

    }
}
