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
using System.IO;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
    /// <summary>
    /// Command to save the <see cref="UidMapper"/> used in the reconciliation.
    /// </summary>
    public class SaveUidMapXmlCommand : ServerCommand, IDisposable
    {
        #region Private Members
        private readonly UidMapper _map;
        private readonly StudyStorageLocation _studyLocation;
        private string _path;
        private string _backupPath; 
        #endregion

        #region Constructors

        public SaveUidMapXmlCommand(StudyStorageLocation studyLocation, UidMapper mapper) :
            base("SaveUidMap", true)
        {
            _studyLocation = studyLocation;
            _map = mapper;
        } 
        #endregion

        #region Overridden Protected Methods
        protected override void OnExecute(ServerCommandProcessor theProcessor)
        {
            if (_map == null)
                return;// nothing to save

            _path = Path.Combine(_studyLocation.GetStudyPath(), "UidMap.xml");
            if (RequiresRollback)
                Backup();

            _map.Save(_path);
        }

        protected override void OnUndo()
        {
            if (File.Exists(_backupPath))
            {
                File.Copy(_backupPath, _path, true);
            }
        } 
        #endregion

        #region Private Methods

        private void Backup()
        {
            _backupPath = FileUtils.Backup(_path);
        }
        
        #endregion

        #region Public Methods
        public void Dispose()
        {
            if (File.Exists(_backupPath))
            {
                File.Delete(_backupPath);
            }
        } 
        #endregion
    }
}