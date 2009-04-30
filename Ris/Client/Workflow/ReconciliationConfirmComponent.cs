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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow
{
    /// <summary>
    /// Extension point for views onto <see cref="ConfirmReconciliationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ConfirmReconciliationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ConfirmReconciliationComponent class
    /// </summary>
    [AssociateView(typeof(ConfirmReconciliationComponentViewExtensionPoint))]
    public class ReconciliationConfirmComponent : ApplicationComponent
    {
        private PatientProfileTable _sourceProfiles;
        private PatientProfileTable _targetProfiles;


        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationConfirmComponent(IList<PatientProfileSummary> targets, IList<PatientProfileSummary> sources)
        {
            _sourceProfiles = new PatientProfileTable();
            _sourceProfiles.Items.AddRange(sources);

            _targetProfiles = new PatientProfileTable();
            _targetProfiles.Items.AddRange(targets);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ITable SourcePatientData
        {
            get { return _sourceProfiles; }
        }

        public ITable TargetPatientData
        {
            get { return _targetProfiles; }
        }

        public void Continue()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

    }
}
