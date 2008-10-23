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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IPerformingWorkflowItemToolContext : IWorkflowItemToolContext<ModalityWorklistItem>
    {
    }

    public interface IPerformingWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

	public abstract class PerformingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ModalityWorklistItem, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IModalityWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
    {
		class PerformingWorkflowItemToolContext : WorkflowItemToolContext, IPerformingWorkflowItemToolContext
        {
            public PerformingWorkflowItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }

        class PerformingWorkflowFolderToolContext : WorkflowFolderToolContext, IPerformingWorkflowFolderToolContext
        {
            public PerformingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }


        protected PerformingWorkflowFolderSystemBase(string title)
            : base(title)
		{
        }

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new PerformingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new PerformingWorkflowItemToolContext(this);
		}
    }
}
