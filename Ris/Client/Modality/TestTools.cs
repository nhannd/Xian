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
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Modality
{
    public class TestTools
    {
        public abstract class WorkflowTool : Tool<IModalityWorkflowToolContext>
        {
            private string _operationClass;

            public WorkflowTool(string operationClass)
            {
                _operationClass = operationClass;
            }

            public bool Enabled
            {
                get
                {
                    return this.Context.GetWorkflowOperationEnablement(_operationClass);
                }
            }

            public event EventHandler EnabledChanged
            {
                add { this.Context.SelectedItemsChanged += value; }
                remove { this.Context.SelectedItemsChanged -= value; }
            }

            public void Apply()
            {
                this.Context.ExecuteWorkflowOperation(_operationClass);
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Start")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class StartTool : WorkflowTool
        {
            public StartTool()
                :base("ClearCanvas.Healthcare.Workflow.Modality.Operations+StartModalityProcedureStep")
            {
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Complete")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class CompleteTool : WorkflowTool
        {
            public CompleteTool()
                : base("ClearCanvas.Healthcare.Workflow.Modality.Operations+CompleteModalityProcedureStep")
            {
            }
        }

        [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel")]
        [ClickHandler("apply", "Apply")]
        [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
        [ExtensionOf(typeof(ModalityWorkflowToolExtensionPoint))]
        public class CancelTool : WorkflowTool
        {
            public CancelTool()
                : base("ClearCanvas.Healthcare.Workflow.Modality.Operations+CancelModalityProcedureStep")
            {
            }
        }


    }
}
